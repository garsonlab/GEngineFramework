/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: GarbageManager
 * Date    : 2018/03/12
 * Version : v1.0
 * Describe: 
 */

using System.Collections.Generic;
using GEngine.Patterns;

namespace GEngine.Managers
{
    /// <summary>
    /// 垃圾回收器，自动调用，可取消手动调用
    /// </summary>
    public class GarbageManager : Manager<GarbageManager>
    {
        public new const string NAME = "GarbageManager";
        private List<Callback_0> m_collectors;

        public override void OnRegister()
        {
            m_collectors = new List<Callback_0>();
            TimerManager.IntervalCall(-1, 60f, Collect);
        }

        public void AddCollector(Callback_0 callback)
        {
            if(!m_collectors.Contains(callback))
                m_collectors.Add(callback);
        }

        public void RemoveCollertor(Callback_0 callback)
        {
            if (m_collectors.Contains(callback))
                m_collectors.Remove(callback);
        }

        public void Collect(object o)
        {
            foreach (var collector in m_collectors)
            {
                collector();
            }
        }
    }
}