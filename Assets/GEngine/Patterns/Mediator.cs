/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Mediator
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Patterns
{
    /// <summary>
    /// 界面显示处理
    /// </summary>
    public class Mediator : KernalBase
    {
        public const string NAME = "Mediator";
        private string m_mediatorName = "";

        #region Constants
        public Mediator() : this("") { }
        public Mediator(string name)
        {
            this.m_mediatorName = string.IsNullOrEmpty(name) ? NAME : name;
        }
        #endregion


        #region Public
        /// <summary>
        /// 名称，由构造函数传入
        /// </summary>
        public string MediatorName { get { return m_mediatorName; } }
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