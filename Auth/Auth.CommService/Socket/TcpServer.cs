
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using System.Reflection;
using log4net;

namespace Auth.CommService
{
    #region TCP服务器连接
    public class TcpServer
    {
        #region 成员
        /// <summary>端口</summary>
        int Port = 0;
        /// <summary>终端Socket列表</summary>
        static public Dictionary<string, Socket> TcpDic = new Dictionary<string, Socket>();
        /// <summary>终端最后心跳时间列表</summary>
        static public Dictionary<string, DateTime> HeartDic = new Dictionary<string, DateTime>();
        /// <summary>
        /// 当前状态
        /// </summary>
        static public Dictionary<string, string> HeartStatusDic = new Dictionary<string, string>();

        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        DataService ds = new DataService();

        /// <summary>播放器结构体</summary>
        struct PlayerEntity
        {
            ///// <summary>序号</summary>
            //public ulong Index;
            /// <summary>点位ID</summary>
            public string PlayerID;
            /// <summary>Socket对象</summary>
            public Socket PlayerSocket;
            /// <summary>数据缓冲区</summary>
            public byte[] data;
        }

        /// <summary>心跳状态修改</summary>
        //HandleData hdData = new HandleData();

        ///// <summary>注册时的排他锁</summary>
        //private static object objLock = new object();
        ///// <summary>点位与设备ID对应字典</summary>
        //static public Dictionary<string, string> StationEquDic = new Dictionary<string, string>();
        /// <summary>本地IP</summary>
        public IPAddress LocalIP = IPAddress.Any;
        #endregion

        #region 根据心跳更新在线状态
        AutoResetEvent checkStatusEvt = new AutoResetEvent(false);
        /// <summary>退出事件</summary>
        ManualResetEvent[] exitEvt = new ManualResetEvent[] { new ManualResetEvent(false), new ManualResetEvent(true) };

