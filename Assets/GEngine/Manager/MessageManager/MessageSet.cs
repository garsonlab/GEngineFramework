/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: MessageSet
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;

namespace GEngine.Managers
{
    internal class MessageSet
    {
        private List<MessageData> m_set;
        private List<MessageData> m_removes; 

        public MessageSet()
        {
            m_set = new List<MessageData>();
            m_removes = new List<MessageData>();
        }

        public void AddMessageHandler(MessageData data)
        {
            m_set.Add(data);
            m_set.Sort();
        }

        public void RemoveMessageHandler(MessageHandler handler, ObjectPool<MessageData> m_data)
        {
            int count = m_set.Count;
            for (int i = 0; i < count; i++)
            {
                if(m_set[i].EqualTo(handler))
                    m_removes.Add(m_set[i]);
            }

            for (int i = 0; i < m_removes.Count; i++)
            {
                m_set.Remove(m_removes[i]);
                m_data.Release(m_removes[i]);
            }
            m_removes.Clear();
        }

        public void RemoveAll(ObjectPool<MessageData> m_data)
        {
            int count = m_set.Count;
            for (int i = 0; i < count; i++)
            {
                m_data.Release(m_set[i]);
            }
            m_set.Clear();
        }

        public void Invoke(Patterns.MessageArgs args)
        {
            int count = m_set.Count;
            for (int i = 0; i < count; i++)
            {
                m_set[i].Invoke(args);
            }
        }
    }
}