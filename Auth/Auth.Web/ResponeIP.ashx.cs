using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using Auth.Web;

namespace Auth.Web
{
    /// <summary>
    /// ResponeIP 的摘要说明
    /// </summary>
    public class ResponeIP : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            Utils fun = new Utils();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("WCF", fun.EncryptDES(ConfigurationManager.AppSettings["wcfBasicAddress"], "dms@51bsi.com"));
            dic.Add("SocketIP", fun.EncryptDES(ConfigurationManager.AppSettings["SocketIP"], "dms@51bsi.com"));
            dic.Add("SocketPort", fun.EncryptDES(ConfigurationManager.AppSettings["SocketPort"], "dms@51bsi.com"));

            dic.Add("LogFtp", fun.EncryptDES(ConfigurationManager.AppSettings["LogFtp"], "dms@51bsi.com"));
            dic.Add("LogFtpUser", fun.EncryptDES(ConfigurationManager.AppSettings["LogFtpUser"], "dms@51bsi.com"));
            dic.Add("LogFtpPwd", fun.EncryptDES(ConfigurationManager.AppSettings["LogFtpPwd"], "dms@51bsi.com"));

           // dic.Add("BroSocketIP", fun.EncryptDES(ConfigurationManager.AppSettings["LogFtpPwd"], "dms@51bsi.com"));

            dic.Add("BroSocketIP", ConfigurationManager.AppSettings["BroSocketIP"]);
            dic.Add("BroSocketPort", ConfigurationManager.AppSettings["BroSocketPort"]);
            dic.Add("PlayerWcf", ConfigurationManager.AppSettings["PlayerWcf"]);
            dic.Add("MediaStore", ConfigurationManager.AppSettings["MediaStore"]);
            var json = JsonConvert.SerializeObject(dic);
            context.Response.Write(json);
           //context.Response.Write(fun.EncryptDES(json, "dms@51bsi.com"));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}