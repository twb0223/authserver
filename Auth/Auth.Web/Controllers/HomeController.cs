using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Model;
using Auth.DataAccess;
using log4net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Auth.Web.Controllers
{
    public class HomeController : Controller
    {
        private AuthDataContext db = new AuthDataContext();
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // GET: /Home/

        public ActionResult Index()
        {
            //ImportClientLog("D:\\clientlogs\\localuser\\ftplogs");
            return View();
        }
        //public void ImportClientLog(string path)
        //{
        //    var loglist = ReadXMlLog(path);
        //    using (AuthDataContext db = new AuthDataContext())
        //    {
        //        try
        //        {
        //            loglist.ForEach(x =>
        //            {
        //                db.LogInfos.Add(x);
        //            });
        //            db.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error(ex);
        //            throw;
        //        }

        //    }
        //}
        //private List<LogInfo> ReadXMlLog(string path)
        //{
        //    List<LogInfo> list = new List<LogInfo>();
        //    string[] filelist = GetFileNames(path);
        //    XDocument doc;
        //    FileInfo f;
        //    foreach (var item in filelist)
        //    {
        //        f = new FileInfo(item);
        //        if (f.Exists && f.Extension == ".xml")
        //        {
        //            doc = XDocument.Load(item);
        //            var loglist = doc.Descendants("Log");

        //            foreach (var li in loglist)
        //            {
        //                LogInfo m = new LogInfo();
        //                //if (li.Element("time").Value != "")
        //                //{
        //                //    m.Log_date = DateTime.Parse(li.Element("time").Value);
        //                //}
        //                m.Log_date = DateTime.Now;
        //                m.Log_level = li.Element("level").Value;
        //                m.Logger = li.Element("tag").Value;
        //                m.Message = "终端：" + f.Name.Split(new char[] { '_' })[0];
        //                m.Exception = li.Element("content").Value;
        //                list.Add(m);
        //            }
        //        }
        //    }
        //    return list;
        //}
        //private static bool IsExistDirectory(string directoryPath)
        //{
        //    return Directory.Exists(directoryPath);
        //}
        //public static string[] GetFileNames(string directoryPath)
        //{
        //    //如果目录不存在，则抛出异常
        //    if (!IsExistDirectory(directoryPath))
        //    {
        //        throw new FileNotFoundException();
        //    }

        //    //获取文件列表
        //    return Directory.GetFiles(directoryPath);
        //}

        public JsonResult Getoffline()
        {
            StringBuilder sb = new StringBuilder();
            var res = new JsonResult();
            string str = "select top 5 * from View_ClinetOnline";
            var list = db.Database.SqlQuery<OlEntity>(str);
            sb.Append("<table  width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><td width=\"35%\">项目</td><td width=\"35%\">部门</td><td>在线</td><td>离线</td></tr>");
            foreach (var item in list)
            {
                sb.Append(" <tr>");
                sb.Append("<td>" + item.Project + "</td>");
                sb.Append("<td>" + item.DepartMent + "</td>");
                if (item.Totalonline != null)
                {
                    sb.Append("<td>" + item.Totalonline + "</td>");
                }
                else
                {
                    sb.Append("<td>0</td>");
                }
                if (item.Totaloffline != null)
                {
                    sb.Append("<td>" + item.Totaloffline + "</td>");
                }
                else
                {
                    sb.Append("<td>0</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            res.Data = sb.ToString();
            return res;
        }
        public JsonResult GetErrorClient()
        {
            var Getoffline = db.ClientInfos.Where(x => x.ClientStatus == "异常").Take(5).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in Getoffline)
            {
                sb.Append("<a href='/Client/Details/" + item.ClientID + "'>");
                sb.Append("异常终端MAC：" + item.ClientMAC);
                sb.Append(" 异常终端无线MAC：" + item.ClientWifiMAC);
                sb.Append("</a>");
            }
            var res = new JsonResult();
            res.Data = sb.ToString();
            return res;
        }
        public JsonResult GetTask()
        {
            var GetoTask = db.UpdateTasks.Where(x => x.UpdateTime >= DateTime.Now).Take(5).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in GetoTask)
            {
                sb.Append("<a href='/Task/Details/" + item.TaskID + "'>");
                sb.Append("计划名称:" + item.TaskName + "&nbsp;" + "&nbsp;执行时间:" + item.UpdateTime);
                sb.Append("</a>");
            }
            var res = new JsonResult();
            res.Data = sb.ToString();
            return res;
        }

        public JsonResult GetLogs()
        {
            var GetoTask = db.LogInfos.Where(x => x.Log_level!="INFO"&&x.Log_level!="DEBUG").OrderByDescending(x=>x.Log_date).Take(5).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in GetoTask)
            {
                sb.Append("<a href='/Log/Details/" + item.LogID + "'>");
                sb.Append("时间:" + item.Log_date + "&nbsp;" + "&nbsp;内容:");
                if (item.Message.Length > 23)
                {
                    sb.Append(item.Message.Substring(0, 23)+"..");
                }
                else
                {
                    sb.Append(item.Message);
                }
                sb.Append("</a>");
            }
            var res = new JsonResult();
            res.Data = sb.ToString();
            return res;
        }
        //
        // GET: /Home/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }
        // GET: /Home/Create

        public ActionResult Create()
        {
            return View();
        }
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class OlEntity
    {
        public string Project { get; set; }
        public string DepartMent { get; set; }
        public int? Totalonline { get; set; }
        public int? Totaloffline { get; set; }
    }
}
