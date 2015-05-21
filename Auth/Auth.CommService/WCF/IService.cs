using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;

namespace Auth.CommService
{
    [ServiceContract]
    public interface IService
    {
        #region 认证服务器使用      
        #region 服务器端调用方法
        ///// <summary>
        ///// 服务器发送数据到指定终端点位
        ///// </summary>
        ///// <param name="sid">点位id</param>
        ///// <param name="msgtype">消息类型</param>
        ///// <param name="xmlmsg">报文</param>
        ///// <returns></returns>
        //[OperationContract]
        //[WebInvoke(UriTemplate = "SendXmlToClientBySid", BodyStyle=WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        //bool SendXmlToClientBySid(string sid, string msgtype, string xmlmsg);

        /// <summary>
        /// 发送立即升级指令
        /// </summary>
        /// <param name="utid">计划ID</param>
        /// <returns>返回结果</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "SendUpdateMsg/{utid}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        bool SendUpdateMsg(string utid);

        /// <summary>
        /// 发送远程指令
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Command/{sid}/{flag}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        bool Command(string sid,string flag);
        #endregion

        #region 终端调用方法
        /// <summary>
        /// 认证时终端调用
        /// </summary>
        /// <param name="xmlpacket">报文</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "SendXmlToServerBySid", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        AuthMsgData SendXmlToServerBySid(AuthMsgData msg);

        /// <summary>
        /// 升级时终端调用
        /// </summary>
        /// <param name="xmlpacket">报文</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "SendUpdateJsonToServer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        UpdateMsgData SendUpdateJsonToServer(UpdateMsgData msg);
        #endregion

        #endregion

        #region 播控平台
        [OperationContract]
        [WebInvoke(UriTemplate = "SendXMLMsg/{utid}/{xmldata}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string SendXMLMsg(string utid,string xmldata);
        #endregion
    }

    #region 数据传输类
    [DataContract]
    [Serializable]
    public class AuthMsgData
    {
        [DataMember]
        public string MsgType { get; set; }

        [DataMember]
        public string CmdType { get; set; }

        [DataMember]
        public string ClientVersion { get; set; }

        [DataMember]
        public string ClientIP { get; set; }


        [DataMember]
        public string ClientPort { get; set; } 

        [DataMember]
        public string CPU { get; set; }

        [DataMember]
        public string MAC { get; set; }

        [DataMember]
        public string WifiMAC { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public string Memory { get; set; }

        [DataMember]
        public string Point { get; set; }

        [DataMember]
        public string Depart { get; set; }

        [DataMember]
        public string Project { get; set; }

        [DataMember]
        public string ClientChangeFlag { get; set; }

        [DataMember]
        public string CommKeys { get; set; }

        [DataMember]
        public string result { get; set; }

    }

    [DataContract]
    [Serializable]
    public class UpdateMsgData
    {
        [DataMember]
        public string MsgType { get; set; }
        [DataMember]
        public string CmdType { get; set; }

        [DataMember]
        public string DeviceID { get; set; }

        [DataMember]
        public string PointID { get; set; }

        [DataMember]
        public string ClientVersion { get; set; }

        /// <summary>
        /// 服务器接收时，存储的是版本ID，返回数据时存储的是新的版本号
        /// </summary>
        [DataMember]
        public string NewClientVersion { get; set; } 

        [DataMember]
        public string FileUrl { get; set; }

        [DataMember]
        public string FileMD5 { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string ClientType { get; set; }


        [DataMember]
        public string DelayUpdateTime { get; set; }

        [DataMember]
        public string result { get; set; }

    }

    #endregion

}
