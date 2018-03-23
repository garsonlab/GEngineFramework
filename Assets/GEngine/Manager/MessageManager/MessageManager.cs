/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: MessageManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;
using GEngine.Patterns;

namespace GEngine.Managers
{
    public class MessageManager : Manager
    {
        private ObjectPool<MessageSet> m_set;
        private ObjectPool<MessageData> m_data;
        private Dictionary<int, MessageSet> m_messages;

        public new const string NAME = "MessageManager";

        public override void OnRegister()
        {
            m_set = new ObjectPool<MessageSet>(() => new MessageSet(), null, ResetSet, ResetSet);
            m_data = new ObjectPool<MessageData>(() => new MessageData(), null, ResetData, ResetData);
            m_messages = new Dictionary<int, MessageSet>();
        }

        /// <summary>
        /// 注册消息监听类型
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="handler">回调方法</param>
        /// <param name="priority">优先级，值越大优先级越高</param>
        public MessageData RegisterMessage(int messageType, MessageHandler handler, int priority = 0)
        {
            MessageSet set;
            if (!m_messages.TryGetValue(messageType, out set))
            {
                set = m_set.Get();
                m_messages.Add(messageType, set);
            }

            MessageData data = m_data.Get();
            data.Type = messageType;
            data.Handler = handler;
            data.Priority = priority;
            set.AddMessageHandler(data);
            return data;
        }

        /// <summary>
        /// 删除该监听类型下所有的该回调方法
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="handler">回调方法</param>
        public void RemoveMessage(int messageType, MessageHandler handler)
        {
            MessageSet set;
            if (m_messages.TryGetValue(messageType, out set))
            {
                set.RemoveMessageHandler(handler, m_data);
            }
        }

        /// <summary>
        /// 移除该监听下的所有方法
        /// </summary>
        /// <param name="messageType"></param>
        public void RemoveMessages(int messageType)
        {
            MessageSet set;
            if (m_messages.TryGetValue(messageType, out set))
            {
                m_set.Release(set);
                m_messages.Remove(messageType);
            }
        }

        /// <summary>
        /// 执行调用
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="args"></param>
        public void Excute(int messageType, MessageArgs args)
        {
            MessageSet set;
            if (m_messages.TryGetValue(messageType, out set))
            {
                set.Invoke(args);
            }
        }

        public override void OnRemove()
        {
            m_set.Clear();
            m_data.Clear();
            m_messages.Clear();
        }

        #region Private Methods
        private void ResetSet(MessageSet set)
        {
            set.RemoveAll(m_data);
        }

        private void ResetData(MessageData data)
        {
            data.Type = 0;
            data.Handler = null;
            data.Priority = 0;
        }
        #endregion
    }
}
