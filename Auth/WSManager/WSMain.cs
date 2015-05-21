using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.ServiceProcess;
using System.Threading;
using System.Collections;
using System.Configuration.Install;
using System.Net;
using System.Net.Sockets;

namespace WSManager
{
    public partial class WSMain : Form
    {
        public WSMain()
        {
            InitializeComponent();
        }
        private string configFileName = "Auth.CommService.exe.config";
        private static string wcfurl = string.Empty;
        /// <summary>
        /// 检测数据库连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckConn_Click(object sender, EventArgs e)
        {
            if (ConnectionTest(this.txtAuthConstr.Text))
            {
                MessageBox.Show("认证数据库连接正常");
            }
            else
            {
                MessageBox.Show("认证数据库连接异常");
            }
            if (ConnectionTest(this.txtpl.Text))
            {
                MessageBox.Show("平台数据库连接正常");
            }
            else
            {
                MessageBox.Show("平台数据库连接异常");
            }
        }
        public static bool ConnectionTest(string conn)
        {
            bool IsCanConnectioned = false;
            //获取数据库连接字符串 
            //创建连接对象
            SqlConnection connection = new SqlConnection(conn);
            try
            {
                //Open DataBase
                //打开数据库
                connection.Open();
                IsCanConnectioned = true;
            }
            catch
            {
                //Can not Open DataBase
                //打开不成功 则连接不成功 
            }
            finally
            {

                //关闭数据库连接
                connection.Close();
            }
            return IsCanConnectioned;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtAuthConstr.Text))
            {
                MessageBox.Show("认证数据库连接字符串不能为空！");
                this.txtAuthConstr.Focus();
                return;
            }
            if (string.IsNullOrEmpty(this.txtpl.Text))
            {
                MessageBox.Show("平台数据库连接字符串不能为空！");
                this.txtpl.Focus();
                return;
            }
            if (string.IsNullOrEmpty(this.txtip.Text))
            {
                MessageBox.Show("IP地址不能为空！");
                this.txtip.Focus();
                return;
            }
            if (string.IsNullOrEmpty(this.txtwcfport.Text))
            {
                MessageBox.Show("WCF端口不能为空！");
                this.txtwcfport.Focus();
                return;
            }
            if (string.IsNullOrEmpty(this.txtsocketport.Text))
            {
                MessageBox.Show("Socket端口不能为空！");
                this.txtsocketport.Focus();
                return;
            }

            if (string.IsNullOrEmpty(this.txtftppath.Text))
            {
                MessageBox.Show("终端日志路径不能为空！");
                this.txtftppath.Focus();
                return;
            }

            SaveConfig(this.txtAuthConstr.Text, "AuthCS");
            SaveConfig(this.txtpl.Text, "BaseDataCS");
            SaveConfig(this.txtip.Text, "localIP");
            SaveConfig(this.txtwcfport.Text, "wcfPort");
            SaveConfig(this.txtsocketport.Text, "localPort");

            SaveSuperSocketConfig();

