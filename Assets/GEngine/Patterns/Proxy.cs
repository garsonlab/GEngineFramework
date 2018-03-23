/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Proxy
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Patterns
{
    /// <summary>
    /// 数据管理
    /// </summary>
    public class Proxy : KernalBase
    {
        public const string NAME = "Proxy";
        private string m_proxyName = "";

        #region Constructors
        public Proxy() : this("") { }
        public Proxy(string name)
        {
            m_proxyName = string.IsNullOrEmpty(name) ? NAME : name;
        }
        #endregion

        #region Public
        /// <summary>
        /// 名称，由构造函数传入
        /// </summary>
        public string ProxyName {get { return m_proxyName;}}
        /// <summary>
        /// 刚被注册时调用
        /// </summary>
        public virtual void OnRegister() { }
        /// <summary>
        /// 移除时调用
        /// </summary>
        public virtual void OnRemove() { }

        #endregion


    }
}