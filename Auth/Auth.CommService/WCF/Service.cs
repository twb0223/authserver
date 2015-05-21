using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ServiceModel.Activation;

namespace Auth.CommService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service : IService
    {
        public static NewTcpServer TS;
        #region 认证服务器

        #region 服务器端调用方法
        //public bool SendXmlToClientBySid(string stationID, string msgtype, string xmlmsg)
        //{
        //    string retmsg = "";
        //    if (CheckComm())
        //    {
        //        XMLPacket msg = new XMLPacket();
        //        if (msgtype == "update")
        //        {
        //            msg.CommType = CommandEnum.UpGrade;
        //            msg.PlayerID = stationID;
        //            msg.Value = "";
        //        }
        //        return TS.SendXmlBySid(stationID, msg, ref retmsg);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 检查Socket通信是否正常
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //private bool CheckComm()
        //{
        //    if (TS == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        public bool SendUpdateMsg(string utid)
        {
            DealPacket dxp = new DealPacket();
            return dxp.ServerPushUpdate(int.Parse(utid), TS);
        }
        public bool Command(string sid, string flag) 
        {
            DealPacket dxp = new DealPacket();
            return dxp.DealRemote(sid, flag, TS);
        }

        #endregion
        #region 终端调用方法
        /// <summary>
        /// 终端传递参数
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public AuthMsgData SendXmlToServerBySid(AuthMsgData msg)
        {
            AuthMsgData returnmsg = null;
            DealPacket dxp = new DealPacket();
            dxp.DealAuthJson(msg, ref returnmsg);
            return returnmsg;
        }
        /// <summary>
        /// 升级请求
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>

        public UpdateMsgData SendUpdateJsonToServer(UpdateMsgData msg)
        {
            UpdateMsgData returnmsg = null;
            DealPacket dxp = new DealPacket();
            dxp.DealUpdate(msg, ref returnmsg);
            return returnmsg;
        }


        #endregion

        #endregion
        #region 播控平台
        public string SendXMLMsg(string utid, string xmldata)
        {
            string returnmsg="";
            TS.Send(utid,xmldata,ref returnmsg);
            return returnmsg;
        }

        #endregion
    }


}
