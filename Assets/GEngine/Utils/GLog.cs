/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: GLog
 * Date    : 2018/03/06
 * Version : v1.0
 * Describe: 
 */

using GEngine.Managers;
using UnityEngine;

namespace GEngine
{
    public enum LogMode
    {
        None = 0,
        All,
        Error
    }

    public class GLog
    {
        public static LogMode logMode = LogMode.All;
        public static string romoteIp = "127.0.0.1";
        public static int remotePort = 18916;
        private static JsonPrinter m_printer;

        public static void Start()
        {
            Application.logMessageReceived += CaptureLog;
            NetworkManager.Instance.Connect(ChannelType.Log, romoteIp, remotePort, OnStateChanged, false);
        }

        private static void OnStateChanged(SocketState state)
        {
            if(state == SocketState.BreakOff)
                NetworkManager.Instance.Connect(ChannelType.Log, romoteIp, remotePort, OnStateChanged, false);
        }

        public static void Stop()
        {
            Application.logMessageReceived -= CaptureLog;
            NetworkManager.Instance.Close(ChannelType.Log);
        }

        private static void CaptureLog(string condition, string stackTrace, LogType type)
        {
            JsonNode node = JsonNode.NewTable();
            node["c"] = JsonNode.NewString(condition);
            node["s"] = JsonNode.NewString(stackTrace);
            node["t"] = JsonNode.NewInt((int)type);
            if(m_printer == null)
                m_printer = new JsonPrinter();
            byte[] bytes = m_printer.Bytes(node);
            NetworkManager.Instance.Send(ChannelType.Log, bytes);
        }

        public static void I(object message)
        {
            I(message, null);
        }

        public static void I(object message, Object context)
        {
            if(logMode == LogMode.All)
                Debug.Log(message, context);
        }

        public static void W(object message)
        {
            W(message, null);
        }

        public static void W(object message, Object context)
        {
            if (logMode == LogMode.All)
                Debug.LogWarning(message, context);
        }

        public static void E(object message)
        {
            E(message, null);
        }

        public static void E(object message, Object context)
        {
            if (logMode != LogMode.None)
                Debug.LogError(message, context);
        }

    }
}
