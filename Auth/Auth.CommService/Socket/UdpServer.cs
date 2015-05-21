using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using log4net;
using System.Reflection;

namespace Auth.CommService
{
    public class UdpServer
    {
        UdpClient udp = null;
        bool run = true;
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Close()
        {
            try
            {
                run = false;
                if (udp != null)
                {
                    udp.Close();
                }
            }
            catch { }
        }
        /// <summary>
        /// 接收udp数据
        /// </summary>
        /// <param name="udpPort"></param>
        /// <param name="localIP"></param>
        /// <param name="tcpPort"></param>
        public void ReceiveUDP(int udpPort, string localIP, int tcpPort)
        {
            ServerEntity info = new ServerEntity();
            info.ServerIP = localIP;
            info.ServerPort = tcpPort;
            string json = JSON.Stringify(info);
            byte[] dataServer = Encoding.ASCII.GetBytes(json);

            ThreadStart start = delegate
            {
                while (run)
                {
                    try
                    {
                        udp = new UdpClient(udpPort);
                        udp.EnableBroadcast = true;
                        log.Info("打开UPD端口" + udpPort + "成功!");
                        break;
                    }
                    catch(Exception ex)
                    {
                        log.Fatal("打开UPD端口" + udpPort + "失败!",ex);
                        Thread.Sleep(10000);
                    }
                }

                while (run)
                {
                    try
                    {
                        IPEndPoint point = new IPEndPoint(IPAddress.Any, 0);

                        if (udp.Available <= 0) continue;
                        if (udp.Client == null) return;

                        byte[] data = udp.Receive(ref point);

                        string str = Encoding.ASCII.GetString(data);

                        log.Info("UDP接收数据:" + str + "");
                 

                        ClientEntity client = JSON.Parse<ClientEntity>(str);

                        IPEndPoint pointClient = new IPEndPoint(point.Address, client.port);
                        UdpClient udpSend = new UdpClient();

                        udpSend.Send(dataServer, dataServer.Length, pointClient);
                    }
                    catch (Exception ex)
                    {
                        log.Error("UDP接收数据异常",ex);
                        Console.WriteLine(DateTime.Now.ToString() + ex.Message);
                    }
                }
            };
            Thread thread = new Thread(start);
            thread.IsBackground = true;
            thread.Start();
        }
    }


    /// <summary>
    /// 休眠唤醒实体
    /// </summary>
    public class ServerEntity
    {
        /// <summary>服务器IP</summary>
        public string ServerIP;
        /// <summary>服务器端口</summary>
        public int ServerPort;
    }


    /// <summary>
    /// 休眠唤醒实体
    /// </summary>
    public class ClientEntity
    {
        /// <summary>类型</summary>
        public string type;
        /// <summary>客户端键值</summary>
        public string key;
        /// <summary>客户端端口</summary>
        public int port;
    }

    /// <summary> 
    /// 解析JSON，仿Javascript风格 
    /// </summary> 
    public static class JSON
    {
        /// <summary>
        /// JSON转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Parse<T>(string jsonString)
        {
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 实体转JSON
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string Stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
