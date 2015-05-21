using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;

namespace Auth.CommService
{
    public class NewTcpServer
    {
        #region 公共变量
        IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
        TelnetServer tsr = null;
             /// <summary>端口</summary>
        int Port = 0;
        /// <summary>终端Socket列表</summary>
        static public Dictionary<string, TelnetSession> TcpDic = new Dictionary<string, TelnetSession>();
        /// <summary>终端最后心跳时间列表</summary>
        static public Dictionary<string, DateTime> HeartDic = new Dictionary<string, DateTime>();
        /// <summary>
        /// 当前状态
        /// </summary>
        static public Dictionary<string, string> HeartStatusDic = new Dictionary<string, string>();

                /// <summary>本地IP</summary>
        public IPAddress LocalIP = IPAddress.Any;

        DataService ds = new DataService();
        /// <summary>
        /// 初始化标识
        /// </summary>
        bool iniflag = false;
        #endregion

        #region 通讯初始化
        public NewTcpServer(int port, IPAddress localIP)
        {
            this.Port = port;
            this.LocalIP = localIP;
            iniflag = bootstrap.Initialize();
        }
        /// <summary>
        /// 开启supersocket通讯
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Start(ref string msg)
        {
            if (!iniflag)//初始化失败
            {
                bootstrap.Initialize();
            }
            try
            {
                var result = bootstrap.Start();
                if (result == StartResult.Failed)
                {
                    return false;
                }
                tsr = bootstrap.AppServers.FirstOrDefault() as TelnetServer;

                tsr.NewSessionConnected += new SessionHandler<TelnetSession>(appServer_NewSessionConnected);//注册有新终端连接事件
                tsr.SessionClosed += new SessionHandler<TelnetSession, SuperSocket.SocketBase.CloseReason>(appServer_SessionClosed);//注册会话结束事件
                tsr.NewRequestReceived += new RequestHandler<TelnetSession, StringRequestInfo>(appServer_NewRequestReceived);//注册消息接收事件

                CheckTerminalStatus();
                return true;
            }
            catch (Exception ex)
            {
                msg = "打开端口失败:" + ex.Message;
                tsr.Logger.Error(msg);
                return false;
            }
        }
        /// <summary>
        /// 关闭当前通信
        /// </summary>
        public void Close()
        {
            //停止服务器监听
            tsr.Stop();
            bootstrap.Stop();
            //更新所有终端在线状态为离线
            UpdateTerminalStatus(new List<string>(), true);

            lock (TcpDic)
            {
                try
                {
                    //关闭所有终端连接
                    foreach (KeyValuePair<string, TelnetSession> pair in TcpDic)
                    {
                        TelnetSession socket = pair.Value;
                        CloseSocket(socket);
                    }
                }
                catch { }
            }
        }
        void appServer_NewSessionConnected(TelnetSession session)
        {

        }
        void appServer_SessionClosed(TelnetSession session, SuperSocket.SocketBase.CloseReason reason)
        {

        }
        void appServer_NewRequestReceived(TelnetSession tsn, StringRequestInfo sri)
        {
            var str = sri.Key.Replace("\n", "") + "</Command>";
            XMLPacket packet = XMLPacket.Parse(str);
            try
            {
                if (packet == null)
                {
                   tsr.Logger.Debug("接收到非法数据包!" + str);
                }
                else
                {
                    switch (packet.CommType)
                    {
                        case CommandEnum.Heartbeat:
                            //保存点位和Socket对象
                            lock (TcpDic)
                            {
                                TcpDic[packet.PlayerID] = tsn;
                            }
                            //更新最后心跳时间
                            lock (HeartDic)
                            {
                                HeartDic[packet.PlayerID] = DateTime.Now;
                            }
                            //休眠状态
                            lock (HeartStatusDic)
                            {
                                HeartStatusDic[packet.PlayerID] = packet.Value;
                            }
                            packet.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            tsn.Send(packet.ToArray(), 0, packet.ToArray().Length);
                            break;
                        case CommandEnum.ACK_OK:
                        case CommandEnum.ACK_FAILED:
                            lock (dic)
                            {
                                if (dic.ContainsKey(packet.TaskNO))
                                {
                                    dicRet[packet.TaskNO] = packet;
                                    //成功信号量
                                    dic[packet.TaskNO].Set();
                                }
                            }
                            break;
                        default:
                            packet.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            tsn.Send(packet.ToArray(), 0, packet.ToArray().Length);
                            break;
                    }
                }
            }
            catch (SocketException)
            {
                //客户端退出
                TcpClientExit(packet.PlayerID, tsn);
                return;
            }
            catch (ObjectDisposedException)
            {
                //客户端退出
                TcpClientExit(packet.PlayerID, tsn);
                return;
            }
            catch (Exception ex)
            {
                tsr.Logger.Error("收到无法处理的数据包" + packet.ToString(),ex);
            }
        }

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
                                tsr.Logger.Warn("更新终端在线状态失败");

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
                        tsr.Logger.Error("通信异常！", ex);
                    }
                    checkStatusEvt.WaitOne(30000);
                }
            };
            Thread thread = new Thread(start);
            thread.Name = "CheckTerminal";
            thread.IsBackground = true;
            thread.Start();
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
                        tsr.Logger.Info("发送数据包成功:" + xmlpacket.ToString());
                    }
                }
                catch (Exception ex)
                {
                    returnMsg = "发送失败!";
                    tsr.Logger.Error("发送数据包异常:" + xmlpacket.ToString(), ex);
                }
            }
            else
            {
                returnMsg = "该终端不在线";
            }
            tsr.Logger.Info(returnMsg);
            return flag;
        }

        /// <summary>任务号</summary>
        int TaskNO = 0;
        /// <summary>任务对应的信号量</summary>
        Dictionary<string, ManualResetEvent> dic = new Dictionary<string, ManualResetEvent>();
        Dictionary<string, XMLPacket> dicRet = new Dictionary<string, XMLPacket>();
        private static object symObj = new object();

        private bool Send(TelnetSession socket, XMLPacket packet, ref XMLPacket retXmlPacket)
        {
            lock (symObj)
            {
                //任务号
                packet.TaskNO = (TaskNO++).ToString();
            }
            bool succ = false;
            dic[packet.TaskNO] = new ManualResetEvent(false);

            byte[] data = packet.ToArray();
            socket.Send(data, 0, data.Length);
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
                tsr.Logger.Info("点位：" + packet.PlayerID + " 任务号：" + packet.TaskNO + " 发送无应答");
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

                TcpDic[stationID].Send(data, 0, data.Length);

                succ = dic[task].WaitOne(5000);

                if (!succ)
                {
                    tsr.Logger.Info(stationID + task + "发送无应答");
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
                    tsr.Logger.Info(stationID + "发送数据包成功:" + xml);
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
            TelnetSession socket = null;
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
        private void TcpClientExit(string stationID, TelnetSession client)
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
                tsr.Logger.Error(stationID + "终端连接中断！", ex);
            }

            tsr.Logger.Info(stationID + "终端退出！");
        }

        /// <summary>
        /// 关闭SOCKET，并更新点位为不在线
        /// </summary>
        /// <param name="socket"></param>
        private void CloseSocket(TelnetSession socket, string stationID = "")
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
                tsr.Logger.Error("更状态异常", ex);
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
                        tsr.Logger.Info(stationID + "离线");
                    }
                }
                catch (Exception ex)
                {
                    tsr.Logger.Error("记录离线日志异常", ex);
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
                tsr.Logger.Error("时间同步异常！", ex);
            }
            return xmlpacket;
        }
        #endregion
    }
}
