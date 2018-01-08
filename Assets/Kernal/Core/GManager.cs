/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: GManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;
using Kernal.Interface;
using Kernal.Patterns;

namespace Kernal.Core
{
    /// <summary>
    /// 管理所有基础工具类
    /// </summary>
    public class GManager : IGManager
    {
        private IDictionary<string, Manager> m_managerMap;
        protected readonly object m_syncRoot = new object();

        public GManager()
        {
            m_managerMap = new Dictionary<string, Manager>();
        }


        public void RegisterManager<T>(string managerName, T manager) where T : Manager
        {
            lock (m_syncRoot)
            {
                if (m_managerMap.ContainsKey(managerName))
                    m_managerMap[managerName] = manager;
                else
                    m_managerMap.Add(managerName, manager);
            }
            manager.OnRegister();
        }

        public T RetrieveManager<T>(string managerName) where T : Manager
        {
            Manager manager = null;
            lock (m_syncRoot)
            {
                if (m_managerMap.ContainsKey(managerName))
                    manager = m_managerMap[managerName];
            }
            return (T)manager;
        }

        public T RemoveManager<T>(string managerName) where T : Manager
        {
            Manager manager = null;
            lock (m_syncRoot)
            {
                if (m_managerMap.TryGetValue(managerName, out manager))
                {
                    m_managerMap.Remove(managerName);
                }
            }
            if(manager != null)
                manager.OnRemove();
            return (T)manager;
        }
    }
}