using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    /// <summary>
    /// 1.iOS 切换到后台后，主线程和收发线程都会被立刻停掉，在后台期间的网络包会被压到系统缓存。当再次唤醒时会被全部推到Socket，所以会连续收到很多数据包
    /// 2.Android 切换到后台后，主线程UI线程的Update不会被调用，但是收发线程不受影响
    /// </summary>
	public class NetManager : MonoSingleton<NetManager>
    {
        public enum NETEVENT
        {
            Connecting,
            Success,
            Close,
            RetryFail,
            NotReachable,
        };

        public enum NETSTATE
        {
            Init = 0,
            Connecting,
            Connected,
            ConnectFailed,
            DisConnected
        }

        public const int TcpClientReceiveBufferSize = 1024 * 1024;
        public const int TcpClientReceiveTimeout = 30000;
        public const int TcpClientSendBufferSize = 1024 * 1024;
        public const int TcpClientSendTimeout = 30000;
        public const float TcpConnectTimeout = 6.0f;
        public const float HeartBeatTime = 8.0f;
        public const float HelloResponseTimeout = 4.0f;

        NETSTATE _netState = NETSTATE.Init;
        public NETSTATE netState
        {
            get { return _netState; }
            set { _netState = value; }
        }

        List<byte[]> _bufferToSend;
        List<byte[]> _bufferFromServer;

        float _heartBeatTimeCount = 0;
        bool _helloHasResponse = false;

        long _sendHelloTimerId = 0;
        long _connectTimerId = 0;

        long _retryTimerId = 0;
        int _retryTimes = 3;

        float _lockDispatcherSecond = 0.0f;

        bool _stopped = true;

        string _hostName = "xxx.yyy.zzz.com";
        int _port = 8080;

        ConnectThread _connecter;
        ReceiverThread _receiver;
        SenderThread _sender;

        TcpClient _tcpClient;
        NetTaskExecutor _taskExecutor;

        public event Action<NETEVENT> netConnectionCallback;

        void Awake()
        {
            _bufferToSend = new List<byte[]>();
            _bufferFromServer = new List<byte[]>();
            _taskExecutor = new NetTaskExecutor();
        }

        void Update()
        {
            if (!_stopped)
            {
                _taskExecutor.Update();
                CheckReConnect();
                CheckLock();
                FireEvent();
                SendHello();
            }
        }

        public void LockEvent(float second)
        {
            Log.Debug("LockEventDispatcher: " + second.ToString());
            _lockDispatcherSecond += second;
        }

        public void UnLockEvent()
        {
            Log.Debug("UnLockEventDispatcher: " + _lockDispatcherSecond.ToString());
            _lockDispatcherSecond = 0.0f;
        }

        bool IsLockEvent()
        {
            return _lockDispatcherSecond > 0.0f;
        }

        void CheckLock()
        {
            if (_lockDispatcherSecond > 0.0f)
            {
                _lockDispatcherSecond -= Time.deltaTime;

                if (_lockDispatcherSecond < 0.0f)
                {
                    UnLockEvent();
                }
            }
        }

        public bool IsConnected()
        {
            return _tcpClient != null && netState == NETSTATE.Connected;
        }

        public static bool IsNotReachable()
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }

        public void SendPacket(byte[] buffer)
        {
            if (netState == NETSTATE.Connected)
            {
                if (buffer != null)
                {
                    lock (_bufferToSend)
                    {
                        _bufferToSend.Add(buffer);
                    }
                }
                else
                {
                    Log.Debug("SendPacket buffer is null.");
                }
            }
            else
            {
                Log.Debug("SendPacket ignore, netState = " + netState.ToString());
            }
        }

        void EnqueueResponsePacket(byte[] buffer)
        {
            if (buffer == null) return;

            lock (_bufferFromServer)
            {
                _bufferFromServer.Add(buffer);
            }
        }

        public void ConnectServer()
        {
            if (netState == NETSTATE.Init || netState == NETSTATE.DisConnected)
            {
                _stopped = false;
                ConnectServer(_hostName, _port);
            }
            else
            {
                Log.Debug("ConnectServer error, netState = " + netState.ToString());
            }
        }

        void ConnectServer(string host, int port)
        {
            Log.Debug("ConnectServer: host = " + host + ", port = " + port.ToString());

            _port = port;

            CloseConnect();
            EnableRetry(true);
            StartConnecting();
        }

        void StartConnecting()
        {
            if (netState == NETSTATE.ConnectFailed || netState == NETSTATE.DisConnected)
            {
                netState = NETSTATE.Connecting;
                netConnectionCallback.Call(NETEVENT.Connecting);

                BeginConnectTimer();
                StartConnectThread();
            }
            else
            {
                Log.Debug("StartConnecting ignore, netState = " + netState.ToString());
            }
        }

        public void StopConnectServer()
        {
            Log.Debug("StopConnectServer");

            _stopped = true;

            CloseConnect();
            EnableRetry(false);

            RemoveConnectTimer();
            RemoveRetryTimer();
            TimeoutManager.instance.ClearTimeout(_sendHelloTimerId);
            _taskExecutor.Clear();
        }

        void ConnectTimeout()
        {
            if (_tcpClient != null && !_tcpClient.Connected)
            {
                Log.Debug("BeginConnect timeout and close socket");

                CloseTcpClient();

                if (_connecter != null)
                {
                    _connecter.ClearCallback();
                    _connecter = null;
                }

                ConnectFail();
            }
        }

        void BeginConnectTimer()
        {
            _connectTimerId = TimeoutManager.instance.CreateTimeout(delegate (long id, object param)
            {
                ConnectTimeout();
            }, TcpConnectTimeout);
        }

        void RemoveConnectTimer()
        {
            TimeoutManager.instance.ClearTimeout(_connectTimerId);
        }

        void BeginRetryTimer()
        {
            _retryTimerId = TimeoutManager.instance.CreateTimeout(delegate (long id, object param)
            {
                Log.Debug("Retry times = " + _retryTimes.ToString());
                StartConnecting();
            }, 0.3f);
        }

        void RemoveRetryTimer()
        {
            TimeoutManager.instance.ClearTimeout(_retryTimerId);
        }

        void SendHello()
        {
            _heartBeatTimeCount += Time.deltaTime;

            if (_heartBeatTimeCount > HeartBeatTime && IsConnected())
            {
                byte[] hello = new byte[1];

                SendPacket(hello);

                _heartBeatTimeCount = 0.0f;
                _helloHasResponse = false;

                _sendHelloTimerId = TimeoutManager.instance.CreateTimeout(delegate (long id, object param)
                {
                    if (!_helloHasResponse)
                    {
                        Log.Debug("HelloPacket response timeout");
                        NotifyDisConnect();
                    }
                }, HelloResponseTimeout);
            }

            if (_helloHasResponse && _sendHelloTimerId > 0)
            {
                TimeoutManager.instance.ClearTimeout(_sendHelloTimerId);
                _sendHelloTimerId = -1;
            }
        }

        void FireEvent()
        {
            lock (_bufferFromServer)
            {
                while (_bufferFromServer.Count() > 0 && !IsLockEvent())
                {
                    byte[] buffer = _bufferFromServer[0];
                    EventDispatcherManager.instance.FireEvent(MSGID.Net, buffer);
                    _bufferFromServer.RemoveAt(0);
                }
            }
        }

        void CheckReConnect()
        {
            if (netState == NETSTATE.DisConnected && !IsNotReachable() && _retryTimes > 0)
            {
                Log.Debug("CheckReConnect connecting");
                StartConnecting();
            }
        }

        void StartReceiveThread()
        {
            if (_tcpClient == null)
            {
                return;
            }

            _receiver = new ReceiverThread(_tcpClient.GetStream(), EnqueueResponsePacket, delegate ()
            {
                Log.Debug("ReceiveThread failed");
                _taskExecutor.Add(NotifyDisConnect);
            });

            _receiver.Run();
        }

        void StartConnectThread()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
                _tcpClient.NoDelay = true;
                _tcpClient.ReceiveBufferSize = TcpClientReceiveBufferSize;
                _tcpClient.ReceiveTimeout = TcpClientReceiveTimeout;
                _tcpClient.SendBufferSize = TcpClientSendBufferSize;
                _tcpClient.SendTimeout = TcpClientSendTimeout;
            }

            _connecter = new ConnectThread(_tcpClient, _hostName, _port, delegate (bool ok)
            {
                Log.Debug("ConnectThread callback: " + ok.ToString());

                if (ok)
                {
                    _taskExecutor.Add(ConnectOK);
                }
                else
                {
                    _taskExecutor.Add(ConnectFail);
                }

                _connecter.ClearCallback();
                _connecter = null;
            });

            _connecter.Run();
        }

        void StartSendThread()
        {
            if (_tcpClient == null)
            {
                return;
            }

            _sender = new SenderThread(_tcpClient.GetStream(), _bufferToSend);
            _sender.Run();
        }

        void EnableRetry(bool isEnable)
        {
            _retryTimes = isEnable ? 3 : 0;
            Log.Debug("EnableRetry: " + isEnable.ToString());
        }

        void NotifyApplicationPause(bool isPause)
        {
            if (isPause)
            {
                EnableRetry(false);
            }
            else if (!_stopped)
            {
                EnableRetry(true);
            }
        }

        void NotifyDisConnect()
        {
            Log.Debug("NotifyDisConnect");

            if (netState == NETSTATE.Connected)
            {
                CloseConnect();
            }
            else
            {
                Log.Debug("NotifyDisConnect ignore, netState = " + netState.ToString());
            }
        }

        void CloseTcpClient()
        {
            if (_tcpClient == null)
            {
                Log.Debug("TcpClient has closed already");
                return;
            }

            if (_tcpClient.Connected)
            {
                try
                {
                    _tcpClient.GetStream().Close();
                }
                catch (Exception ex)
                {
                    Log.Debug("CloseTcpClient Stream Exception: " + ex.ToString());
                }
            }

            try
            {
                _tcpClient.Close();
            }
            catch (System.Exception ex)
            {
                Log.Debug("CloseTcpClient TcpClient Exception: " + ex.ToString());
            }

            _tcpClient = null;
        }

        void CloseConnect()
        {
            netState = NETSTATE.DisConnected;
            Log.Debug("CloseConnect");

            lock (_bufferToSend)
            {
                _bufferToSend.Clear();
            }

            _heartBeatTimeCount = 0;
            _helloHasResponse = false;
            TimeoutManager.instance.ClearTimeout(_sendHelloTimerId);
            SetTerminated();

            if (_tcpClient == null)
            {
                return;
            }

            CloseTcpClient();

            netConnectionCallback.Call(NETEVENT.Close);
        }

        void SetTerminated()
        {
            if (_sender != null)
                _sender.SetTerminated();

            if (_receiver != null)
                _receiver.SetTerminated();
        }

        void ConnectOK()
        {
            Log.Debug("ConnectOK");

            if (netState == NETSTATE.Connecting)
            {
                netState = NETSTATE.Connected;

                RemoveConnectTimer();
                StartSendThread();
                StartReceiveThread();

                EnableRetry(true);

                netConnectionCallback.Call(NETEVENT.Success);
            }
            else
            {
                Log.Debug("ConnectOK ignore, netState = " + netState.ToString());
            }
        }

        void ConnectFail()
        {
            Log.Debug("ConnectFail");

            if (netState == NETSTATE.Connecting)
            {
                netState = NETSTATE.ConnectFailed;

                RemoveConnectTimer();

                if (--_retryTimes > 0)
                {
                    CloseTcpClient();
                    BeginRetryTimer();
                }
                else
                {
                    CloseConnect();
                    netConnectionCallback.Call(NETEVENT.RetryFail);
                }
            }
            else
            {
                Log.Debug("ConnnectFail ignore, netState = " + netState.ToString());
            }
        }

        void OnApplicationQuit()
        {
            StopConnectServer();
        }

        void OnApplicationPause(bool isPause)
        {
            Log.Debug("NetConnection OnApplicationPause = " + isPause.ToString());
            NotifyApplicationPause(isPause);
        }
    }
}
