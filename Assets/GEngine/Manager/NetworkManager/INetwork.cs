/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: INetwork
 * Date    : 2018/03/07
 * Version : v1.0
 * Describe: 
 */

using System.Net.Sockets;

namespace GEngine.Managers
{
    public enum ChannelType
    {
        Auth, //认证服
        Zone, //主服
        Battle, //战斗服
        Span, //跨服
        Log,//log
    }

    public enum SocketState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 连接中
        /// </summary>
        Connecting,
        /// <summary>
        /// 已连接
        /// </summary>
        Connected,
        /// <summary>
        /// 连接失败/网络中断
        /// </summary>
        BreakOff,
        /// <summary>
        /// 断开
        /// </summary>
        Dispose
    }

    public interface INetwork
    {
        /// <summary>
        /// 当前连接通道
        /// </summary>
        ChannelType ChannelType { get; }
        /// <summary>
        /// Socket类型
        /// </summary>
        SocketType SocketType { get; }
        /// <summary>
        /// 连接类型
        /// </summary>
        ProtocolType ProtocolType { get; }
        /// <summary>
        /// 接收缓存大小
        /// </summary>
        uint BufferSize { get; set; }
        /// <summary>
        /// 超时
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 当前连接状态
        /// </summary>
        SocketState State { get; }
        /// <summary>
        /// 发送流量
        /// </summary>
        long SendLength { get; }
        /// <summary>
        /// 接收流量
        /// </summary>
        long ReceiveLength { get; }
        /// <summary>
        /// 网络用量
        /// </summary>
        long NetUsage { get; }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        void Connect(string ip, int port);
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="bytes"></param>
        void Send(byte[] bytes);
        /// <summary>
        /// 断开
        /// </summary>
        void Close();
        /// <summary>
        /// 状态变化
        /// </summary>
        /// <param name="state"></param>
        void OnStateChanged(SocketState state);
        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="msgBytes"></param>
        /// <returns></returns>
        bool PopMessage(ref int msgType, out byte[] msgBytes);
    }
}