using UnityEngine;
using System;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class FNetManager : FMonoSingleton<FNetManager>
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

        private const int TcpClientReceiveBufferSize = 1024;
        private const int TcpClientReceiveTimeout = 30000;
        private const int TcpClientSendBufferSize = 1024;
        private const int TcpClientSendTimeout = 30000;
        private const float TcpConnectTimeout = 6.0f;

        private NETSTATE _NetState = NETSTATE.Init;
        public NETSTATE NetState
        {
            get { return _NetState; }
            set { _NetState = value; }
        }

        public delegate void NetConnectionCallback(NETEVENT netEvent);
        private List<byte[]> _bufToSend;
        private List<byte[]> _bufFromSevr;
        private long _connectTimerId = 0;
        private long _retryTimerId = 0;
        private int _retryTimes = 3;

        private ConnectThread _connecter;
        private ReceiverThread _receiver;
        private SenderThread _sender;

        private TcpClient _tcpClient;
        private string _NetHostName = "testlogin.bluff.qq.com";
        private int _Port = 8080;

        private FNetTaskExecutor _taskExecutor;

        public event NetConnectionCallback netConnectionCallback;

        private bool _stopped = true;

        public UIButton openBtn;
        public UIButton closeBtn;
        public UILabel label;

        void Awake()
        {
            _bufToSend = new List<byte[]>();
            _bufFromSevr = new List<byte[]>();
            _taskExecutor = new FNetTaskExecutor();
        }

        void Update()
        {
            if (!_stopped)
            {
                _taskExecutor.Update();
                CheckReConnect();
            }
        }

        bool IsNotReachable()
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }

        bool IsConnected()
        {
            return _tcpClient != null && NetState == NETSTATE.Connected;
        }

        void ConnectServer()
        {
            if (NetState == NETSTATE.Init || NetState == NETSTATE.DisConnected)
            {
                _stopped = false;
                ConnectServer(_NetHostName, _Port);
            }
            else
            {
                FLog.Debug("ConnectServer Error: NetState = " + NetState.ToString());
            }
        }

        void ConnectServer(string strIP, int nPort)
        {
            FLog.Debug("ConnectServer: " + strIP.ToString() + ":" + nPort.ToString());
            CloseConnect();
            _Port = nPort;
            EnableRetry(true);
            StartConnecting();
        }

        void StopConnectServer()
        {
            FLog.Debug("StopConnectServer");
            _stopped = true;
            CloseConnect();
            EnableRetry(false);

            RemoveConnectTimer();
            RemoveRetryTimer();
            _taskExecutor.Clear();
        }

        void OnApplicationQuit()
        {
            StopConnectServer();
        }

        void OnApplicationPause(bool isPause)
        {
            FLog.Debug("NetConnection OnApplicationPause = " + isPause.ToString());
            NotifyApplicationPause(isPause);
        }

        void StartConnecting()
        {
            if (NetState == NETSTATE.ConnectFailed || NetState == NETSTATE.DisConnected)
            {
                NetState = NETSTATE.Connecting;
                if (netConnectionCallback != null)
                {
                    netConnectionCallback(NETEVENT.Connecting);
                }

                BeginConnectTimer();
                StartConnectThread();
            }
            else
            {
                FLog.Debug("StartConnecting ignore, NetState=" + NetState.ToString());
            }
        }

        void ConnectTimeout()
        {
            if (_tcpClient != null && !_tcpClient.Connected)
            {
                FLog.Debug("BeginConnect Timeout, Close socket");
                CloseTcpClient();
                if (_connecter != null)
                {
                    _connecter.ClearCallback();
                    _connecter = null;
                }
                ConnectFail();
            }
            else
            {
                FLog.Debug("BeginConnect m_tcpClient is connected");
            }
        }

        void BeginConnectTimer()
        {
            _connectTimerId = TimerManager.instance.SetTimeout(delegate (long id, object p1, object p2)
            {
                FLog.Debug("ConnectTimeout");
                ConnectTimeout();
            }, TcpConnectTimeout);
        }

        void RemoveConnectTimer()
        {
            TimerManager.instance.ClearTimeout(_connectTimerId);
        }

        void BeginRetryTimer()
        {
            _retryTimerId = TimerManager.instance.SetTimeout(delegate (long id, object p1, object p2)
            {
                FLog.Debug("RetryTimes = " + _retryTimes.ToString());
                StartConnecting();
            }, 0.3f);
        }

        void RemoveRetryTimer()
        {
            TimerManager.instance.ClearTimeout(_retryTimerId);
        }

        void CheckReConnect()
        {
            if (NetState == NETSTATE.DisConnected && !IsNotReachable() && _retryTimes > 0)
            {
                FLog.Debug("CheckReConnect, Connecting");
                StartConnecting();
            }
        }

        void SendMsg(byte[] msg)
        {
            if (msg != null)
            {
                lock (_bufToSend)
                    _bufToSend.Add(msg);
            }
        }

        void EnqueueRespondMsg(byte[] msg)
        {
            if (msg != null)
            {
                lock (_bufFromSevr)
                    _bufFromSevr.Add(msg);
            }
        }

        void StartReceiveThread()
        {
            if (_tcpClient == null)
            {
                return;
            }
            _receiver = new ReceiverThread(_tcpClient.GetStream(), EnqueueRespondMsg, delegate ()
            {
                FLog.Debug("ReceiveThread Callback");
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

            _connecter = new ConnectThread(_tcpClient, _NetHostName, _Port, delegate (bool bOK)
            {
                FLog.Debug("ConnectThread Callback: " + bOK.ToString());
                if (bOK)
                    _taskExecutor.Add(ConnectOK);
                else
                    _taskExecutor.Add(ConnectFail);

                if (_connecter != null)
                {
                    _connecter.ClearCallback();
                    _connecter = null;
                }
            });
            _connecter.Run();
        }

        void StartSendThread()
        {
            if (_tcpClient == null)
                return;

            _sender = new SenderThread(_tcpClient.GetStream(), _bufToSend);
            _sender.Run();
        }

        void EnableRetry(bool isEnable)
        {
            _retryTimes = isEnable ? 3 : 0;
            FLog.Debug("EnableRetry: " + isEnable.ToString());
        }

        void NotifyApplicationPause(bool isPause)
        {
            if (isPause)
                EnableRetry(false);
            else if (!_stopped)
                EnableRetry(true);
        }

        void NotifyDisConnect()
        {
            FLog.Debug("NotifyDisConnect");

            if (NetState == NETSTATE.Connected)
                CloseConnect();
            else
                FLog.Debug("NotifyDisConnect NetState = " + NetState.ToString());
        }

        void OnNetConnectionCallback(NETEVENT netEvent)
        {
            FLog.Debug("OnNetConnectionCallback, netEvent = " + netEvent.ToString());
        }

        void CloseTcpClient()
        {
            if (_tcpClient == null)
            {
                FLog.Debug("CloseTcpClient already");
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
                    FLog.Debug("CloseTcpClient Stream Close exception: " + ex.ToString());
                }
            }

            try
            {
                _tcpClient.Close();
            }
            catch (Exception ex)
            {
                FLog.Debug("CloseTcpClient TcpClient Close exception: " + ex.ToString());
            }

            _tcpClient = null;
        }

        void CloseConnect()
        {
            NetState = NETSTATE.DisConnected;
            FLog.Debug("CloseConnect");

            lock (_bufToSend)
            {
                _bufToSend.Clear();
            }

            SetTerminateFlag();

            if (_tcpClient == null)
                return;

            CloseTcpClient();

            if (netConnectionCallback != null)
                netConnectionCallback(NETEVENT.Close);
        }

        void SetTerminateFlag()
        {
            if (_sender != null)
            {
                _sender.SetTerminateFlag();
                //_sender.Interrupt();
                //_sender.Abort();
            }

            if (_receiver != null)
            {
                _receiver.SetTerminateFlag();
                //_receiver.Interrupt();
                //_receiver.Abort();
            }
        }

        void ConnectOK()
        {
            FLog.Debug("ConnectOK");

            if (NetState == NETSTATE.Connecting)
            {
                NetState = NETSTATE.Connected;
                RemoveConnectTimer();
                StartSendThread();
                StartReceiveThread();

                EnableRetry(true);

                if (netConnectionCallback != null)
                    netConnectionCallback(NETEVENT.Success);
            }
            else
            {
                FLog.Debug("ConnectOK ignore, NetState = " + NetState.ToString());
            }
        }

        void ConnectFail()
        {
            FLog.Debug("ConnectFail");

            if (NetState == NETSTATE.Connecting)
            {
                NetState = NETSTATE.ConnectFailed;
                RemoveConnectTimer();
                if (--_retryTimes > 0)
                {
                    CloseTcpClient();
                    BeginRetryTimer();
                }
                else
                {
                    CloseConnect();

                    if (netConnectionCallback != null)
                        netConnectionCallback(NETEVENT.RetryFail);
                }
            }
            else
            {
                FLog.Debug("ConnnectFail ignore, NetState = " + NetState.ToString());
            }
        }
    }
}