        /// <summary>
        /// 根据心跳更新在线状态
        /// </summary>
        private void CheckTerminalStatus()
        {
            ThreadStart start = delegate
            {
                while (true)
                {
                    try
                    {
                        if (WaitHandle.WaitAny(exitEvt) == 0) return;

                        //在线列表
                        List<string> listOnline = new List<string>();
                        //不在线列表
                        List<string> listOffline = new List<string>();

                        lock (HeartDic)
                        {
                            foreach (KeyValuePair<string, DateTime> pair in HeartDic)
                            {
                                //2分内无通信则不在线
                                if (DateTime.Now.Subtract(pair.Value).TotalSeconds > 120)
                                {
                                    listOffline.Add(pair.Key);
                                }
                                else
                                {
                                    listOnline.Add(pair.Key);

                                }
                            }
                        }
                        //需要更新在线状态
                        if (listOnline.Count > 0)
                        {
                            //更新在线状态
                            if (!UpdateTerminalStatus(listOnline))
                            {
                                log.Warn("更新终端在线状态失败");

                            }
                        }

                        //关闭不在线的点位
                        foreach (string stationID in listOffline)
                        {
                            CloseSocket(stationID);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLog.MLog(ex, "通信异常！");
                        log.Error("通信异常！", ex);
                    }
                    checkStatusEvt.WaitOne(30000);
                }
            };
            Thread thread = new Thread(start);
            thread.Name = "CheckTerminal";
            thread.Start();
        }

        #endregion

        #region 接收TCP数据
        public TcpServer(int port, IPAddress localIP)
        {
            this.Port = port;
            this.LocalIP = localIP;
        }

        /// <summary>本地TCP对象</summary>
        Socket tcpSocket;
        /// <summary>
        /// 启动服务器程序,开始监听客户端请求
        /// </summary>
        public bool Start(ref string msg)
        {
            try
            {
                //绑定到本机端口
                EndPoint point = new IPEndPoint(this.LocalIP, Port);
                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //绑定到本机端口
                tcpSocket.Bind(point);
                //开始监听
                tcpSocket.Listen(100);
                //设置异步方法接受客户端连接

                tcpSocket.BeginAccept(new AsyncCallback(AcceptConn), null);

                //根据心跳更新在线状态
                CheckTerminalStatus();
               
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLog.MLog("打开端口失败:" + ex);
              
                return false;
            }
        }


        /// <summary>
        /// 客户端连接处理函数
        /// </summary>
        /// <param name="iar">欲建立服务器连接的Socket对象</param>
        private void AcceptConn(IAsyncResult iar)
        {
            Socket client = tcpSocket.EndAccept(iar);
            PlayerEntity player;
            player.PlayerID = "";
            player.PlayerSocket = client;
            player.data = new byte[10240];

            try
            {
                //开始接受来自该客户端的数据
                client.BeginReceive(player.data, 0, player.data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), player);
            }
            catch (Exception ex)
            {
                ExceptionLog.MLog("客户端连接异常!" + ex);
              
            }
            finally
            {
                //继续接受客户端连接
                tcpSocket.BeginAccept(new AsyncCallback(AcceptConn), null);
            }
        }
        /// <summary>
        /// 接受数据完成处理函数，异步的特性就体现在这个函数中，
        /// 收到数据后，会自动解析为字符串报文
        /// </summary>
        /// <param name="iar">目标客户端Socket</param>
        private void ReceiveData(IAsyncResult iar)
        {
            PlayerEntity clientinfo = (PlayerEntity)iar.AsyncState;
            Socket cs = clientinfo.PlayerSocket;
            string sid = clientinfo.PlayerID;
            string dataStr = "";
            try
            {
                //结束异步调用
                int recvlength = cs.EndReceive(iar);
                //正常的关闭
                if (recvlength == 0)
                {
                    //客户端退出
                    TcpClientExit(sid, cs);
                    return;
                }
                //处理粘包
                string sep = "</Command>";
                dataStr = XMLPacket.ArrayToString(clientinfo.data, 0, recvlength);
                string[] args = dataStr.Split(new string[] { sep }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var str in args)
                {
                    if (str.Trim() == "") continue;
                    //处理收到的数据包
                    string data = str.Trim() + sep;
                    //解析收到的数据
                    XMLPacket packet = XMLPacket.Parse(data);
                    //收到的数据非法
                    if (packet == null)
                    {
                        ExceptionLog.MLog("接收到非法数据包!" + data);
                    }
                    else
                    {
                        sid = packet.PlayerID;
                        switch (packet.CommType)
                        {
                            case CommandEnum.Heartbeat:
                                lock (TcpDic)
                                {
                                    TcpDic[sid] = cs;
                                }
                                lock (HeartDic)
                                {
                                    HeartDic[sid] = DateTime.Now;
                                }
                                lock (HeartStatusDic)
                                {
                                    HeartStatusDic[sid] = packet.Value;
                                }
                                packet.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                cs.Send(packet.ToArray());
                                //SendTcpData(cs, packet.ToArray());
                                
                                //log.Info("收到心跳包:" + sid);
                                break;
                            case CommandEnum.ACK_OK:
                            case CommandEnum.ACK_FAILED:
                                if (dic.ContainsKey(packet.TaskNO))
                                {
                                    dicRet[packet.TaskNO] = packet;
                                    //成功信号量
                                    dic[packet.TaskNO].Set();
                                }
                                break;
                            default:
                                packet.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                cs.Send(packet.ToArray());
                                //SendTcpData(cs, packet.ToArray());
                               // log.Info("收到点位" + sid + "数据包:" + data);
                                break;
                        }
                    }
                }
            }
            catch (SocketException)
            {
                TcpClientExit(sid, cs);
                return;
            }
            catch (ObjectDisposedException)
            {
                TcpClientExit(sid, cs);
                return;
            }
            try
            {
                //继续接收来自来客户端的数据
                cs.BeginReceive(clientinfo.data, 0, clientinfo.data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientinfo);
            }
            catch (Exception ex)
            {
                ExceptionLog.MLog(ex);
                //客户端退出
                TcpClientExit(sid, cs);
            }
        }

