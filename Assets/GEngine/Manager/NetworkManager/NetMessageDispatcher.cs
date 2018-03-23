/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: NetMessage
 * Date    : 2018/03/08
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections.Generic;

namespace GEngine.Managers
{

    public class NetMessageHandler : IComparable<NetMessageHandler>
    {
        static ObjectPool<NetMessageHandler> m_pool = new ObjectPool<NetMessageHandler>(
            ()=>new NetMessageHandler(), 
            null, 
            m=>m.Reset(), 
            null);



        private Callback_1<object> m_callback;
        public int priority;
        public static NetMessageHandler New(Callback_1<object> callback, int priority)
        {
            NetMessageHandler handler = m_pool.Get();
            handler.m_callback = callback;
            handler.priority = priority;
            return handler;
        }

        public void Invoke(object parm)
        {
            if (m_callback != null)
                m_callback(parm);
        }

        public void Release()
        {
            m_pool.Release(this);
        }
        public int CompareTo(NetMessageHandler other)
        {
            return priority <= other.priority ? 1 : -1;
        }

        public bool EqualTo(Callback_1<object> other)
        {
            return other == m_callback;
        }
        public void Reset()
        {
            m_callback = null;
            priority = 0;
        }
    }

    public class NetMessageDispatcher 
    {
        static ObjectPool<NetMessageDispatcher> m_messages = new ObjectPool<NetMessageDispatcher>(
            ()=>new NetMessageDispatcher(), msg=>msg.Reset(), msg=>msg.Reset(), null);

        private int m_msgType;
        private List<NetMessageHandler> handles; 
        public static NetMessageDispatcher New(int msgType)
        {
            NetMessageDispatcher messageDispatcher = m_messages.Get();
            messageDispatcher.m_msgType = msgType;
            messageDispatcher.handles = new List<NetMessageHandler>();
            return messageDispatcher;
        }

        public int MsgType { get { return m_msgType; } }

        public void RegisterListener(Callback_1<object> callback, int priority)
        {
            lock (handles)
            {
                NetMessageHandler handler = NetMessageHandler.New(callback, priority);
                handles.Add(handler);
                handles.Sort();
            }
        }

        public void RemoveListener(Callback_1<object> callback)
        {
            int index = -1;
            lock (handles)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    if (handles[i].EqualTo(callback))
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    NetMessageHandler handler = handles[index];
                    handles.RemoveAt(index);
                    handler.Release();
                }
            }
        }

        public void Dispatchs(object data)
        {
            lock (handles)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    handles[i].Invoke(data);
                }
            }
        }

        public void Release()
        {
            m_messages.Release(this);
        }

        private void Reset()
        {
            m_msgType = 0;
            if(handles == null)
                handles = new List<NetMessageHandler>();
            foreach (var handler in handles)
            {
                handler.Release();
            }
            handles.Clear();
        } 

        
    }



}
