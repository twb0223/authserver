using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Auth.CommService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            string serverName = LoadConfig("serverName");
            if (!string.IsNullOrEmpty(serverName))
            {
                this.serviceInstaller1.ServiceName = serverName;
                this.serviceInstaller1.DisplayName = serverName;
            }
        }
        /// <summary>
        /// 根据key获取value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string LoadConfig(string strKey)
        {
            string strValue = "";
            XmlDocument doc = new XmlDocument();

            string configFileName = "Auth.CommService.exe.config";
            string root = System.Reflection.Assembly.GetExecutingAssembly().Location;
            root = root.Remove(root.LastIndexOf('\\') + 1);
            string strFileName = Path.Combine(root, configFileName);

            doc.Load(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                if (att != null && att.Value == strKey)
                {
                    //对目标元素中的第二个属性赋值
                    att = nodes[i].Attributes["value"];
                    strValue = att.Value;   //value值
                    break;
                }
            }
            return strValue;
        }
    }
}
