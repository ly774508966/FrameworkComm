using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    class ConnectThread : BaseThread
    {
        private TcpClient _tcpClient;
        private string _hostName;
        private int _port;
        private Action<bool> _callback;

        public ConnectThread(TcpClient tcpClient, string hostName, int port, Action<bool> callback)
        {
            _tcpClient = tcpClient;
            _hostName = hostName;
            _port = port;
            _callback = callback;
        }

        public void ClearCallback()
        {
            _callback = null;
        }

        protected override void Main()
        {
            Log.Debug("ConnectThread.Main: Begin");

            try
            {
                _tcpClient.Connect(_hostName, _port);
                _callback(true);
            }
            catch (ObjectDisposedException ex)  // closed by timeout.
            {
                Log.Debug("Connect ObjectDisposedException: " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.Debug("Connect Exception: " + ex.Message);
                _callback(false);
            }

            Log.Debug("ConnectThread.Main: End");
        }
    }

    class ReceiverThread : BaseThread
    {
        const uint MaxPacketSize = 1024 * 512;
        const short PackageHeaderSize = 2;

        byte[] _recvBuffer;
        int _recvBufferOffset;

        Action _onReceiveFail = null;
        Action<byte[]> _onReceivePackage = null;

        NetworkStream _netStream;

        public ReceiverThread(NetworkStream stream, Action<byte[]> onReceivePackage, Action onReceiveFail) : base()
        {
            _netStream = stream;
            _recvBuffer = new byte[2 * MaxPacketSize];
            _recvBufferOffset = 0;

            _onReceivePackage = onReceivePackage;
            _onReceiveFail = onReceiveFail;
        }

        protected override void Main()
        {
            Log.Debug("ReceiverThread.Main: Begin");

            while (!CheckTerminated())
            {
                ReadFromStream();
                ScanPackets();
            }

            Log.Debug("ReceiverThread.Main: End");
        }

        protected void ReadFromStream()
        {
            if (_netStream.CanRead)
            {
                try
                {
                    int length = _netStream.Read(_recvBuffer, _recvBufferOffset, _recvBuffer.Length - _recvBufferOffset);

                    _recvBufferOffset += length;

                    if (length == 0)
                    {
                        Log.Debug("NetStream.Read length = 0");
                        _onReceiveFail.Call();
                        SetTerminated();
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("ReceiverThread ReadFromStream Exception: " + ex.ToString());
                    _onReceiveFail.Call();
                    SetTerminated();
                }
            }
            else
            {
                Thread.Sleep(16);
            }
        }

        protected void ScanPackets()
        {
            while (_recvBufferOffset > PackageHeaderSize && !CheckTerminated())
            {
                ushort pkgSize = 0;
                if (BitConverter.IsLittleEndian)
                {
                    pkgSize = BitConverter.ToUInt16(new byte[] { _recvBuffer[1], _recvBuffer[0] }, 0);
                }
                else
                {
                    pkgSize = BitConverter.ToUInt16(new byte[] { _recvBuffer[0], _recvBuffer[1] }, 0);
                }
                if (pkgSize <= _recvBufferOffset)
                {
                    byte[] _buffer = new byte[pkgSize - PackageHeaderSize];
                    Array.Copy(_recvBuffer, PackageHeaderSize, _buffer, 0, _buffer.Length);

                    _onReceivePackage.Call(_buffer);

                    if (_recvBufferOffset > pkgSize)
                    {
                        for (int i = pkgSize, j = 0; i < _recvBufferOffset; i++, j++)
                        {
                            _recvBuffer[j] = _recvBuffer[i];
                        }
                        _recvBufferOffset -= pkgSize;
                    }
                    else
                    {
                        _recvBufferOffset = 0;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    class SenderThread : BaseThread
    {
        const int MaxPacketSize = 1024 * 512;
        const short PackageHeaderSize = 2;
        List<byte[]> _packetsToSend = null;

        NetworkStream _netStream;

        public SenderThread(NetworkStream stream, List<byte[]> packetsToSend) : base()
        {
            _netStream = stream;
            _packetsToSend = packetsToSend;
        }

        protected override void Main()
        {
            Log.Debug("SenderThread.Main: Begin");

            while (!CheckTerminated())
            {
                bool sleep = false;
                byte[] buffer = null;

                lock (_packetsToSend)
                {
                    if (_packetsToSend.Count > 0)
                    {
                        buffer = _packetsToSend[0];
                    }
                    else
                    {
                        sleep = true;
                    }
                }

                if (buffer != null)
                {
                    byte[] len = BitConverter.GetBytes((ushort)(buffer.Length + PackageHeaderSize));
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(len);
                    }

                    byte[] _bufferWithLen = new byte[len.Length + buffer.Length];
                    len.CopyTo(_bufferWithLen, 0);
                    buffer.CopyTo(_bufferWithLen, len.Length);

                    try
                    {
                        _netStream.Write(_bufferWithLen, 0, (int)_bufferWithLen.Length);
                        _netStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Log.DebugFormat("SenderThead Write Exception: {0}, {1}, {2}", ex.Message, ex.StackTrace, ex.InnerException.Message);
                    }

                    lock (_packetsToSend)
                    {
                        if (_packetsToSend.Count > 0)
                        {
                            _packetsToSend.RemoveAt(0);
                        }
                    }
                }

                if (sleep)
                {
                    Thread.Sleep(15);
                }
            }

            Log.Debug("SenderThread.Main: End");
        }
    }
}
