using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Auth.Model;
using Auth.DataAccess;
using System.Xml.XPath;
using System.Reflection;
using log4net;
using System.Data;

namespace Auth.CommService
{
    public class DealPacket
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        DataService ds = new DataService();
        public DealPacket()
        {

        }

        #region 处理认证类报文
        /// <summary>
        /// 终端初始化发送特征信息
        /// </summary>
        /// <param name="doc"></param>

        public void DealAuthJson(AuthMsgData msg, ref AuthMsgData returnmsg)
        {
            returnmsg = msg;

            if (string.IsNullOrEmpty(msg.DeviceID))//通过设备id判断是否是首次接入
            {
                //检测信息是否完整
                if (string.IsNullOrEmpty(msg.ClientVersion) || string.IsNullOrEmpty(msg.CPU) || string.IsNullOrEmpty(msg.Memory) || (string.IsNullOrEmpty(msg.MAC) && string.IsNullOrEmpty(msg.WifiMAC)))
                {
                    returnmsg.result = "Error";
                    log.Warn("终端传递信息不完整");
                    return;
                }
                //todo：解密mac，wifimac 首次使用默认密钥加密的数据。
                string mac = EncryptAndDecrypt.DecryptDES(msg.MAC, "dms@51bsi.com");
                string wifimac = EncryptAndDecrypt.DecryptDES(msg.WifiMAC, "dms@51bsi.com");


                if (String.IsNullOrEmpty(wifimac))
                {
                    wifimac = "null";
                }
                if (String.IsNullOrEmpty(mac))
                {
                    mac = "null";
                }
                var BCM = ds.GetEntity(msg.CPU, msg.Memory, mac, wifimac);

          

                var ACM = ds.GetEntityForClientInfo(msg.CPU, msg.Memory, mac, wifimac);
             
                //判断平台是否有数据
                if (!string.IsNullOrEmpty(BCM.EquipmentID))
                {
                    //如果本地空，则表示首次接入需要生成认证记录
                    if (ACM == null)
                    {
                      
                        //添加本地认证记录
                        ClientInfo model = new ClientInfo();
                        model.ClientCpu = BCM.EquipmentCPU;
                        model.ClientMAC = BCM.EquipmentMac;
                        model.ClientMemory = BCM.EquipmentMemory;
                        model.ClientInitDate = DateTime.Now;
                        model.ClientWifiMAC = BCM.EquipmentWifiMac;
                        model.ClientVersion = msg.ClientVersion;//使用客户端版本号
                        model.ClientName = BCM.EquipmentName;
                        model.ClientType = BCM.OsName;
                        model.OnlineStatus = string.Empty;
                        model.UpdateStaus = "无";
                        model.ClientDeviceID = BCM.EquipmentID;
                        model.Point = BCM.StationID;
                        model.PointName = BCM.StationName;
                        model.ClientIP = BCM.EquipmentIP;
                        model.ClientPort = BCM.EquipmentPort;
                        model.ClientStatus = "验证通过";
                        model.ClientChangeFlag = BCM.ClientChangeFlag;//获取 数据平台变更标识
                        model.Project = BCM.Project;
                        model.Department = BCM.Department;
                        model.CommKeys = Guid.NewGuid().ToString();
                        ds.Insert(model);

                     

                        //构造返回信息
                        returnmsg.CommKeys = model.CommKeys;
                        returnmsg.Point = BCM.StationID.ToString();
                        returnmsg.ClientIP = BCM.EquipmentIP;
                        returnmsg.ClientPort = BCM.EquipmentPort;
                        returnmsg.DeviceID = EncryptAndDecrypt.EncryptDES(BCM.EquipmentID, returnmsg.CommKeys);
                        returnmsg.ClientChangeFlag = BCM.ClientChangeFlag;//变更标识
                        returnmsg.result = "OK";
                        log.Info("MAC:" + mac + "初始化成功！");
                       

                    }
                    else
                    {
                        //如果本地有记录，说明设备终端信息重置过，再次接入了，此时需要更认证记录数据，并返回相关信息。
                        ACM.ClientCpu = BCM.EquipmentCPU;
                        ACM.ClientMAC = BCM.EquipmentMac;
                        ACM.ClientMemory = BCM.EquipmentMemory;
                        ACM.ClientInitDate = DateTime.Now;
                        ACM.ClientWifiMAC = BCM.EquipmentWifiMac;
                        ACM.ClientVersion = msg.ClientVersion;//使用客户端版本号
                        ACM.ClientName = BCM.EquipmentName;
                        ACM.ClientType = BCM.OsName;
                        ACM.UpdateStaus = "无";
                        ACM.ClientDeviceID = BCM.EquipmentID;
                        ACM.Point = BCM.StationID;
                        ACM.PointName = BCM.StationName;
                        ACM.ClientIP = BCM.EquipmentIP;
                        ACM.ClientPort = BCM.EquipmentPort;
                        ACM.ClientStatus = "验证通过";
                        ACM.ClientChangeFlag = BCM.ClientChangeFlag;
                        ACM.Department = BCM.Department;
                        ACM.Project = BCM.Project;
                        if (string.IsNullOrEmpty(ACM.CommKeys))
                        {
                            ACM.CommKeys = Guid.NewGuid().ToString();
                        }
                        ds.UpdateClient(ACM);//同步本地信息

                        returnmsg.CommKeys = ACM.CommKeys;//将生成的密钥传给终端
                        returnmsg.Point = BCM.StationID.ToString();
                        returnmsg.ClientIP = BCM.EquipmentIP;
                        returnmsg.ClientPort = BCM.EquipmentPort;
                        returnmsg.DeviceID = EncryptAndDecrypt.EncryptDES(BCM.EquipmentID, returnmsg.CommKeys);
                        returnmsg.ClientChangeFlag = BCM.ClientChangeFlag;//变更标识
                        returnmsg.result = "OK";

                        log.Info("MAC:" + mac + "再次初始化成功！");


                    }
                }
                else
                {
                    //平台没有找到该设备记录，认证服务器也没有，则记录异常终端。
                    if (ACM == null)
                    {

                            ClientInfo model = new ClientInfo();
                            model.ClientCpu = msg.CPU;
                            model.ClientMAC = mac;
                            model.ClientMemory = msg.Memory;
                            model.ClientWifiMAC = wifimac;
                            model.ClientVersion = msg.ClientVersion;//使用客户端版本号
                            model.ClientName = string.Empty;
                            model.ClientType = string.Empty;
                            model.OnlineStatus = string.Empty;
                            model.UpdateStaus = "无";
                            model.ClientStatus = "异常";
                            model.ClientChangeFlag = string.Empty;//获取 数据平台变更标识
                            model.Project = string.Empty;
                            model.Department = string.Empty;
                            model.CommKeys = string.Empty;
                            ds.Insert(model);



                    }
                    else
                    {
                        //平台没有该设备记录，但是认证服务器有，说明平台将该设备删除了，此时将认证服务器记录设为异常
                        ACM.ClientStatus = "异常";
                        ds.UpdateClient(ACM);
                    }
                    returnmsg.result = "Error";
                    log.Warn("WifiMAC:" + msg.WifiMAC + ",Mac:" + msg.MAC + " 的设备在基础数据平台未找到！");
                }
            }
            else
            {
                
                if (string.IsNullOrEmpty(msg.ClientChangeFlag) || string.IsNullOrEmpty(msg.ClientVersion) || string.IsNullOrEmpty(msg.DeviceID))
                {
                    returnmsg.result = "Error";
                    log.Warn("终端传递信息不完整");
                    return;
                }
                //有设备id，说明是初始化过了的设备
                var ACM = ds.GetEntity(msg.DeviceID);//认证服务器数据
                var BCM = ds.GetBaseEntity(msg.DeviceID);//基础数据平台数据
                if (!string.IsNullOrEmpty(BCM.EquipmentID))
                {
                    if (ACM != null)
                    {
                        //场景：认证服务器有记录，平台也有记录，则判断变更标识
                        if (BCM.ClientChangeFlag == msg.ClientChangeFlag)
                        {
                            //如果变更标识相等，表明平台数据没有变化,在判断版本号，已终端发送的版本号为主。
                            if (ACM.ClientVersion != msg.ClientVersion)
                            {
                                ACM.ClientVersion = msg.ClientVersion;
                                ds.UpdateClient(ACM);
                            }
                            returnmsg.CommKeys = ACM.CommKeys;
                            returnmsg.result = "OK";
                            log.Info("终端验证通过！");
                        }
                        else
                        {
                            //场景：平台数据有变更，导致变更标识不一致了，需要同步认证服务器数据。

                            ACM.ClientCpu = BCM.EquipmentCPU;
                            ACM.ClientMAC = BCM.EquipmentMac;
                            ACM.ClientMemory = BCM.EquipmentMemory;
                            ACM.ClientInitDate = DateTime.Now;
                            ACM.ClientWifiMAC = BCM.EquipmentWifiMac;
                            ACM.ClientVersion = msg.ClientVersion;//使用客户端版本号
                            ACM.ClientName = BCM.EquipmentName;
                            ACM.ClientType = BCM.OsName;
                            ACM.UpdateStaus = "无";
                            ACM.ClientDeviceID = BCM.EquipmentID;
                            ACM.Point = BCM.StationID;
                            ACM.PointName = BCM.StationName;
                            ACM.ClientIP = BCM.EquipmentIP;
                            ACM.ClientPort = BCM.EquipmentPort;
                            ACM.ClientStatus = "验证通过";
                            ACM.ClientChangeFlag = BCM.ClientChangeFlag;
                            ACM.Department = BCM.Department;
                            ACM.Project = BCM.Project;
                            ds.UpdateClient(ACM);//同步本地信息

                            //返回改变后的信息给终端
                            returnmsg.CommKeys = ACM.CommKeys;
                            returnmsg.Point = BCM.StationID.ToString();
                            returnmsg.ClientIP = BCM.EquipmentIP;
                            returnmsg.ClientPort = BCM.EquipmentPort;
                            returnmsg.DeviceID = EncryptAndDecrypt.EncryptDES(BCM.EquipmentID, returnmsg.CommKeys);
                            returnmsg.ClientChangeFlag = BCM.ClientChangeFlag;//变更标识
                            returnmsg.result = "OK";



                            log.Info("平台数据变更,终端信息同步通过！");

                        }
                    }
                    else
                    {
                        // 场景：如果认证服务器没有该设备记录，未知原因导致，则添加新数据到认证服务器，
                        ClientInfo model = new ClientInfo();
                        model.ClientCpu = BCM.EquipmentCPU;
                        model.ClientMAC = BCM.EquipmentMac;
                        model.ClientMemory = BCM.EquipmentMemory;
                        model.ClientInitDate = DateTime.Now;
                        model.ClientWifiMAC = BCM.EquipmentWifiMac;
                        model.ClientVersion = msg.ClientVersion;//使用客户端版本号
                        model.ClientName = BCM.EquipmentName;
                        model.ClientType = BCM.OsName;
                        model.OnlineStatus = string.Empty;
                        model.UpdateStaus = "无";
                        model.ClientDeviceID = BCM.EquipmentID;
                        model.Point = BCM.StationID;
                        model.PointName = BCM.StationName;
                        model.ClientIP = BCM.EquipmentIP;
                        model.ClientPort = BCM.EquipmentPort;
                        model.ClientStatus = "验证通过";
                        model.ClientChangeFlag = BCM.ClientChangeFlag;//获取 数据平台变更标识
                        model.Project = BCM.Project;
                        model.Department = BCM.Department;
                        model.CommKeys = Guid.NewGuid().ToString();
                        ds.Insert(model);

                        returnmsg.CommKeys = model.CommKeys;//将生成的密钥传给终端
                        returnmsg.Point = BCM.StationID.ToString();
                        returnmsg.ClientIP = BCM.EquipmentIP;
                        returnmsg.ClientPort = BCM.EquipmentPort;
                        returnmsg.DeviceID = EncryptAndDecrypt.EncryptDES(BCM.EquipmentID, returnmsg.CommKeys);
                        returnmsg.ClientChangeFlag = BCM.ClientChangeFlag;//变更标识

                        returnmsg.result = "OK";

                        log.Info("终端信息同步通过！");
                    }
                }
                else
                {
                    //场景：如果平台没有找到该设备ID对应的终端。
                    if (ACM != null)
                    {
                        //如果认证服务器有该设备的记录，则将该设备状态改为异常
                        ACM.ClientStatus = "异常";
                        ds.UpdateClient(ACM);
                        returnmsg.result = "Error";
                    }
                    else
                    {
                        //如果认证服务也没有改数据,则将该设备加入异常

                        ClientInfo model = new ClientInfo();
                        model.ClientCpu = string.Empty; ;
                        model.ClientMAC = string.Empty; ;
                        model.ClientMemory = string.Empty;
                        model.ClientWifiMAC = string.Empty;                   
                        model.ClientName = string.Empty;
                        model.ClientType = string.Empty;
                        model.OnlineStatus = string.Empty;
                        model.UpdateStaus = "无";
                        model.ClientStatus = "异常";                  
                        model.Project = string.Empty;
                        model.Department = string.Empty;
                        model.CommKeys = string.Empty;


                        model.ClientVersion = msg.ClientVersion;//使用客户端版本号
                        model.ClientChangeFlag = msg.ClientChangeFlag;//获取 数据平台变更标识
                        model.ClientDeviceID = msg.DeviceID;

                        ds.Insert(model);

                        returnmsg.result = "Error";

                        log.Warn("WifiMAC:" + msg.WifiMAC + ",Mac:" + msg.MAC + " 的设备在基础数据平台未找到！");
                    }
                }
            }
        }
        #endregion
        #region 处理升级类报文
        /// <summary>
        /// 处理升级请求报文
        /// </summary>
        /// <param name="msg">终端请求报文</param>
        /// <param name="returnmsg">服务器返回报文</param>
        public void DealUpdate(UpdateMsgData msg, ref UpdateMsgData returnmsg)
        {
            returnmsg = msg;

            VersionInfo AVM = new VersionInfo();
            if (msg.CmdType == "Update")
            {
                if (string.IsNullOrEmpty(msg.NewClientVersion))//表明是终端自己检测升级
                {
                    AVM = ds.GetVersionEntity(int.Parse(msg.ClientType), msg.Type);//取该类型的终端的最新版本 

                   
                    // returnmsg.DelayUpdateTime = GlobalConfig.ReadAppSetting("DelayUpdateTime");
                }
                else//终端收到服务器推送后请求升级
                {
                    AVM = ds.GetVersionEntityByID(msg.NewClientVersion);//此时NewClientVersion 存储的是服务器给终端的对应的版本ID
                }
                if (AVM!=null)
                {
                    if (AVM.VersionNo != msg.ClientVersion)
                    {
                        var commkey = ds.GetCommKeyByPoint(msg.DeviceID);//获取设备id对应的通讯密钥
                        returnmsg.FileUrl = EncryptAndDecrypt.EncryptDES(AVM.FielPath, commkey);
                        //returnmsg.FileMD5 = EncryptAndDecrypt.EncryptDES(AVM.MD5,commkey);

                        //returnmsg.FileUrl = AVM.FielPath;
                        returnmsg.FileMD5 = AVM.MD5;
                        returnmsg.NewClientVersion = AVM.VersionNo;
                        returnmsg.result = "Yes";
                    }
                    else
                    {
                        returnmsg.result = "No";
                    }
                }
                else
                {
                    log.Info("终端[" + msg.DeviceID + "]没有检测到可用版本,请检查数据！");
                    returnmsg.result = "Error";
                }
            }

        }
        /// <summary>
        /// 服务器推送更新
        /// </summary>
        /// <param name="utid"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public bool ServerPushUpdate(int utid, NewTcpServer comm)
        {
            bool flag = true;
            string isupdatenow = "";
            string updatetime = "";
            string updateOrRollback = "";
            try
            {
                var dic = ds.GetUpDateStationsForTasKID(utid, ref isupdatenow, ref updatetime, ref updateOrRollback);
                XMLPacket pack = new XMLPacket();
                pack.CommType = CommandEnum.UpGrade;
                foreach (var item in dic)
                {
                    pack.PlayerID = item.Key;
                    string msg = "";
                    pack.TaskNO = "";
                    pack.Value = updateOrRollback + "|" + item.Value.VersionID.ToString() + "|" + item.Value.ClientTypeName + "|" + isupdatenow + "|" + updatetime;
                    flag = comm.SendXmlBySid(item.Key, pack, ref msg);
                }
            }
            catch (Exception ex)
            {
                log.Error("推送更新异常", ex);
                flag = false;
            }
            return flag;
        }

