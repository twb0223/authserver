using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Auth.CommService
{
    public class XMLPacket : Packet
    {
        #region 数据成员
        /// <summary>指令</summary>
        public CommandEnum CommType
        {
            get;
            set;
        }
        private string _playid = string.Empty;
        /// <summary>播放器ID</summary>
        public string PlayerID
        {
            get { return _playid; }
            set { _playid = value; }
        }
        private string _taskno = string.Empty;
        /// <summary>任务号</summary>
        public string TaskNO
        {
            get { return _taskno; }
            set { _taskno = value; }
        }
        private string _value = string.Empty;
        /// <summary>消息数据</summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Data
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion
        /// <summary>
        /// 将XMLPacket转换成XML字符串
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override string ToString()
        {
            //检查包是否正常
            CheckPacket();

            return string.Format(MainXML, this.CommType, this.PlayerID, this.TaskNO, this.Value,this.Data);

        }

        /// <summary>
        /// 将XMLPacket转换成XML字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            string xml = ToString();

            return Encode.GetBytes(xml);
        }
        /// <summary>
        /// 将byte数组转换成XMLPacket
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XMLPacket Parse(byte[] value)
        {
            string xml = ArrayToString(value);
            return Parse(xml);
        }
        /// <summary>
        /// 将byte数组转换成XMLPacket
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static XMLPacket Parse(byte[] value, int index, int count)
        {
            string xml = ArrayToString(value, index, count);
            return Parse(xml);
        }
        /// <summary>
        /// 将字符串转换成XMLPacket
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XMLPacket Parse(string value)
        {
            XMLPacket packet = new XMLPacket();
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(value);
                XmlNode node = xd.SelectSingleNode("Command");
                //指令类型
                string commType = node.Attributes["CommType"].Value;
                packet.CommType = EnumConvert.Parse<CommandEnum>(commType);
                //指令类型
                packet.PlayerID = node.Attributes["PlayerID"].Value;
                packet.TaskNO = node.Attributes["TaskNO"].Value;
                packet.Value = node.SelectSingleNode("Value").InnerText;
                //   packet.PlaylistStr = node.SelectSingleNode("Data").InnerXml;
            }
            catch (Exception)
            {
                //ExceptionLog.MLog(ex);
                packet = null;
            }
            return packet;
        }

        /// <summary>主XML格式</summary>
        private static string MainXML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.Append("<Command  CommType=\"{0}\" PlayerID=\"{1}\" TaskNO=\"{2}\">");
                sb.Append("<Value>{3}</Value>");
                sb.Append("<Data>{4}</Data>");
                sb.Append("</Command>");
                return sb.ToString();
            }
        }

        /// <summary>检查包是否正常</summary>
        protected override void CheckPacket()
        {
            //PlayerID
            if (string.IsNullOrEmpty(this.PlayerID))
            {
                throw new Exception("PlayerID不能为空");
            }
            //其它后面再补充
        }
    }

    #region XML数据包基类
    /// <summary>XML数据包基类</summary>
    public class Packet
    {
        /// <summary>字符串转byte[]编码</summary>
        protected static Encoding Encode = Encoding.GetEncoding("gb2312");

        /// <summary>检查包是否正常</summary>
        protected virtual void CheckPacket()
        {

        }

        /// <summary>
        /// 字符串转byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] StringToArray(string value)
        {
            return Encode.GetBytes(value);
        }
        /// <summary>
        /// byte[]转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ArrayToString(byte[] value)
        {
            return Encode.GetString(value);
        }
        /// <summary>
        /// byte[]转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string ArrayToString(byte[] value, int index, int count)
        {
            return Encode.GetString(value, index, count);
        }



        /// <summary>
        /// 转yyyy-MM-dd日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ConvertDate(string value)
        {
            return ConvertTime(value, "yyyy-MM-dd");
        }

        /// <summary>
        /// 转HH-mm-ss时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ConvertTime(string value)
        {
            return ConvertTime(value, "HH:mm:ss");
        }

        /// <summary>
        /// 转HH-mm-ss时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ConvertTime(string value, string format)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("zh-CHS");
            return DateTime.ParseExact(value, format, cultureInfo);
        }
    }
    #endregion
}
