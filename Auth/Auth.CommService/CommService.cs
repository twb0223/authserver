using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.CommService
{
    public partial class CommService : ServiceBase
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        ServiceHost host = null;
        DataService ds = new DataService();
        public CommService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            Start();

        }

        protected override void OnStop()
        {
            // StopTimer();
            StopLogTimer();
            //关闭WCF服务
            try
            {
                if (host != null)
                {
                    host.Close();
                    log.Info("WCF服务关闭");
                }
            }
            catch (Exception ex)
            {
                log.Error("尝试关闭WCF服务失败！", ex);
                host.Abort();
            }

            //关闭连接
            if (comm != null) comm.Close();
            comm = null;
            //if (udp != null) udp.Close();
            log.Info("Socket通讯关闭");
        }

        /// <summary>
        /// 开启Socket通讯和WCF通讯服务,定时任务
        /// </summary>
        private void Start()
        {
            if (!OpenComm()) return;
            try
            {
                string url = string.Format(GlobalConfig.WcfAddress, GlobalConfig.WcfPort);
                Uri baseAddress = new Uri(url);
                Service.TS = comm;
                host = new ServiceHost(typeof(Service), baseAddress);
                host.Open();
                log.Info("打开WCF服务成功！");
            }
            catch (Exception ex)
            {
                log.Fatal("打开WCF服务失败！", ex);
                ExceptionLog.MLog(ex, "打开WCF服务失败！");
                host.Abort();
                this.Stop();
            }
            OpenLogTimer();//开启日志导入扫描
        }

        #region 打开通信
        NewTcpServer comm = null;
         
       // UdpServer udp = null;
        /// <summary>
        /// 打开通信
        /// </summary>
        private bool OpenComm()
        {
            bool succ = false;
            try
            {
                //读配置文件
                string msg = "";
                if (GlobalConfig.ReadConfig(ref msg))
                {
                    //打开UDP服务端口
                    //udp = new UdpServer();
                    //udp.ReceiveUDP(GlobalConfig.UdpPort, GlobalConfig.LocalIPStr, GlobalConfig.Port);
                    comm = new NewTcpServer(GlobalConfig.Port, GlobalConfig.LocalIP);
                    msg = "";

                    while (true)
                    {
                        //打开端口，失败重试
                        succ = comm.Start(ref msg);
                        if (succ) break;
                        //打开失败后等待1秒再重试
                        Thread.Sleep(5000);
                    }

                    //打开通信失败
                    if (!succ)
                    {
                        log.Warn(msg);
                        this.Stop();
                    }
                }
                else
                {
                    ExceptionLog.MLog("读配置文件失败:"+msg);
                    log.Error("读配置文件失败:" + msg);
                 
                    this.Stop();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.MLog(ex, "打开TCP/UPD通信失败！");
                log.Fatal("打开TCP/UPD通信失败！", ex);
              
                this.Stop();
            }
            return succ;
        }
        #endregion

        //#region 设置定时扫描升级计划数据库，实现定时推送升级信息
        //private System.Timers.Timer myTimer;
        //int times = 0;
        //private void OpenTimer()
        //{
        //    myTimer = new System.Timers.Timer();
        //    myTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Tick);
        //    myTimer.Interval = double.Parse(GlobalConfig.ReadAppSetting("UpdateCheckTime"));//定时一小时检测一次
        //    myTimer.Start();
        //    log.Debug("开启升级计划扫描");
        //}
        //private void StopTimer()
        //{

        //    myTimer.Stop();
        //    log.Debug("停止升级计划扫描");
        //}
        ///// <summary>
        ///// 定时读取升级计划数据库，并发消息给对应终端
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="e"></param>
        //private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    var list = ds.GetUpdateStations();
        //    string str = "";
        //    foreach (var dic in list)
        //    {
        //        foreach (var item in dic)
        //        {
        //            //item.Key 点位
        //            //item.Value 版本实体
        //            XMLPacket pc = new XMLPacket();
        //            pc.CommType = CommandEnum.UpGrade;
        //            pc.PlayerID = item.Key;
        //            //构造终端接收数据：版本号|文件路径|MD5
        //            pc.Value = item.Value.VersionNo + "|" + item.Value.FielPath + "|" + item.Value.MD5;
        //            //todo.向终端发消息，将点位信息和版本信息发给终端。
        //            comm.SendXmlBySid(item.Key, pc, ref str);
        //        }

        //    }
        //}


        //#endregion

        #region 定时将终端日志导入数据库
        private System.Timers.Timer logTimer;
        private void OpenLogTimer()
        {
            logTimer = new System.Timers.Timer();
            logTimer.Elapsed += new System.Timers.ElapsedEventHandler(LogTimer_Tick);
            logTimer.Interval = 3600000;//定时一小时检测一次

            //logTimer.Interval = 6000;
            logTimer.AutoReset = true;
            logTimer.Start();

            log.Debug("开启日志定时导入任务");
        }
        private void StopLogTimer()
        {
            logTimer.Stop();
            log.Debug("停止日志定时导入任务");
        }
        private void LogTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            string itime = GlobalConfig.ReadAppSetting("ClienLogImportTime");
            int hour = 22;//默认22点
            if (!string.IsNullOrEmpty(itime))
            {
                hour = int.Parse(itime);
            }
            if (e.SignalTime.Hour == hour)
            {
                try
                {
                    ds.ImportClientLog(GlobalConfig.ReadAppSetting("ClientLogPath"));
                    ds.DeleteDir(GlobalConfig.ReadAppSetting("ClientLogPath"));
                }
                catch (Exception ex)
                {
                    log.Debug(ex);
                }
            }
        }
        #endregion
    }
}
