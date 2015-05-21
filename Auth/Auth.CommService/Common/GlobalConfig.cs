using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Net;

namespace Auth.CommService
{
    public class GlobalConfig
    {
        ///// <summary>IIS地址</summary>
        //public static string IisPath = "";
        ///// <summary>资源服务器地址</summary>
        //public static string MaterialPath = "";
        /// <summary>与终端通信端口</summary>
        public static int Port = 0;
        /// <summary>与平台通信的WCF地址</summary>
        public static string WcfAddress = "";
        /// <summary>与平台通信的WCF端口</summary>
        public static int WcfPort = 0;
        /// <summary>线程退出通知</summary>
        public static ManualResetEvent[] QuitEvent = new ManualResetEvent[] { new ManualResetEvent(false), new ManualResetEvent(true) };
        /// <summary>本地IP</summary>
        public static IPAddress LocalIP = IPAddress.Any;
        /// <summary>udp端口，用于接收广播寻址消息</summary>
        public static int UdpPort = 0;

        public static string LogPath = "";
        /// <summary>本地IP</summary>
        public static string LocalIPStr = "";

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool ReadConfig(ref string msg)
        {
            //IisPath = ReadAppSetting("MediaStoreURI");
            //if (string.IsNullOrEmpty(IisPath))
            //{
            //    msg = "请配置正确的IIS地址";
            //    return false;
            //}

            //MaterialPath = ReadAppSetting("MaterialPlatformURL");
            //if (string.IsNullOrEmpty(MaterialPath))
            //{
            //    msg = "请配置资源服务器地址";
            //    return false;
            //}

            string port = ReadAppSetting("localPort");
            if (string.IsNullOrEmpty(port))
            {
                msg = "请配置正确的播放器通信端口";
                return false;
            }
            msg = "";
            bool succ = int.TryParse(port, out Port);
            if (!succ)
            {
                msg = "请配置正确的播放器通信端口";
                return false;
            }

            msg = "";
            port = ReadAppSetting("udpPort");
            succ = int.TryParse(port, out UdpPort);
            if (!succ)
            {
                msg = "请配置正确的UDP通信端口";
                return false;
            }

            //本地IP
            string ip = ReadAppSetting("localIP");
            if (string.IsNullOrEmpty(ip))
            {
                LocalIP = IPAddress.Any;
            }
            if (!IPAddress.TryParse(ip, out LocalIP))
            {
                LocalIP = IPAddress.Any;
            }
            LocalIPStr = GetLocalIP();
            if (string.IsNullOrEmpty(LocalIPStr))
            {
                msg = "获得本地IP失败";
                return false;
            }

            WcfAddress = ReadAppSetting("wcfAddress");
            if (string.IsNullOrEmpty(WcfAddress))
            {
                msg = "请配置正确的WCF地址";
                return false;
            }
            LogPath = ReadAppSetting("ClientLogPath");
            if (string.IsNullOrEmpty(LogPath))
            {
                msg = "请配置正确的终端日志文件地址";
                return false;
            }

            string wcfPortStr = ReadAppSetting("wcfPort");
            if (string.IsNullOrEmpty(wcfPortStr))
            {
                msg = "请配置正确的WCF端口";
                return false;
            }
            msg = "";
            succ = int.TryParse(wcfPortStr, out WcfPort);
            if (!succ)
            {
                msg = "请配置正确的WCF端口";
            }

            return succ;
        }
        /// <summary>
        /// 读取AppSetting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch
            {
                return null;
            }
        }
        private static string GetLocalIP()
        {
            string ip = "";
            try
            {
                if (LocalIP == IPAddress.Any)
                {
                    //从网卡获得
                    string strHostName = Dns.GetHostName();  //得本机主机名
                    IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
                    ip = ipEntry.AddressList[0].ToString(); //假设本地主机单网卡
                }
                else
                {
                    ip = LocalIP.ToString();
                }
            }
            catch { }
            return ip;
        }
    }
}
