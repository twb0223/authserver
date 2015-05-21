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
using Auth.Web;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using Microsoft.Http;


namespace Auth.Web.Controllers
{
    public class TaskController : Controller
    {
        private AuthDataContext db = new AuthDataContext();
        private string baseUrl = ConfigurationManager.AppSettings["wcfBasicAddress"].ToString();
        public ActionResult Index(string VersionNo, string startdate, string enddate, int id = 1)
        {
            return ajaxSearchGetResult(VersionNo, startdate, enddate,id);
        }
        private ActionResult ajaxSearchGetResult(string VersionNo, string startdate, string enddate, int id)
        {

            var qry = db.UpdateTasks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(VersionNo))
                qry = qry.Where(a => a.VersionNo.Contains(VersionNo));

            if (!string.IsNullOrWhiteSpace(startdate))
            {
                DateTime sdate = DateTime.Parse(startdate);
                qry = qry.Where(a => a.UpdateTime >= sdate);
            }
            if (!string.IsNullOrWhiteSpace(enddate))
            {
                DateTime edate = DateTime.Parse(enddate);
                qry = qry.Where(a => a.UpdateTime < edate);
            }

            var model = qry.OrderByDescending(a => a.TaskID).ToPagedList(id, 10);

            if (Request.IsAjaxRequest())
                return PartialView("_TaskSearchGet", model);
            return View(model);
        }
        public ActionResult Details(int id = 0)
        {
            UpdateTask updatetask = db.UpdateTasks.Find(id);
            if (updatetask == null)
            {
                return HttpNotFound();
            }
            return View(updatetask);
        }

        public ActionResult ShowPro(string ProjectInfoName, int id = 1)
        {
            return ajaxSearchGetResultForPro(ProjectInfoName, id);
        }

        private ActionResult ajaxSearchGetResultForPro(string ProjectInfoName, int id)
        {
            var qry = GetProjectInfo().AsQueryable();
            if (!string.IsNullOrWhiteSpace(ProjectInfoName))
                qry = qry.Where(a => a.ProjectInfoName.Contains(ProjectInfoName));
            var model = qry.OrderByDescending(a => a.ProjectInfoID).ToPagedList(id, 10);
            if (Request.IsAjaxRequest())
                return PartialView("_ShowProSearchGet", model);
            return View(model);
        }
        private List<ProjectInfo> GetProjectInfo()
        {
            List<ProjectInfo> plist = new List<ProjectInfo>();
            SQLHelper sql = new SQLHelper();
            DataTable dt = sql.GetDataTable("Select [StationPorjectID],[StationPorjectName] from [DMS_TBM_StationPorject]");
            foreach (DataRow item in dt.Rows)
            {
                ProjectInfo m = new ProjectInfo();
                m.ProjectInfoID = int.Parse(item["StationPorjectID"].ToString());
                m.ProjectInfoName = item["StationPorjectName"].ToString();
                plist.Add(m);
            }
            return plist;
        }

        private List<DepartmentInfo> GetDepInfo()
        {
            List<DepartmentInfo> plist = new List<DepartmentInfo>();
            SQLHelper sql = new SQLHelper();
            DataTable dt = sql.GetDataTable("Select [StationDepartmentID],[StationDepartmentName] from [DMS_TBM_StationDepartment]");
            foreach (DataRow item in dt.Rows)
            {
                DepartmentInfo m = new DepartmentInfo();
                m.DepartmentInfoID = int.Parse(item["StationDepartmentID"].ToString());
                m.DepartmentInfoName = item["StationDepartmentName"].ToString();
                plist.Add(m);
            }
            return plist;
        }

        public ActionResult ShowDep(string DepartmentInfoName, int id = 1)
        {
            return ajaxSearchGetResultForDep(DepartmentInfoName, id);
        }
        private ActionResult ajaxSearchGetResultForDep(string DepartmentInfoName, int id)
        {
            var qry = GetDepInfo().AsQueryable();
            if (!string.IsNullOrWhiteSpace(DepartmentInfoName))
                qry = qry.Where(a => a.DepartmentInfoName.Contains(DepartmentInfoName));
            var model = qry.OrderByDescending(a => a.DepartmentInfoID).ToPagedList(id, 10);
            if (Request.IsAjaxRequest())
                return PartialView("_ShowDepSearchGet", model);
            return View(model);
        }
        public ActionResult ShowStation(string PointNo, int id = 1)
        {
            return ajaxSearchGetResultForStation(PointNo, id);
        }
        private ActionResult ajaxSearchGetResultForStation(string PointNo, int id)
        {
            var qry = GetPointInfo().AsQueryable();
            if (!string.IsNullOrWhiteSpace(PointNo))
                qry = qry.Where(a => a.PointNo.Contains(PointNo));
            var model = qry.OrderByDescending(a => a.PointInfoID).ToPagedList(id, 10);
            if (Request.IsAjaxRequest())
                return PartialView("_ShowStationSearchGet", model);
            return View(model);
        }
        private List<PointInfo> GetPointInfo()
        {
            List<PointInfo> plist = new List<PointInfo>();
            SQLHelper sql = new SQLHelper();
            DataTable dt = sql.GetDataTable("Select [StationID],[StationName] from [DMS_TBM_Station]");
            foreach (DataRow item in dt.Rows)
            {
                PointInfo m = new PointInfo();
                m.PointNo = item["StationID"].ToString();
                m.PointName = item["StationName"].ToString();
                plist.Add(m);
            }
            return plist;
        }

        //
        // GET: /Task/Create

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UpdateTask updatetask)
        {
            if (ModelState.IsValid)
            {
                db.UpdateTasks.Add(updatetask);
                db.SaveChanges();
                //推送升级消息给终端
                PushMsgToClient(updatetask.TaskID);  
                return RedirectToAction("Index");
            }
            return View(updatetask);
        }
        private string PushMsgToClient(int utid)
        {
            var client = new HttpClient();
            var strUrl = baseUrl + "SendUpdateMsg/"+utid;
            var response = client.Get(strUrl);
            response.EnsureStatusIsSuccessful();
           return response.Content.ReadAsString();
         
        }
        public ActionResult Edit(int id = 0)
        {
            UpdateTask updatetask = db.UpdateTasks.Find(id);
            if (updatetask == null)
            {
                return HttpNotFound();
            }
            return View(updatetask);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UpdateTask updatetask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(updatetask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(updatetask);
        }
        public ActionResult Delete(int id = 0)
        {
            UpdateTask updatetask = db.UpdateTasks.Find(id);
            db.UpdateTasks.Remove(updatetask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}