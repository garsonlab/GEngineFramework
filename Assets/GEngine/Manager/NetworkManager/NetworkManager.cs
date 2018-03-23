/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: NetworkManager
 * Date    : 2018/03/06
 * Version : v1.0
 * Describe: TODO：区分不同的频道
 *              消息解析
 *              消息加密
 */

using System.Collections.Generic;
using System.Net.Sockets;
using GEngine.Patterns;

namespace GEngine.Managers
{
    
    public class NetworkManager : Manager
    {
        public new const string NAME = "NetworkManager";
        public static NetworkManager Instance;
        private Dictionary<ChannelType, NetworkAgent> m_channelMap;
        private Dictionary<int, NetMessageDispatcher> m_listeners;

        public override void OnRegister()
        {
            Instance = this;
            m_channelMap = new Dictionary<ChannelType, NetworkAgent>();
            m_listeners = new Dictionary<int, NetMessageDispatcher>();
            TimerManager.RepeatedCall(-1, 0, 0, false, OnUpdate);
        }

        private void OnUpdate(object parm)
        {
            var enumerator = m_channelMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var channel = enumerator.Current.Value;
                if (!channel.IsConnected && channel.State == SocketState.Connected)
                {
                    channel.OnStateChanged(SocketState.BreakOff);
                    continue;
                }
                byte[] bytes;
                int msgType = 0;
                while (channel.PopMessage(ref msgType, out bytes))
                {
                    DispatchMessage(msgType, bytes);
                }
            }
            enumerator.Dispose();
        }

        public void Connect(ChannelType channelType, string ip, int port, Callback_1<SocketState> callback, bool isTcp)
        {
            NetworkAgent agent;
            if (!m_channelMap.TryGetValue(channelType, out agent))
            {
                if(isTcp)
                    agent = new NetworkAgent(channelType, SocketType.Stream, ProtocolType.Tcp);
                else
                    agent = new NetworkAgent(channelType, SocketType.Dgram, ProtocolType.Udp);
                m_channelMap.Add(channelType, agent);
            }
            agent.onStateChanged = callback;
            agent.Connect(ip, port);
        }

        public void Send(ChannelType channelType, byte[] bytes)
        {
            NetworkAgent agent;
            if (m_channelMap.TryGetValue(channelType, out agent))
            {
                //TODO 消息加密
                agent.Send(bytes);
            }
        }

        public void Close(ChannelType channelType)
        {
            NetworkAgent agent;
            if (m_channelMap.TryGetValue(channelType, out agent))
            {
                agent.Close();
                m_channelMap.Remove(channelType);
            }
        }

        public void RegisterListener(int msgType, Callback_1<object> callback, int priority = 0)
        {
            NetMessageDispatcher dispatcher;
            if (!m_listeners.TryGetValue(msgType, out dispatcher))
            {
                dispatcher = NetMessageDispatcher.New(msgType);
                m_listeners.Add(msgType, dispatcher);
            }
            dispatcher.RegisterListener(callback, priority);
        }

        public void RemoveListener(int msgType, Callback_1<object> callback)
        {
            NetMessageDispatcher dispatcher;
            if (m_listeners.TryGetValue(msgType, out dispatcher))
            {
                dispatcher.RemoveListener(callback);
            }
        }

        public void RemoveListeners(int msgType)
        {
            NetMessageDispatcher dispatcher;
            if (m_listeners.TryGetValue(msgType, out dispatcher))
            {
                dispatcher.Release();
                m_listeners.Remove(msgType);
            }
        }

        private void DispatchMessage(int msgType, byte[] bytes)
        {
            NetMessageDispatcher dispatcher;
            if (m_listeners.TryGetValue(msgType, out dispatcher))
            {
                //ToDO 消息解密
                dispatcher.Dispatchs(bytes);
            }
        }
    }
}