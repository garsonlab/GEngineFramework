/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: KernalBase
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

using System.Collections.Generic;
using Kernal.Core;
using Kernal.Interface;
using Kernal.Managers;

namespace Kernal.Patterns
{
    /// <summary>
    /// 框架基础类，用于消息通知及内容获取
    /// </summary>
    public class KernalBase : IDispatcher
    {
        private IList<MessageData> m_listeners = new List<MessageData>();

        #region SendMessage
        public void SendMessage(int messageType)
        {
            SendMessage(messageType, null, null);
        }

        public void SendMessage(int messageType, object body)
        {
            SendMessage(messageType, body, null);
        }

        public void SendMessage(int messageType, object body, string type)
        {
            MessageManager.Excute(messageType, new MessageArgs(messageType, body, type));
        }
        #endregion

        #region Register & Remove
        public void RegisterMessage(int messageType, MessageHandler handler, int priority = 0)
        {
            var data = MessageManager.RegisterMessage(messageType, handler, priority);
            m_listeners.Add(data);
        }

        public void RemoveMessage(int messageType, MessageHandler handler)
        {
            MessageManager.RemoveMessage(messageType, handler);
            int count = m_listeners.Count;
            for (int i = count-1; i >= 0; i--)
            {
                if(m_listeners[i].EqualTo(messageType, handler))
                    m_listeners.RemoveAt(i);
            }
        }

        public void RemoveMessages(int messageType)
        {
            MessageManager.RemoveMessages(messageType);
            int count = m_listeners.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (m_listeners[i].EqualTo(messageType))
                    m_listeners.RemoveAt(i);
            }
        }

        /// <summary>
        /// 在界面移除时仅有管理调用
        /// </summary>
        public void RemoveAllMessages()
        {
            int count = m_listeners.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var data = m_listeners[i];
                MessageManager.RemoveMessage(data.Type, data.Handler);
            }
            m_listeners.Clear();
        }
        #endregion

        #region Retrieve
        public T RetrieveCommand<T>(string commandName) where T : Command
        {
            return Facade.Instance.RetrieveCommand<T>(commandName);
        }

        public T RetrieveProxy<T>(string proxyName) where T : Proxy
        {
            return Facade.Instance.RetrieveProxy<T>(proxyName);
        }

        public T RetrieveMediator<T>(string mediatorName) where T : Mediator
        {
            return Facade.Instance.RetrieveMediator<T>(mediatorName);
        }

        public T RetrieveManager<T>(string managerName) where T : Manager
        {
            return Facade.Instance.RetrieveManager<T>(managerName);
        }
        #endregion

        #region Members

        private MessageManager m_messageManager;
        private TimerManager m_timerManager;

        #endregion

        #region Accessors
        public MessageManager MessageManager
        {
            get
            {
                if (m_messageManager == null)
                    m_messageManager = RetrieveManager<MessageManager>(MessageManager.NAME);
                return m_messageManager;
            }
        }
        public TimerManager TimerManager
        {
            get
            {
                if (m_timerManager == null)
                    m_timerManager = RetrieveManager<TimerManager>(TimerManager.NAME);
                return m_timerManager;
            }
        }
        #endregion
    }
}