        #endregion
        #region 处理远程操作类报文
        public bool DealRemote(string sid, string flag, NewTcpServer comm)
        {
            XMLPacket pc = new XMLPacket();
            pc.PlayerID = sid;
            string str = "";
            bool result = true;
            switch (flag)
            {
                case "reboot":
                    pc.CommType = CommandEnum.Reboot;
                    result = comm.SendXmlBySid(sid, pc, ref str);
                    break;
                case "turndown":
                    pc.CommType = CommandEnum.TurnDown;
                    result = comm.SendXmlBySid(sid, pc, ref str);
                    break;
                case "sleep":
                    pc.CommType = CommandEnum.Sleep;
                    result = comm.SendXmlBySid(sid, pc, ref str);
                    break;
                case "wakeup":
                    pc.CommType = CommandEnum.Wakeup;
                    result = comm.SendXmlBySid(sid, pc, ref str);
                    break;
                case "upload":
                    pc.CommType = CommandEnum.UploadLog;
                    result = comm.SendXmlBySid(sid, pc, ref str);
                    break;
                case "shutdown":
                    pc.CommType = CommandEnum.Shutdown;
                    pc.Value = GlobalConfig.ReadAppSetting("ClientShutDowntime");
                    result = comm.SendXmlBySid(sid, pc, ref str);
                    break;
                default:
                    break;
            }
            return result;
        }
        #endregion

    }
}
