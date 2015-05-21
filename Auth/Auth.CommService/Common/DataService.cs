using Auth.DataAccess;
using Auth.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Auth.CommService
{
    public class DataService
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SQLHelper sql;

        public DataService()
        {
            sql = new SQLHelper();
        }
        #region 终端请求服务器数据操作

        #region 认证
        /// <summary>
        /// 更新基础数据平台数据到终端
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateClient(ClientInfo entity)
        {
            using (AuthDataContext ad = new AuthDataContext())
            {
                try
                {
                    ad.Entry(entity).State = EntityState.Modified;
                    ad.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error("终端信息同步异常", ex);
                }
            }
        }
        /// <summary>
        /// 通过设备编号读取设备明细
        /// </summary>
        /// <param name="deviceid">CPU,Disk,Mac</param>
        /// <returns>返回设备信息实体</returns>am>
        /// <returns></returns>
        public BaseClientInfo GetEntity(string cpu, string disk, string mac, string wifimac)
        {
            BaseClientInfo model = new BaseClientInfo();
            StringBuilder sb = new StringBuilder();
            string str = "";

            sb.Append("Select dves.StationID,dve.StationName,dve.EquipmentID,dve.EquipmentName,dve.EquipmentMac,dve.EquipmentIP,dve.EquipmentPort,dve.EquipmentCpuTypeName,dve.OsName,dvs.InstitutionName,dvs.VersionNo,dve.EquipmentCPU,dve.EquipmentDisk,dve.EquipmentWifiMac,dve.ClientChangeFlag,t1.StationDepartmentName,t1.StationPorjectName ");
            sb.Append(" from [DMS_View_EquipmentStation]  dves inner join [DMS_View_Equipment] dve on dves.EquipmentID=dve.EquipmentID  inner join [DMS_View_Station] dvs on dves.StationID=dvs.StationID ");
            sb.Append(" inner join (select  dts.StationID,dts.StationName,tsd.StationDepartmentName,tsp.StationPorjectName from dbo.DMS_TBM_Station dts inner join dbo.DMS_TBM_StationDepartment tsd on tsd.StationDepartmentID=dts.StationDepartmentID inner join dbo.DMS_TBM_StationPorject tsp on tsp.StationPorjectID=dts.StationPorjectID) t1 on t1.StationID=dves.StationID");

            //sb.Append(" Where dve.EquipmentDisk='{0}' and dve.EquipmentCPU='{1}' and (dve.EquipmentMac='{2}' or dve.EquipmentWifiMac='{3}')");
            //str = string.Format(sb.ToString(), disk, cpu, mac, wifimac);

            sb.Append(" Where dve.EquipmentMac='{0}' or dve.EquipmentWifiMac='{1}'");
            str = string.Format(sb.ToString(), mac, wifimac);
           
            try
            {

                DataTable dt = sql.GetDataTable(str);
                foreach (DataRow dv in dt.Rows)
                {
                    model.EquipmentID = dv["EquipmentID"].ToString();
                    model.EquipmentMac = dv["EquipmentMac"].ToString();
                    model.EquipmentIP = dv["EquipmentIP"].ToString();
                    model.EquipmentName = dv["EquipmentName"].ToString();
                    model.StationID = dv["StationID"].ToString();
                    model.StationName = dv["StationName"].ToString();
                    model.EquipmentPort = dv["EquipmentPort"].ToString();
                    model.OsName = dv["OsName"].ToString();
                    model.EquipmentCpuTypeName = dv["EquipmentCpuTypeName"].ToString();
                    model.VersionNo = dv["VersionNo"].ToString();
                    model.InstitutionName = dv["InstitutionName"].ToString();
                    model.EquipmentCPU = dv["EquipmentCPU"].ToString();
                    model.EquipmentMemory = dv["EquipmentDisk"].ToString();
                    model.EquipmentWifiMac = dv["EquipmentWifiMac"].ToString();
                    model.ClientChangeFlag = dv["ClientChangeFlag"].ToString();
                    model.Project = dv["StationPorjectName"].ToString();
                    model.Department = dv["StationDepartmentName"].ToString();
                }
            }
            catch (Exception ex)
            {
                log.Error("读取基础数据异常", ex);

            }

            return model;
        }

        public ClientInfo GetEntityForClientInfo(string cpu, string disk, string mac, string wifimac)
        {
            ClientInfo model = new ClientInfo();
            using (AuthDataContext ad = new AuthDataContext())
            {
                try
                {
                    // model = ad.ClientInfos.Where(x => x.ClientMemory == disk && x.ClientCpu == cpu && (x.ClientWifiMAC == wifimac || x.ClientMAC == mac)).FirstOrDefault();
                    model = ad.ClientInfos.Where(x => x.ClientCpu == cpu && (x.ClientWifiMAC == wifimac || x.ClientMAC == mac)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Error("获取认证服务器数据失败！", ex);
                }
            }
            return model;
        }
        public ClientInfo GetEntity(string deviceid)
        {
            ClientInfo model = new ClientInfo();
            using (AuthDataContext ad = new AuthDataContext())
            {
                try
                {
                    model = ad.ClientInfos.Where(x => x.ClientDeviceID == deviceid).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Error("获取认证服务器数据失败！", ex);
                }
            }
            return model;
        }
        /// <summary>
        /// 通过设备编号读取设备明细
        /// </summary>
        /// <param name="deviceid">设备编号</param>
        /// <returns>返回设备信息实体</returns>
        public BaseClientInfo GetBaseEntity(string deviceid)
        {
            BaseClientInfo model = new BaseClientInfo();
            StringBuilder sb = new StringBuilder();
            sb.Append("Select dves.StationID,dve.StationName,dve.EquipmentID,dve.EquipmentName,dve.EquipmentMac,dve.EquipmentIP,dve.EquipmentPort,dve.EquipmentCpuTypeName,dve.OsName,dvs.InstitutionName,dvs.VersionNo,dve.EquipmentCPU,dve.EquipmentDisk,dve.EquipmentWifiMac,dve.ClientChangeFlag,t1.StationDepartmentName,t1.StationPorjectName ");
            sb.Append(" from [DMS_View_EquipmentStation]  dves inner join [DMS_View_Equipment] dve on dves.EquipmentID=dve.EquipmentID  inner join [DMS_View_Station] dvs on dves.StationID=dvs.StationID ");
            sb.Append(" inner join (select  dts.StationID,dts.StationName,tsd.StationDepartmentName,tsp.StationPorjectName from dbo.DMS_TBM_Station dts inner join dbo.DMS_TBM_StationDepartment tsd on tsd.StationDepartmentID=dts.StationDepartmentID inner join dbo.DMS_TBM_StationPorject tsp on tsp.StationPorjectID=dts.StationPorjectID) t1 on t1.StationID=dves.StationID");
            sb.Append(" Where dve.EquipmentID='{0}'");
            string str = string.Format(sb.ToString(), deviceid);
            try
            {
                DataTable dt = sql.GetDataTable(str);
                foreach (DataRow dv in dt.Rows)
                {
                    model.EquipmentID = dv["EquipmentID"].ToString();
                    model.EquipmentMac = dv["EquipmentMac"].ToString();
                    model.EquipmentIP = dv["EquipmentIP"].ToString();
                    model.EquipmentName = dv["EquipmentName"].ToString();
                    model.StationID = dv["StationID"].ToString();
                    model.StationName = dv["StationName"].ToString();
                    model.EquipmentPort = dv["EquipmentPort"].ToString();
                    model.OsName = dv["OsName"].ToString();
                    model.EquipmentCpuTypeName = dv["EquipmentCpuTypeName"].ToString();
                    model.VersionNo = dv["VersionNo"].ToString();
                    model.InstitutionName = dv["InstitutionName"].ToString();
                    model.EquipmentCPU = dv["EquipmentCPU"].ToString();
                    model.EquipmentMemory = dv["EquipmentDisk"].ToString();
                    model.EquipmentWifiMac = dv["EquipmentWifiMac"].ToString();
                    model.ClientChangeFlag = dv["ClientChangeFlag"].ToString();
                    model.Project = dv["StationPorjectName"].ToString();
                    model.Department = dv["StationDepartmentName"].ToString();
                }
            }
            catch (Exception ex)
            {
                log.Error("读取基础数据平台异常", ex);

            }

            return model;
        }
        /// <summary>
        /// 将基础数据平台数据在终端认证时自动加入到本地数据库
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(ClientInfo entity)
        {
            using (AuthDataContext db = new AuthDataContext())
            {
                try
                {
                    db.ClientInfos.Add(entity);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error("终端信息添加异常", ex);
                }
            }
        }

        public void Delete(ClientInfo entity)
        {

            using (AuthDataContext db = new AuthDataContext())
            {
                try
                {
                    db.ClientInfos.Remove(entity);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error("终端信息删除异常", ex);
                }
            }
        }

        #endregion

        #region 升级
        public VersionInfo GetVersionEntity(int clienttype, string type)
        {
            VersionInfo model = new VersionInfo();
            using (AuthDataContext bb = new AuthDataContext())
            {
                model = bb.VersionInfos.Where(x => x.ClientType == clienttype && x.IsPublish == 1 && x.ClientTypeName == type).OrderByDescending(x => x.VersionNo).FirstOrDefault();
            }
            return model;
        }

        public VersionInfo GetVersionEntityByID(string versionid)
        {
            VersionInfo model = new VersionInfo();
            using (AuthDataContext bb = new AuthDataContext())
            {
                model = bb.VersionInfos.Find(int.Parse(versionid));

            }
            if (model.IsPublish == 0)
            {
                model = null;
            }
            return model;
        }
        #endregion

        #region 在线状态
        /// <summary>
        /// 批量更新终端在线状态
        /// </summary>
        /// <param name="dicStatus"></param>
        /// <param name="offLineAll"></param>
        /// <returns></returns>
        public bool BatUpdateOnlineStatus(Dictionary<string, List<string>> dicStatus, bool offLineAll = false)
        {
            string sql = string.Empty;
            List<string> sqlList = new List<string>();
            int result = 0;
            //将所有终端置为不在线，退出服务时执行
            if (offLineAll)
            {
                //无心跳全部置0
                sql = "update ClientInfo set OnlineStatus ='0'";
                using (AuthDataContext ad = new AuthDataContext())
                {
                    result = ad.Database.ExecuteSqlCommand(sql);
                }
            }
            else
            {
                using (AuthDataContext ad = new AuthDataContext())
                {
                    foreach (KeyValuePair<string, List<string>> pair in dicStatus)
                    {
                        //获得所有点位
                        List<string> listOnline = pair.Value;
                        if (listOnline.Count > 0)
                        {
                            //获得在线状态
                            string status ="1";
                            StringBuilder strSqlList = new StringBuilder();
                            strSqlList.Append("('");
                            string strOnline = string.Join("','", listOnline.ToArray());
                            strSqlList.Append(strOnline);
                            strSqlList.Append("')");
                            sql += string.Format("update ClientInfo set OnlineStatus='{0}',LastHitTime={1} where Point in {2}  ", status, "getdate()", strSqlList);
                        }
                    }
                    result = ad.Database.ExecuteSqlCommand(sql);
                }
            }
            if (result >=0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 更新在线状态
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="offline"></param>
        /// <returns></returns>
        public bool UpdateOnlineStatus(string sid, int offline)
        {
            int scl = 0;
            using (AuthDataContext db = new AuthDataContext())
            {
                var entity = db.ClientInfos.Where(x => x.Point == sid).FirstOrDefault();
                entity.LastHitTime = DateTime.Now;
                if (offline == 1)
                {
                    entity.OnlineStatus = "1";
                }
                else
                {
                    entity.OnlineStatus = "0";
                }
                db.Entry(entity).State = EntityState.Modified;
                scl = db.SaveChanges();
            }
            if (scl > 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 加密相关
        public string GetCommKeyByPoint(string DeviceID)
        {
            string key = string.Empty;
            using (AuthDataContext db = new AuthDataContext())
            {
                key = db.ClientInfos.Where(x => x.ClientDeviceID == DeviceID).Select(x => x.CommKeys).FirstOrDefault();
            }
            return key;
        }

        #endregion

        #endregion

        #region 服务器发送终端数据操作

        #endregion

        #region window服务操作
        #region 推送升级消息


        /// <summary>
        /// 获取计划更新点位 版本字典信息
        /// </summary>
        /// <returns></returns>
        //public List<Dictionary<string,  VersionInfo>> GetUpdateStations()
        //{
        //    List<Dictionary<string, VersionInfo>> list = new List<Dictionary<string, VersionInfo>>();
        //    using (AuthDataContext db = new AuthDataContext())
        //    {
        //        try
        //        {
        //            var tlist = db.UpdateTasks.Where(x => x.UpdateTime <= DateTime.Now).ToList();
        //            VersionInfo ve;
        //            tlist.ForEach(x =>
        //            {
        //                Dictionary<string, VersionInfo> dic = new Dictionary<string, VersionInfo>();

        //                ve = db.VersionInfos.Find(x.VersionID);//找到该计划对应的版本实体
        //                switch (x.UpdateScope)
        //                {
        //                    case 1:
        //                        dic = UpdateListForPro(x.IDList, ve);
        //                        break;
        //                    case 2:
        //                        dic = UpdateListForDep(x.IDList, ve);
        //                        break;
        //                    case 3:
        //                        dic = UpdateListForStation(x.IDList, ve);
        //                        break;
        //                    default:
        //                        dic = UpdateListForAll(ve);
        //                        break;
        //                }
        //                list.Add(dic);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("更新计划数据提取异常", ex);
        //        }
        //    }
        //    return list;
        //}
        /// <summary>
        /// 立即更新点位 版本字典信息
        /// </summary>
        /// <param name="utid">计划ID</param>
        /// <returns></returns>
        public Dictionary<string, VersionInfo> GetUpDateStationsForTasKID(int utid, ref string isupdatenow, ref string updatetime, ref string updateOrRollback)
        {

            Dictionary<string, VersionInfo> dic = new Dictionary<string, VersionInfo>();
            using (AuthDataContext db = new AuthDataContext())
            {
                var ut = db.UpdateTasks.Find(utid);
                VersionInfo ve = db.VersionInfos.Find(ut.VersionID);
                updatetime = ut.UpdateTime.ToString();
                isupdatenow = ut.IsExecNow.ToString();
                updateOrRollback = ut.UpdateType.ToString();
                switch (ut.UpdateScope)
                {
                    case 1:
                        dic = UpdateListForPro(ut.IDList, ve);
                        break;
                    case 2:
                        dic = UpdateListForDep(ut.IDList, ve);
                        break;
                    case 3:
                        dic = UpdateListForStation(ut.IDList, ve);
                        break;
                    default:
                        dic = UpdateListForAll(ve);
                        break;
                }

            }
            return dic;
        }
        /// <summary>
        /// 解析计划列表中更新范围集合: 项目集合，构建字典信息。
        /// </summary>
        /// <param name="idatalist"></param>
        /// <param name="ve"></param>
        /// <returns></returns>
        private Dictionary<string, VersionInfo> UpdateListForPro(string idatalist, VersionInfo ve)
        {
            Dictionary<string, VersionInfo> dic = new Dictionary<string, VersionInfo>();
            string[] str = idatalist.Split(new char[] { '|' });
            string strsql = "";
            DataTable dt;
            foreach (var pid in str)
            {
                strsql = string.Format("Select StationID from [DMS_TBM_Station] Where [StationPorjectID]='{0}'", int.Parse(pid));
                dt = sql.GetDataTable(strsql);
                foreach (DataRow dr in dt.Rows)
                {
                    dic.Add(dr["StationID"].ToString(), ve);
                }
            }
            return dic;
        }
        private Dictionary<string, VersionInfo> UpdateListForDep(string idatalist, VersionInfo ve)
        {
            Dictionary<string, VersionInfo> dic = new Dictionary<string, VersionInfo>();
            string[] str = idatalist.Split(new char[] { '|' });
            string strsql = "";
            DataTable dt;
            foreach (var depid in str)
            {
                strsql = string.Format("Select StationID from [DMS_TBM_Station] Where [StationDepartmentID]='{0}'", int.Parse(depid));
                dt = sql.GetDataTable(strsql);
                foreach (DataRow dr in dt.Rows)
                {
                    dic.Add(dr["StationID"].ToString(), ve);
                }
            }
            return dic;
        }
        private Dictionary<string, VersionInfo> UpdateListForStation(string idatalist, VersionInfo ve)
        {

            Dictionary<string, VersionInfo> dic = new Dictionary<string, VersionInfo>();
            string[] str = idatalist.Split(new char[] { '|' });
            foreach (var sid in str)
            {
                dic.Add(sid, ve);
            }
            return dic;
        }
        /// <summary>
        /// 获取所有点位信息
        /// </summary>
        /// <param name="ve"></param>
        /// <returns></returns>
        private Dictionary<string, VersionInfo> UpdateListForAll(VersionInfo ve)
        {
            Dictionary<string, VersionInfo> dic = new Dictionary<string, VersionInfo>();
            using (AuthDataContext db = new AuthDataContext())
            {
                var cilist = db.ClientInfos.Where(x => x.OnlineStatus == "1").ToList();
                cilist.ForEach(x =>
                {
                    dic.Add(x.Point, ve);
                });
            }
            return dic;

        }
        #endregion

        //#region 自动导入终端日志
        //public void ImportClientLogs(String ftp, String username, String password)
        //{
        //    StringBuilder result = new StringBuilder();
        //    FtpWebRequest reqFTP;
        //    try
        //    {
        //        String ftpserver;
        //        if (ftp.StartsWith("ftp"))
        //        {
        //            if (ftp.StartsWith("ftp://"))
        //            {

        //                ftpserver = ftp;
        //            }
        //            else
        //            {
        //                ftpserver = ftp + "//:";
        //            }

        //        }
        //        else
        //        {
        //            if (ftp.EndsWith("/"))
        //            {

        //                ftpserver = "ftp://" + ftp;
        //            }
        //            else
        //            {

        //                ftpserver = "ftp://" + ftp + "/";
        //            }

        //        }
        //        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpserver));
        //        reqFTP.UsePassive = false;
        //        reqFTP.UseBinary = true;
        //        reqFTP.Credentials = new NetworkCredential(username, password);
        //        reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
        //        FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
        //        StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
        //        string line = reader.ReadLine();

        //        while (line != null)
        //        {

        //            if (line.EndsWith(".xml"))
        //            {

        //                result.Append(line);
        //                result.Append("\n");
        //            }
        //            line = reader.ReadLine();

        //        }

        //        result.Remove(result.ToString().LastIndexOf('\n'), 1);

        //        reader.Close();
        //        response.Close();


        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("终端日志读取异常", ex);
        //    }
        //}
        //public static String readerFtpFile(String ftp, String username, String password, String filename)
        //{

        //    StringBuilder result = new StringBuilder();
        //    FtpWebRequest reqFTP;
        //    try
        //    {
        //        String ftpserver = ftp + filename;
        //        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpserver));
        //        reqFTP.UsePassive = false;
        //        reqFTP.UseBinary = true;
        //        reqFTP.Credentials = new NetworkCredential(username, password);
        //        reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
        //        FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
        //        StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
        //        string line = reader.ReadLine();
        //        while (line != null)
        //        {
        //            line = reader.ReadLine();
        //            result.Append("\n");
        //        }
        //        result.Remove(result.ToString().LastIndexOf('\n'), 1);
        //        reader.Close();
        //        response.Close();

        //        return result.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        return result.ToString();

        //    }
        //}
        //#endregion

        #region 文件操作
        public void ImportClientLog(string path)
        {
            var loglist = ReadXMlLog(path);

            StringBuilder sb = new StringBuilder();
            using (AuthDataContext db = new AuthDataContext())
            {
                loglist.ForEach(x =>
                {
                    sb.Append(string.Format("  insert into loginfo (Log_date,Log_level,Logger,Message,Exception) values('{0}','{1}','{2}','{3}','{4}')  ",x.Log_date,x.Log_level,x.Logger,x.Message,x.Exception));
                    //log.Debug(sb.ToString());
                    //db.LogInfos.Add(x);
                });
               // db.SaveChanges();
                //log.Debug(sb.ToString());
                db.Database.ExecuteSqlCommand(sb.ToString());
            }
        }

        //public void test()
        //{
        //    using (AuthDataContext db = new AuthDataContext())
        //    {
        //        db.Database.ExecuteSqlCommand("insert into loginfo (Log_date,Log_level,Logger,Message,Exception) values('2014-2-3 09:45:36.252','INFO','3333333','3333','33333')");
        //    }
        //}
        private List<LogInfo> ReadXMlLog(string path)
        {

            List<LogInfo> list = new List<LogInfo>();
            string[] filelist = GetFileNames(path);
            XDocument doc;
            FileInfo f;
            try
            {
                foreach (var item in filelist)
                {
                    f = new FileInfo(item);
                    if (f.Exists && f.Extension == ".xml")
                    {
                        doc = XDocument.Load(item);
                        var loglist = doc.Descendants("Log");

                        foreach (var li in loglist)
                        {
                            LogInfo m = new LogInfo();
                            if (li.Element("time").Value != "")
                            {
                                m.Log_date = DateTime.Parse(li.Element("time").Value);
                                m.Log_level = li.Element("level").Value;
                                m.Logger = li.Element("tag").Value;
                                m.Message = "终端：" + f.Name.Split(new char[] { '_' })[0];
                                m.Exception = li.Element("content").Value;
                                list.Add(m);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("读取终端日志异常:" + ex);

            }
            
            return list;
        }
        private static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        private static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }
        public bool DeleteDir(string strPath)
        {
            try
            { // 清除空格 
                strPath = @strPath.Trim().ToString();
                // 判断文件夹是否存在  
                if (System.IO.Directory.Exists(strPath))
                { 
                    // 获得文件夹数组  
                    string[] strDirs = System.IO.Directory.GetDirectories(strPath);
                    // 获得文件数组 
                    string[] strFiles = System.IO.Directory.GetFiles(strPath);
                    // 遍历所有子文件夹 
                    foreach (string strFile in strFiles)
                    { 
                        // 删除文件夹  
                        System.IO.File.Delete(strFile);
                    }
                    // 遍历所有文件 
                    foreach (string strdir in strDirs)
                    { 
                        // 删除文件 
                        System.IO.Directory.Delete(strdir, true);
                    }
                }
                // 成功  
                return true;
            }
            catch (Exception Exp) // 异常处理         
            {
                log.Error("终端日志清空异常",Exp);
                return false;
            }
        }
        #endregion
        #endregion
    }
}
