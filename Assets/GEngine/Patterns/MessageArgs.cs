/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: MessageArgs
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Patterns
{
    public class MessageArgs
    {
        #region Members
        private int m_type;
        private object m_body;
        private object m_sender;
        #endregion

        #region Constructors
        public MessageArgs(int type) : this(type, null, null) { }
        public MessageArgs(int type, object body) : this(type, body, null) { }
        public MessageArgs(int type, object body, object sender)
        {
            this.m_type = type;
            this.m_body = body;
            this.m_sender = sender;
        }
        #endregion

        #region Public
        /// <summary>
        /// 消息的类型
        /// </summary>
        public int Type { get { return m_type; }}
        /// <summary>
        /// 消息参数
        /// </summary>
        public object Body { get { return m_body; }}
        /// <summary>
        /// 发送者
        /// </summary>
        public object Sender { get { return m_sender; } }

        /// <summary>
        /// 转换body
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return (T) m_body;
        }
        /// <summary>
        /// 转换sender
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSender<T>()
        {
            return (T) m_sender;
        }

        public override string ToString()
        {
            string msg = "Message Type: " + m_type;
            msg += "\nBody:" + (Body ?? "null");
            msg += "\nSender:" + (Sender ?? "null");
            return msg;
        }
        #endregion
    }
}
