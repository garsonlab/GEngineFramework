/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: KernalBase
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

using System.Collections.Generic;
using GEngine.Core;
using GEngine.Interface;
using GEngine.Managers;
using UnityEngine;

namespace GEngine.Patterns
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

        public T RetrieveManager<T>() where T : Manager
        {
            return Facade.Instance.RetrieveManager<T>();
        }
        #endregion

        #region Members

        private static MessageManager m_messageManager;
        private static TimerManager m_timerManager;
        private static ResourceManager m_resourceManager;
        private static InputManager m_inputManager;
        private static UIManager m_uiManager;

        private StartCoroutineHandler m_startCoroutine;
        #endregion

        #region Accessors
        /// <summary>
        /// 消息管理器
        /// </summary>
        public MessageManager MessageManager
        {
            get
            {
                if (m_messageManager == null)
                    m_messageManager = RetrieveManager<MessageManager>();
                return m_messageManager;
            }
        }
        /// <summary>
        /// 计时器
        /// </summary>
        public TimerManager TimerManager
        {
            get
            {
                if (m_timerManager == null)
                    m_timerManager = RetrieveManager<TimerManager>();
                return m_timerManager;
            }
        }
        /// <summary>
        /// 资源管理器
        /// </summary>
        public ResourceManager ResourceManager
        {
            get
            {
                if (m_resourceManager == null)
                    m_resourceManager = RetrieveManager<ResourceManager>();
                return m_resourceManager;
            }
        }
        /// <summary>
        /// 操作管理器
        /// </summary>
        public InputManager InputManager
        {
            get
            {
                if (m_inputManager == null)
                    m_inputManager = RetrieveManager<InputManager>();
                return m_inputManager;
            }
        }

        /// <summary>
        /// UI管理器
        /// </summary>
        public UIManager UIManager
        {
            get
            {
                if (m_uiManager == null)
                    m_uiManager = RetrieveManager<UIManager>();
                return m_uiManager;
            }
        }

        /// <summary>
        /// 协程调用
        /// </summary>
        public StartCoroutineHandler StartCoroutine
        {
            get
            {
                if (m_startCoroutine == null)
                {
                    MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
                    if (mono == null)
                    {
                        GameObject obj = new GameObject("GManager");
                        mono = obj.AddComponent<MonoManager>();
                    }
                    m_startCoroutine = mono.StartCoroutine;
                }
                return m_startCoroutine;
            }
        }
        #endregion
    }
}