            SaveConfig("http://" + this.txtip.Text + ":{0}/Auth.CommService/Service", "wcfAddress");
            MessageBox.Show("保存成功！");
        }

        #region 服务管理

        private string serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Auth.CommService.exe");
        private string serverName = "DMSCommService";    //默认服务名称
        /// <summary>
        /// 安装
        /// </summary>
        private void btnInstall_Click(object sender, EventArgs e)
        {
            serverName = txtServerName.Text.Trim();
            try
            {
                //保存配置文件
                if (!OpSaveConfig())
                {
                    MessageBox.Show("数据库连接失败,无法执行操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Enabled = false;
                //安装
                ServiceManager.InstallService(serverPath, serverName);
                Thread.Sleep(1000);


                //启动
                ServiceController myController = new ServiceController(serverName);
                ServiceControllerStatus status = myController.Status;
                if (status != ServiceControllerStatus.Running && status != ServiceControllerStatus.StartPending)
                {
                    myController.Start();
                    Thread.Sleep(2000);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                this.Enabled = true;
            }
            btnRefresh_Click(null, null);
        }
        private void SaveSuperSocketConfig()
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            //string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            string strFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);

            doc.Load(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("server");

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Attributes["port"] != null)
                {
                    nodes[i].Attributes["port"].Value = this.txtsocketport.Text;
                }
            }
            doc.Save(strFileName);
        }

        private void btnUnInstall_Click(object sender, EventArgs e)
        {
            serverName = txtServerName.Text.Trim();
            try
            {
                this.Enabled = false;
                ServiceManager.UnInstallService(serverPath, serverName);
                Thread.Sleep(1000);

            }
            catch (Exception)
            {
            }
            finally
            {
                this.Enabled = true;
            }
            btnRefresh_Click(null, null);
        }

        private bool OpSaveConfig()
        {
            btnOK_Click(null, null);
            return true;
        }

        /// <summary>
        /// 重启
        /// </summary>
        private void btnReboot_Click(object sender, EventArgs e)
        {
            serverName = txtServerName.Text.Trim();
            try
            {
                //保存配置文件
                if (!OpSaveConfig())
                {
                    MessageBox.Show("数据库连接失败,无法执行操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Enabled = false;
                if (ServiceManager.ISWindowsServiceInstalled(serverName))
                {
                    ServiceController myController = new ServiceController(serverName);
                    ServiceControllerStatus status = myController.Status;
                    if (status != ServiceControllerStatus.Stopped && status != ServiceControllerStatus.StopPending)
                    {
                        myController.Stop();
                        Thread.Sleep(2000);
                    }
                    myController.Start();
                    Thread.Sleep(2000);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                this.Enabled = true;
            }
            btnRefresh_Click(null, null);
        }
        /// <summary>
        /// 停止
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                if (ServiceManager.ISWindowsServiceInstalled(serverName))
                {
                    ServiceController myController = new ServiceController(serverName);
                    ServiceControllerStatus status = myController.Status;
                    if (status != ServiceControllerStatus.Stopped && status != ServiceControllerStatus.StopPending)
                    {
                        myController.Stop();
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                this.Enabled = true;
            }
            btnRefresh_Click(null, null);
        }
        /// <summary>
        /// 刷新
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            serverName = txtServerName.Text.Trim();
            try
            {
                if (!ServiceManager.ISWindowsServiceInstalled(serverName))
                {
                    cbStatus.SelectedIndex = 0;
                }
                else
                {
                    ServiceController myController = new ServiceController(serverName);
                    ServiceControllerStatus status = myController.Status;
                    if (status == ServiceControllerStatus.Running)
                    {
                        cbStatus.SelectedIndex = 1;
                    }
                    else cbStatus.SelectedIndex = 2;
                }
            }
            catch (Exception)
            {
            }
            #region 按钮设置
            if (cbStatus.SelectedIndex == 0)
            {
                btnInstall.Enabled = true;

                btnUnInstall.Enabled = false;
                btnReboot.Enabled = false;
                // btnStartup.Enabled = false;
                btnStop.Enabled = false;
            }
            else if (cbStatus.SelectedIndex == 1)
            {
                btnInstall.Enabled = false;

                btnUnInstall.Enabled = true;
                btnReboot.Enabled = true;
                // btnStartup.Enabled = false;
                btnStop.Enabled = true;
            }
            else if (cbStatus.SelectedIndex == 2)
            {
                btnInstall.Enabled = false;

                btnUnInstall.Enabled = true;
                btnReboot.Enabled = true;
                // btnStartup.Enabled = true;
                btnStop.Enabled = false;
            }
            #endregion

        }

        private void txtServerName_TextChanged(object sender, EventArgs e)
        {
            btnRefresh_Click(null, null);
        }

        #endregion

        #region  方法保存修改的设置
        /// <summary>
        /// 方法保存修改的设置
        /// </summary>
        /// <param name="ConnenctionString"></param>
        /// <param name="strKey"></param>
        private void SaveConfig(string ConnenctionString, string strKey)
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            //string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            string strFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);

            doc.Load(strFileName);
            //找出名称为"add"的所有元素
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                if (att != null && att.Value == strKey)
                {
                    //对目标元素中的第二个属性赋值
                    att = nodes[i].Attributes["value"];
                    att.Value = ConnenctionString;
                    break;
                }
                else
                {
                    if (nodes[i].Attributes["name"] != null && nodes[i].Attributes["name"].Value == strKey)
                    {
                        att = nodes[i].Attributes["connectionString"];
                        att.Value = ConnenctionString;
                    }
                }
            }
            //保存上面的修改
            doc.Save(strFileName);
        }

        /// <summary>
        /// 根据key获取value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string LoadAppSetting(string strKey)
        {
            string strValue = "";
            XmlDocument doc = new XmlDocument();

            string strFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);

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
                else
                {
                    if (nodes[i].Attributes["name"] != null && nodes[i].Attributes["name"].Value == strKey)
                    {
                        att = nodes[i].Attributes["connectionString"];
                        strValue = att.Value;
                        break;
                    }
                }
            }

            return strValue;
        }

        #endregion
        private string GetIP()
        {
            string hostName = Dns.GetHostName();//本机名   
            System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostName);//会返回所有地址，包括IPv4和IPv6  
            string ip = "";
            foreach (var item in addressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)//判断ip是否是ipv4
                {
                    ip = item.ToString();
                    break;
                }
            }
            return ip;
        }

        private void WSMain_Load(object sender, EventArgs e)
        {
            this.txtAuthConstr.Text = LoadAppSetting("AuthCS");
            this.txtpl.Text = LoadAppSetting("BaseDataCS");
            this.txtip.Text = LoadAppSetting("localIP") == "" ? GetIP() : LoadAppSetting("localIP");
            this.txtsocketport.Text = LoadAppSetting("localPort");
            this.txtwcfport.Text = LoadAppSetting("wcfPort");
            this.txtftppath.Text = LoadAppSetting("ClientLogPath");

            this.txtServerName.Text = serverName;
            this.lbl1.Text = "SQLServer身份认证：Data Source=服务器名称/IP;Initial Catalog=数据库名称;User Id=用户名;Password=密码;";
            this.lbl2.Text = "Windows身份认证  ：Data Source=服务器名称/IP;Initial Catalog=数据库名称;Integrated Security=True;";

            //服务管理
            btnRefresh_Click(null, null);
        }

        private void btnCheckConn_Click_1(object sender, EventArgs e)
        {
            WCFConnTest test = new WCFConnTest();
            test.Url = string.Format("http://{0}:{1}/Auth.CommService/Service", this.txtip.Text, this.txtwcfport.Text);
            test.Show();
        }

        private ServiceController getServiceByName(string strServiceName)
        {
            foreach (ServiceController sc in ServiceController.GetServices())
            {
                if (sc.ServiceName.ToLower().Trim() == strServiceName.ToLower().Trim())
                {
                    this.txtServerName.Text = sc.ServiceName;
                    this.cbStatus.Text = sc.Status.ToString();
                    return sc;
                }
            }
            return null;
        }
    }
}
