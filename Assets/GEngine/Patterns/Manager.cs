/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Manager
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using GEngine.Core;
using GEngine.Managers;

namespace GEngine.Patterns
{
    public abstract class Manager
    {
        /// <summary>
        /// 名称，必须由子类覆盖
        /// </summary>
        public const string NAME = "Manager";

        /// <summary>
        /// 刚被注册时调用
        /// </summary>
        public virtual void OnRegister() { }
        /// <summary>
        /// 移除时调用
        /// </summary>
        public virtual void OnRemove() { }




        #region Members

        private TimerManager m_timerManager;

        #endregion

        #region Accessors

        public TimerManager TimerManager
        {
            get
            {
                if (m_timerManager == null)
                    m_timerManager = Facade.Instance.RetrieveManager<TimerManager>();
                return m_timerManager;
            }
        }

        #endregion
    }


    public abstract class Manager<T> : Manager where T : Manager, new()
    {
        private static T m_instance;

        public static T Instance
        {
            get { return m_instance ?? (m_instance = Facade.Instance.RetrieveManager<T>()); }
        }
    }

}