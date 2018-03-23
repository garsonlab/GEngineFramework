/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: GManager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections.Generic;
using GEngine.Interface;
using GEngine.Patterns;

namespace GEngine.Core
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


        public void RegisterManager<T>(T manager) where T : Manager
        {
            lock (m_syncRoot)
            {
                var type = typeof (T).ToString();
                if (m_managerMap.ContainsKey(type))
                    m_managerMap[type] = manager;
                else
                    m_managerMap.Add(type, manager);
            }
            manager.OnRegister();
        }

        public T RetrieveManager<T>() where T : Manager
        {
            Manager manager = null;
            lock (m_syncRoot)
            {
                if (m_managerMap.ContainsKey(typeof(T).ToString()))
                    manager = m_managerMap[typeof(T).ToString()];
            }
            return (T)manager;
        }

        public T RemoveManager<T>() where T : Manager
        {
            Manager manager = null;
            lock (m_syncRoot)
            {
                if (m_managerMap.TryGetValue(typeof(T).ToString(), out manager))
                {
                    m_managerMap.Remove(typeof(T).ToString());
                }
            }
            if(manager != null)
                manager.OnRemove();
            return (T)manager;
        }
    }
}