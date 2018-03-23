/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: MessageData
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

using System;
using GEngine.Patterns;

namespace GEngine.Managers
{
    public class MessageData : IComparable<MessageData>
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 优先级，数值越大优先级越高
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 消息回调
        /// </summary>
        public MessageHandler Handler { get; set; }

        public void Invoke(MessageArgs args)
        {
            if (Handler != null)
                Handler(args);
        }

        public int CompareTo(MessageData other)
        {
            if (Priority > other.Priority)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public bool EqualTo(int type)
        {
            return type == Type;
        }

        public bool EqualTo(MessageHandler handler)
        {
            return handler == Handler;
        }

        public bool EqualTo(int type, MessageHandler hander)
        {
            return type == Type && hander == Handler;
        }
    }
}
