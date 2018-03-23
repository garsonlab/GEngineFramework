/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: View
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;
using GEngine.Interface;
using GEngine.Patterns;

namespace GEngine.Core
{
    /// <summary>
    /// 管理所有的Mediator
    /// </summary>
    public class View : IView
    {
        private IDictionary<string, Mediator> m_mediatorMap;
        protected readonly object m_syncRoot = new object();

        public View()
        {
            m_mediatorMap = new Dictionary<string, Mediator>();
            InitializeView();
        }

        protected virtual void InitializeView() { }

        public void RegisterMediator(Mediator mediator)
        {
            lock (m_syncRoot)
            {
                if (m_mediatorMap.ContainsKey(mediator.MediatorName))
                    m_mediatorMap[mediator.MediatorName] = mediator;
                else
                    m_mediatorMap.Add(mediator.MediatorName, mediator);
            }
            mediator.OnRegister();
        }

        public T RetrieveMediator<T>(string mediatorName) where T : Mediator
        {
            Mediator mediator = null;
            lock (m_syncRoot)
            {
                if (m_mediatorMap.ContainsKey(mediatorName))
                    mediator = m_mediatorMap[mediatorName];
            }
            return (T)mediator;
        }

        public T RemoveMediator<T>(string mediatorName) where T : Mediator
        {
            Mediator mediator = null;
            lock (m_syncRoot)
            {
                if (m_mediatorMap.TryGetValue(mediatorName, out mediator))
                {
                    mediator.RemoveAllMessages();
                    m_mediatorMap.Remove(mediatorName);
                }
            }
            if (mediator != null)
                mediator.OnRemove();
            return (T)mediator;
        }

        public bool HasMediator(string mediatorName)
        {
            lock (m_syncRoot)
            {
                return m_mediatorMap.ContainsKey(mediatorName);
            }
        }
    }
}