        /// <summary>
        /// 发送TCP数据
        /// </summary>
        /// <param name="client">客户端Socket</param>
        /// <param name="data">数据</param>
        private void SendTcpData(Socket client, byte[] data)
        {
            client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendDataEnd), client);
        }

        /// <summary>
        /// 发送数据完成处理函数
        /// </summary>
        /// <param name="iar">目标客户端Socket</param>
        private void SendDataEnd(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndSend(iar);
        }

        /// <summary>
        /// 关闭当前通信
        /// </summary>
        public void Close()
        {
            //关闭本地监听终节点
            CloseSocket(tcpSocket);
            //更新所有终端在线状态为离线
            UpdateTerminalStatus(new List<string>(), true);
            //通知线程退出
            exitEvt[0].Set();
            checkStatusEvt.Set();
            lock (TcpDic)
            {
                try
                {
                    //关闭所有终端连接
                    foreach (KeyValuePair<string, Socket> pair in TcpDic)
                    {
                        Socket socket = pair.Value;
                        CloseSocket(socket);
                    }
                }
                catch (Exception ex)
                {
                    //log.Error("关闭通信异常!", ex);
                    ExceptionLog.MLog("关闭通信异常!"+ex);
                }
            }
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送XML
        /// </summary>
        /// <param name="stationID">点位ID</param>
        /// <param name="xmlpacket">XMLPacket包</param>
        /// <returns></returns>
        public bool SendXmlBySid(string stationID, XMLPacket xmlpacket, ref string returnMsg)
        {
            bool flag = false;
            if (TcpDic.ContainsKey(stationID))
            {
                try
                {
                    XMLPacket reXmlpacket = new XMLPacket();
                    flag = Send(TcpDic[stationID], xmlpacket, ref reXmlpacket);
                    //发送成功
                    if (!flag)
                    {
                        returnMsg = "发送失败!";
                    }
                    else
                    {
                        if (reXmlpacket == null)
                        {
                            returnMsg = "发送失败!";
                            flag = false;
                        }
                        else
                        {
                            returnMsg = reXmlpacket.Value;
                            flag = (reXmlpacket.CommType == CommandEnum.ACK_OK);
                        }
                        //写入记录
                        log.Info("发送数据包成功:" + xmlpacket.ToString());
                    }
                }
                catch (Exception ex)
                {
                    returnMsg = "发送失败!";
                    log.Error("发送数据包异常:" + xmlpacket.ToString(), ex);
                }
            }
            else
            {
                returnMsg = "该终端不在线";
            }
            log.Info(returnMsg);
            return flag;
        }

        /// <summary>任务号</summary>
        int TaskNO = 0;
        /// <summary>任务对应的信号量</summary>
        Dictionary<string, ManualResetEvent> dic = new Dictionary<string, ManualResetEvent>();
        Dictionary<string, XMLPacket> dicRet = new Dictionary<string, XMLPacket>();
        private static object symObj = new object();

        private bool Send(Socket socket, XMLPacket packet, ref XMLPacket retXmlPacket)
        {
            lock (symObj)
            {
                //任务号
                packet.TaskNO = (TaskNO++).ToString();
            }
            bool succ = false;
            dic[packet.TaskNO] = new ManualResetEvent(false);

            byte[] data = packet.ToArray();
            int len = socket.Send(data);
            succ = dic[packet.TaskNO].WaitOne(10000);
            if (succ)
            {
                if (dicRet.ContainsKey(packet.TaskNO))
                {
                    retXmlPacket = dicRet[packet.TaskNO];
                }
            }
            else
            {
                log.Info("点位：" + packet.PlayerID + " 任务号：" + packet.TaskNO + " 发送无应答");
            }
            return succ;
        }

        public bool Send(string stationID, string xml, ref string msg)
        {
            bool succ = false;
            if (TcpDic.ContainsKey(stationID))
            {
                XmlDocument xd = new XmlDocument();
                try
                {
                    xd.LoadXml(xml);
                }
                catch
                {
                    msg = "发送的数据无法解析成XML";
                    return false;
                }

                string task = "";

                lock (symObj)
                {
                    //任务号
                    if (TaskNO >= int.MaxValue) TaskNO = 0;
                    task = (TaskNO++).ToString();
                }

                dic[task] = new ManualResetEvent(false);
                //直接修改XML中的任务号
                XmlNode node = xd.SelectSingleNode("Command");
                node.Attributes["TaskNO"].Value = task;

                byte[] data = Packet.StringToArray(xd.InnerXml);
                int len = TcpDic[stationID].Send(data);

                succ = dic[task].WaitOne(5000);

                if (!succ)
                {
                    log.Info(stationID + task + "发送无应答");
                }
                else
                {
                    XMLPacket retXmlPacket = null;

                    if (dicRet.ContainsKey(task))
                    {
                        retXmlPacket = dicRet[task];
                    }
                    if (retXmlPacket == null)
                    {
                        msg = "发送失败!";
                        succ = false;
                    }
                    else
                    {
                        msg = retXmlPacket.Value;
                        succ = (retXmlPacket.CommType == CommandEnum.ACK_OK);
                    }
                    //写入记录
                    log.Info(stationID + "发送数据包成功:" + xml);
                }
                dic.Remove(task);
                dicRet.Remove(task);
            }
            else
            {
                msg = "该终端不在线";
            }
            return succ;
        }
        #endregion

        #region 关闭与终端的连接
        /// <summary>
        /// 关闭与终端的连接
        /// </summary>
        /// <param name="stationID">终端点位</param>
        public void CloseSocket(string stationID)
        {
            Socket socket = null;
            try
            {
                if (TcpDic.ContainsKey(stationID))
                {
                    socket = TcpDic[stationID];
                }
            }
            catch { }

            TcpClientExit(stationID, socket);
        }

        /// <summary>
        /// TCP客户端退出
        /// </summary>
        /// <param name="sim">点位</param>
        /// <param name="client">客户端Socket</param>
        private void TcpClientExit(string stationID, Socket client)
        {
            try
            {
                //关闭SOCKET
                CloseSocket(client, stationID);

                lock (TcpDic)
                {
                    if (TcpDic.ContainsKey(stationID))
                    {
                        TcpDic.Remove(stationID);
                    }
                }
                lock (HeartDic)
                {
                    if (HeartDic.ContainsKey(stationID))
                    {
                        HeartDic.Remove(stationID);
                    }
                }
                lock (HeartStatusDic)
                {
                    if (HeartStatusDic.ContainsKey(stationID))
                    {
                        HeartStatusDic.Remove(stationID);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(stationID + "终端连接中断！", ex);
            }

            log.Info(stationID + "终端退出！");
        }

        /// <summary>
        /// 关闭SOCKET，并更新点位为不在线
        /// </summary>
        /// <param name="socket"></param>
        private void CloseSocket(Socket socket, string stationID = "")
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Close();
                }
            }
            catch { }

            //更新终端不在线
            if (!string.IsNullOrEmpty(stationID))
            {
                try
                {
                    ds.UpdateOnlineStatus(stationID, 0);
                }
                catch { }
            }
        }
        #endregion

        #region 更新终端在线状态
        /// <summary>
        /// 更新终端在线状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateTerminalStatus(List<string> listOnline, bool offLineAll = false)
        {
            Dictionary<string, List<string>> dicStatus = new Dictionary<string, List<string>>();

            //不是退出模式
            if (!offLineAll)
            {
                lock (HeartStatusDic)
                {
                    //根据在线状态分组
                    var group = from dic in HeartStatusDic
                                where listOnline.Contains(dic.Key)
                                group dic by dic.Value into g
                                select new { g.Key, g };
                    //获得各状态对应的点位列表
                    foreach (var a in group)
                    {
                        List<string> list = new List<string>();

                        foreach (var b in a.g)
                        {
                            list.Add(b.Key);
                        }
                        //获得在线状态和对应的点位
                        dicStatus.Add(a.Key, list);
                    }
                }
            }

            bool flag = false;
            try
            {
                flag = ds.BatUpdateOnlineStatus(dicStatus, offLineAll);

            }
            catch (Exception ex)
            {
                log.Error("更状态异常", ex);
            }
            //退出时在线终端添加离线日志
            if (offLineAll)
            {
                try
                {
                    List<string> list = TcpDic.Keys.ToList();
                    foreach (string stationID in list)
                    {
                        //离线待机日志
                        log.Info(stationID + "离线");
                    }
                }
                catch (Exception ex)
                {
                    log.Error("记录离线日志异常", ex);
                }
            }
            return flag;
        }
        #endregion

        #region 同步时间
        private XMLPacket SynchronizationTime(string stationID)
        {
            XMLPacket xmlpacket = new XMLPacket();
            try
            {
                xmlpacket.CommType = CommandEnum.SynchronizationTime;
                xmlpacket.PlayerID = stationID;
                xmlpacket.Value = DateTime.Now.ToString("yyyyMMdd.HHmmss");
            }
            catch (Exception ex)
            {
                log.Error("时间同步异常！", ex);
            }
            return xmlpacket;
        }
        #endregion
    }
    #endregion
}
