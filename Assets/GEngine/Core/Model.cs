/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Model
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
    /// 管理所有的Proxy
    /// </summary>
    public class Model : IModel
    {
        private IDictionary<string, Proxy> m_proxyMap;
        protected readonly object m_syncRoot = new object();

        public Model()
        {
            m_proxyMap = new Dictionary<string, Proxy>();
            InitializeModel();
        }

        protected virtual void InitializeModel()
        {
        }


        public void RegisterProxy(Proxy proxy)
        {
            lock (m_syncRoot)
            {
                if (m_proxyMap.ContainsKey(proxy.ProxyName))
                    m_proxyMap[proxy.ProxyName] = proxy;
                else
                    m_proxyMap.Add(proxy.ProxyName, proxy);
            }
            proxy.OnRegister();
        }

        public T RetrieveProxy<T>(string proxyName) where T : Proxy
        {
            Proxy proxy = null;
            lock (m_syncRoot)
            {
                if (m_proxyMap.ContainsKey(proxyName))
                    proxy = m_proxyMap[proxyName];
            }
            return (T)proxy;
        }

        public T RemoveProxy<T>(string proxyName) where T : Proxy
        {
            Proxy proxy = null;
            lock (m_syncRoot)
            {
                if (m_proxyMap.TryGetValue(proxyName, out proxy))
                {
                    proxy.RemoveAllMessages();
                    m_proxyMap.Remove(proxyName);
                }
            }
            if(proxy != null)
                proxy.OnRemove();
            return (T)proxy;
        }

        public bool HasProxy(string proxyName)
        {
            lock (m_syncRoot)
            {
                return m_proxyMap.ContainsKey(proxyName);
            }
        }
    }
}