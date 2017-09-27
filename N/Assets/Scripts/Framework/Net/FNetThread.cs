using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Framework
{
    class ConnectThread : FThread
    {
        private TcpClient _tcpClient;
        private string _hostName;
        private int _port;
        private FCallback.FunBool _fCallback;

        public ConnectThread(TcpClient tcpClient, string hostName, int port, FCallback.FunBool fCallback)
        {
            _tcpClient = tcpClient;
            _hostName = hostName;
            _port = port;
            _fCallback = fCallback;
        }

        protected override void Main()
        {
            FLog.Debug("ConnectThread.Main: Begin");

            try
            {
                _tcpClient.Connect(_hostName, _port);
                if (_fCallback != null)
                    _fCallback(true);
            }
            catch (ObjectDisposedException ex)
            {
                FLog.Debug("Connect ObjectDisposedException: " + ex.Message);
            }
            catch (Exception ex)
            {
                FLog.Debug("Connect Exception: " + ex.Message);
                if (_fCallback != null)
                    _fCallback(false);
            }

            FLog.Debug("ConnectThread.Main: End");
        }

        public void ClearCallback()
        {
            _fCallback = null;
        }
    }

    class ReceiverThread : FThread
    {
        public delegate void OnReceiveFail();
        public delegate void OnReceiveData(byte[] data);

        private const uint MaxBufferSize = 1024;

        private byte[] _recBuf;
        private int _recBufOffset;

        private OnReceiveFail _fOnReceiveFail = null;
        private OnReceiveData _fOnReceiveData = null;

        private NetworkStream _netStream;

        public ReceiverThread(NetworkStream stream, OnReceiveData fOnReceiveData, OnReceiveFail fOnReceiveFail) : base()
        {
            _netStream = stream;
            _recBuf = new byte[2 * MaxBufferSize];
            _recBufOffset = 0;

            _fOnReceiveData = fOnReceiveData;
            _fOnReceiveFail = fOnReceiveFail;
        }

        protected override void Main()
        {
            FLog.Debug("ReceiverThread.Main: Begin");

            while (!IsTerminateFlagSet())
            {
                ReadFromStream();
                ScanPackets();
            }

            FLog.Debug("ReceiverThread.Main: End");
        }

        protected void ReadFromStream()
        {
            if (_netStream.CanRead)
            {
                try
                {
                    int length = _netStream.Read(_recBuf, _recBufOffset, _recBuf.Length - _recBufOffset);
                    _recBufOffset += length;

                    if (length == 0)
                    {
                        FLog.Debug("NetStream.Read length = 0");
                        if (_fOnReceiveFail != null)
                            _fOnReceiveFail();
                        SetTerminateFlag();
                    }
                }
                catch (Exception ex)
                {
                    FLog.Error("ReceiverThread Exception:" + ex.ToString());
                    if (_fOnReceiveFail != null)
                        _fOnReceiveFail();
                    SetTerminateFlag();
                }
            }
            else
            {
                Thread.Sleep(16);
            }
        }

        protected void ScanPackets()
        {
            while (_recBufOffset > 0 && !IsTerminateFlagSet())
            {
                byte[] _buffer = new byte[_recBufOffset];
                Array.Copy(_recBuf, 0, _buffer, 0, _buffer.Length);
                if (_fOnReceiveData != null)
                    _fOnReceiveData(_buffer);
                _recBufOffset = 0;
            }
        }
    }

    class SenderThread : FThread
    {
        private List<byte[]> _msgToSend = null;
        const int MaxDataSize = 1024;

        private NetworkStream _netStream;

        public SenderThread(NetworkStream stream, List<byte[]> msgToSend) : base()
        {
            _netStream = stream;
            _msgToSend = msgToSend;
        }

        protected override void Main()
        {
            FLog.Debug("SenderThread.Main: Begin");

            while (!IsTerminateFlagSet())
            {
                bool sleep = false;
                byte[] _buffer = null;

                lock (_msgToSend)
                {
                    if (_msgToSend.Count > 0)
                        _buffer = _msgToSend[0];
                    else
                        sleep = true;
                }

                if (_buffer != null)
                {
                    try
                    {
                        _netStream.Write(_buffer, 0, _buffer.Length);
                        _netStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        FLog.Error(string.Format("SenderThead, Main Write Exception, {0}, {1}, {2}", ex.Message, ex.StackTrace, ex.InnerException.Message));
                    }

                    lock (_msgToSend)
                    {
                        if (_msgToSend.Count > 0)
                            _msgToSend.RemoveAt(0);
                    }
                }

                if (sleep)
                    Thread.Sleep(15);
            }

            FLog.Debug("SenderThread.Main: End");
        }
    }
}