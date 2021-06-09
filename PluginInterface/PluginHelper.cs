using System;
using System.Runtime.InteropServices;
using PublicInfos;

namespace PluginInterface
{
    public static class PluginHelper
    {
        public enum LogLevel
        {
            Debug = 0,
            Info = 10,
            Warning = 20,
            Error = 30
        }
        public static void WriteLog(LogLevel level, string type, string msg)
        {
            if (addLog == null)
            {
                IntPtr apiAddr = GetModuleHandle("cqp.dll");
                IntPtr api = GetProcAddress(apiAddr, "cq_addlog");
                addLog = (CQ_addLog)Marshal.GetDelegateForFunctionPointer(api, addLog.GetType());
            }
            addLog(MainSave.AuthCode, (int)level, Marshal.StringToHGlobalAnsi(type), Marshal.StringToHGlobalAnsi(msg));
        }
        public static void SendGroupMessageTmp(long GroupID, string msg)
        {
            if (sendGroupMsg == null)
            {
                IntPtr apiAddr = GetModuleHandle("cqp.dll");
                IntPtr api = GetProcAddress(apiAddr, "CQ_sendGroupMsg");
                sendGroupMsg = (CQ_sendGroupMsg)Marshal.GetDelegateForFunctionPointer(api, sendGroupMsg.GetType());
            }
            sendGroupMsg(MainSave.AuthCode, GroupID, Marshal.StringToHGlobalAnsi(msg));
        }
        public static void SengPrivateMessageTmp(long QQID, string msg)
        {
            if (sendPrivateMsg == null)
            {
                IntPtr apiAddr = GetModuleHandle("cqp.dll");
                IntPtr api = GetProcAddress(apiAddr, "CQ_sendPrivateMsg");
                sendPrivateMsg = (CQ_sendPrivateMsg)Marshal.GetDelegateForFunctionPointer(api, sendPrivateMsg.GetType());
            }
            sendPrivateMsg(MainSave.AuthCode, QQID, Marshal.StringToHGlobalAnsi(msg));
        }
        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, string funcName);
        private delegate int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg);
        private static CQ_addLog addLog;
        private delegate int CQ_sendGroupMsg(int authCode, long qqId, IntPtr msg);
        private static CQ_sendGroupMsg sendGroupMsg;
        private delegate int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg);
        private static CQ_sendPrivateMsg sendPrivateMsg;
    }
}
