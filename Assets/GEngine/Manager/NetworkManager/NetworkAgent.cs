/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: NetworkAgent
 * Date    : 2018/03/08
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GEngine.Managers
{
    /// <summary>
    /// TCP连接通道
    /// </summary>
    public class NetworkAgent : INetwork, IDisposable
    {
        #region Members
        private ChannelType m_channelType;
        private SocketType m_socketType;
        private ProtocolType m_protocolType;
        private SocketState m_state;
        private uint m_bufferSize = 1024*8;
        private byte[] m_buffer;
        private int m_Timeout;
        private long m_sendLength;
        private long m_receiveLength;

        private Socket m_socket;
        private Thread m_sendThread;
        private Thread m_receiveThread;
        private Queue<byte[]> m_sendBuffer;
        private ByteArray m_receiveBuffer;

        private bool m_sendOn;
        private bool m_receiveOn;
        #endregion

        #region Public Args

        public Callback_1<SocketState> onStateChanged; 
        #endregion

        public NetworkAgent(ChannelType channelType, SocketType socketType, ProtocolType protocolType)
        {
            m_channelType = channelType;
            m_socketType = socketType;
            m_protocolType = protocolType;
            m_buffer = new byte[m_bufferSize];
            m_state = SocketState.None;
            m_sendLength = 0;
            m_receiveLength = 0;

            m_sendBuffer = new Queue<byte[]>();
            m_receiveBuffer = new ByteArray();
        }


        #region Accessors
        public ChannelType ChannelType { get { return m_channelType; } }
        public SocketType SocketType { get { return m_socketType;} }
        public ProtocolType ProtocolType { get { return m_protocolType;} }
        public uint BufferSize { get { return m_bufferSize; } set { m_bufferSize = value; m_buffer = new byte[value];} }
        public int Timeout { get { return m_Timeout; } set { m_Timeout = value; } }
        public bool IsConnected { get { return m_socket != null && m_socket.Connected; } }
        public SocketState State { get { return m_state; } }
        public long SendLength { get { return m_sendLength; } }
        public long ReceiveLength { get { return m_receiveLength; } }
        public long NetUsage { get { return m_sendLength + m_receiveLength; } }
        #endregion

        public void Connect(string ip, int port)
        {
            if (IsConnected)
            {
                GLog.W("Connect Warrming:: Already Connected!");
                return;
            }
            OnStateChanged(SocketState.Connecting);

            IPAddress address = Dns.GetHostAddresses(ip)[0];
            IPEndPoint endPoint = new IPEndPoint(address, port);
            m_socket = new Socket(address.AddressFamily, m_socketType, m_protocolType)
            {
                NoDelay = true,
                SendTimeout = Timeout,
                ReceiveTimeout = Timeout
            };
            try
            {
                m_socket.BeginConnect(endPoint, BeginConnect, m_socket);
            }
            catch (Exception e)
            {
                GLog.E("Connect Error:: " + e);
            }

        }

        public void Send(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            AbortThread();
            if (m_socket != null)
            {
                try
                {
                    m_socket.Close();
                    m_socket = null;
                }
                catch (Exception e)
                {
                    GLog.E(e.ToString());
                }
            }

            lock (m_sendBuffer)
            {
                m_sendBuffer.Clear();
            }
            m_receiveBuffer.Clear();
        }

        public void OnStateChanged(SocketState state)
        {
            if (m_state != state)
            {
                m_state = state;
                if (onStateChanged != null)
                    onStateChanged(state);
            }
        }

        public bool PopMessage(ref int msgType, out byte[] msgBytes)
        {
            //TODO: 分析是否是一条消息
            msgBytes = null;
            return false;
        }

        #region Private Methods
        private void BeginConnect(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            if (socket != null && socket.Connected)
            {
                socket.EndConnect(ar);
                OnStateChanged(SocketState.Connected);
                StartThread();
            }
            else
            {
                OnStateChanged(SocketState.BreakOff);
                GLog.E("Socket Connect Failed.");
            }
        }

        private void StartThread()
        {
            if(m_sendThread == null)
                m_sendThread = new Thread(StartSend);
            m_sendOn = true;
            m_sendThread.Start();
            if(m_receiveThread == null)
                m_receiveThread = new Thread(StartReceive);
            m_receiveOn = true;
            m_receiveThread.Start();
        }

        private void StartSend(object obj)
        {
            while (m_sendOn)
            {
                if (m_state == SocketState.Connected && m_sendBuffer.Count > 0)
                {
                    byte[] bytes = null;
                    lock (m_sendBuffer)
                    {
                        bytes = m_sendBuffer.Dequeue();
                    }
                    try
                    {
                        m_sendLength += m_socket.Send(bytes);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.ToString());
                    }
                }
            }
        }

        private void StartReceive(object obj)
        {
            while (m_receiveOn)
            {
                if (m_state == SocketState.Connected && m_socket.Available > 0)
                {
                    int size = 0;
                    try
                    {
                        size = m_socket.Receive(m_buffer);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.ToString());
                    }
                    if (size > 0)
                    {
                        m_receiveLength += size;
                        m_receiveBuffer.Push(m_buffer, 0, size);
                    }
                }
            }
        }

        private void AbortThread()
        {
            m_sendOn = false;
            m_receiveOn = false;
            if(m_sendThread != null)
                m_sendThread.Abort();
            if(m_receiveThread != null)
                m_receiveThread.Abort();
        }

        #endregion

        public void Dispose()
        {
            Close();
        }
    }
}