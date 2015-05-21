using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Model;
using Auth.DataAccess;
using Webdiyer.WebControls.Mvc;
using System.Threading;
using System.IO;
using System.Text;
using System.Reflection;
using log4net;
using System.Data.SqlClient;
namespace Auth.Web.Controllers
{
    public class LogController : Controller
    {
        private AuthDataContext db = new AuthDataContext();
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index(string Log_level, string logType, string StartDate, string EndDate, int id = 1)
        {
            return ajaxSearchGetResult(Log_level, logType, StartDate, EndDate, id);
        }
        private ActionResult ajaxSearchGetResult(string Log_level, string logType, string startdate, string enddate, int id = 1)
        {

            var qry = db.LogInfos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(Log_level))
                qry = qry.Where(a => a.Log_level == Log_level);
            if (!string.IsNullOrWhiteSpace(startdate))
            {
                DateTime sdate = DateTime.Parse(startdate);
                qry = qry.Where(a => a.Log_date >= sdate);
            }
            if (!string.IsNullOrWhiteSpace(enddate))
            {
                DateTime edate = DateTime.Parse(enddate);
                qry = qry.Where(a => a.Log_date < edate);
            }
            var model = qry.OrderByDescending(a => a.Log_date).ToPagedList(id, 10);
            if (Request.IsAjaxRequest())

                return PartialView("_LogSearchGet", model);
            return View(model);
        }
        public ActionResult Details(int id = 0)
        {
            LogInfo loginfo = db.LogInfos.Find(id);
            if (loginfo == null)
            {
                return HttpNotFound();
            }
            return View(loginfo);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public FileResult Export()
        {

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("log1");
            //获取list数据

            var list = db.LogInfos.ToList();

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("日志编号");
            row1.CreateCell(1).SetCellValue("日期");
            row1.CreateCell(2).SetCellValue("日志级别");
            row1.CreateCell(3).SetCellValue("记录器");
            row1.CreateCell(4).SetCellValue("线程");
            row1.CreateCell(5).SetCellValue("消息");
            row1.CreateCell(6).SetCellValue("异常");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].LogID.ToString());
                rowtemp.CreateCell(1).SetCellValue(list[i].Log_date.ToString());
                rowtemp.CreateCell(2).SetCellValue(list[i].Log_level.ToString());
                rowtemp.CreateCell(3).SetCellValue(list[i].Logger.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].Thread.ToString());
                rowtemp.CreateCell(5).SetCellValue(list[i].Message.ToString());
                rowtemp.CreateCell(6).SetCellValue(list[i].Exception.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            string filename = "Logs_" + DateTime.Now.ToString() + ".xls";
            return File(ms, "application/vnd.ms-excel", filename);
        }


        public ActionResult Clean()
        {
            using (AuthDataContext db = new AuthDataContext())
            {
                try
                {
                    //db.LogInfos.SqlQuery("Truncate Table LogInfo");
                    db.Database.ExecuteSqlCommand("Truncate Table LogInfo");
                }
                catch (Exception ex)
                {
                    log.Error("日志清空异常", ex);
                }

            }
            return RedirectToAction("Index");
        }
    }
}