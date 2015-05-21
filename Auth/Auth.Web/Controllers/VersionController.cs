using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Model;
using Auth.DataAccess;
using log4net;
using System.Reflection;
using Webdiyer.WebControls.Mvc;
using Auth.Web;
using System.IO;
using System.Text;


namespace Auth.Web.Controllers
{
    public class VersionController : Controller
    {
        private AuthDataContext db = new AuthDataContext();
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /Version/

        public ActionResult Index(string VersionNo, string IsPublish, string ClientType, int id = 1)
        {
            return ajaxSearchGetResult(VersionNo, IsPublish, ClientType, id);
        }
        private ActionResult ajaxSearchGetResult(string VersionNo, string IsPublish, string ClientType, int id = 1)
        {

            var qry = db.VersionInfos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(VersionNo))
                qry = qry.Where(a => a.VersionNo.Contains(VersionNo));

            if (!string.IsNullOrWhiteSpace(IsPublish))
            {
                var isp = int.Parse(IsPublish);
                qry = qry.Where(a => a.IsPublish == isp);
            }

            if (!string.IsNullOrWhiteSpace(ClientType))
            {
                var ctp = int.Parse(ClientType);
                qry = qry.Where(a => a.ClientType == ctp);
            }


            var model = qry.OrderByDescending(a => a.VersionID).ToPagedList(id, 10);

            if (Request.IsAjaxRequest())
                return PartialView("_VersionSearchGet", model);
            return View(model);
        }
        //
        // GET: /Version/Details/5

        public ActionResult Details(int id = 0)
        {
            VersionInfo versioninfo = db.VersionInfos.Find(id);
            if (versioninfo == null)
            {
                return HttpNotFound();
            }
            return View(versioninfo);
        }

        //
        // GET: /Version/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Version/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VersionInfo versioninfo)
        {
            HttpFileCollectionBase fileToUpload = Request.Files;
            var curFile = Request.Files[0];
            string path = "";
            try
            {
                if (curFile.ContentLength > 1)
                {
                   // versioninfo.VersionNo = curFile.FileName.Replace("P_V", "").Replace("M_V", "").Replace(".apk", "").Replace(".exe", "");

                    var filename = System.IO.Path.GetFileName(curFile.FileName);
                    int index = filename.LastIndexOf(".");
                    string lastName = filename.Substring(index, filename.Length - index);

                    string newfile = versioninfo.ClientTypeName + "_" + versioninfo.VersionNo + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + lastName;

                    path = System.IO.Path.Combine(Server.MapPath("~/VersionFile"), newfile);
                    curFile.SaveAs(path);

                    versioninfo.FielPath = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/VersionFile/" + newfile;

                    
                    log.Info("版本文件上传成功:" + path);
                    versioninfo.MD5 = GetMD5HashFromFile(path);
                }
            }
            catch (Exception ex)
            {
                log.Error("版本文件上传失败:" + path, ex);
            }

            versioninfo.IsPublish = 0;
            versioninfo.CreatAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.VersionInfos.Add(versioninfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(versioninfo);
        }
        private string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                log.Error("获取MD5异常", ex);
            }
            return "";
        }

        public ActionResult Publish(int id)
        {
            VersionInfo versioninfo = db.VersionInfos.Find(id);
            versioninfo.IsPublish = 1;
            if (ModelState.IsValid)
            {
                db.Entry(versioninfo).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult PublishCancle(int id)
        {
            VersionInfo versioninfo = db.VersionInfos.Find(id);
            versioninfo.IsPublish = 0;
            if (ModelState.IsValid)
            {
                db.Entry(versioninfo).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Version/Edit/5

        public ActionResult Edit(int id = 0)
        {
            VersionInfo versioninfo = db.VersionInfos.Find(id);
            if (versioninfo == null)
            {
                return HttpNotFound();
            }
            return View(versioninfo);
        }


        //
        // POST: /Version/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VersionInfo versioninfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(versioninfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(versioninfo);
        }

        //
        // GET: /Version/Delete/5

        public ActionResult Delete(int id)
        {
            VersionInfo versioninfo = db.VersionInfos.Find(id);
            db.VersionInfos.Remove(versioninfo);
            db.SaveChanges();

            if (versioninfo.FielPath != null)
            {
                string filename = versioninfo.FielPath.Replace("http://", "").Split(new char[] { '/' })[2];
                var path = System.IO.Path.Combine(Server.MapPath("~/VersionFile"), filename);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}