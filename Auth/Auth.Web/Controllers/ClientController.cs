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
using System.Reflection;
using log4net;
using System.Configuration;
using Microsoft.Http;
using Newtonsoft.Json;

namespace Auth.Web.Controllers
{
    public class ClientController : Controller
    {
        private AuthDataContext db = new AuthDataContext();
        private string baseUrl = ConfigurationManager.AppSettings["wcfBasicAddress"].ToString();
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index(string ClientName, string ClientDeviceID, string ClientStatus, int id = 1)
        {
            return ajaxSearchGetResult(ClientName, ClientDeviceID, ClientStatus, id);
        }

        private ActionResult ajaxSearchGetResult(string ClientName, string ClientDeviceID, string ClientStatus, int id = 1)
        {
            var qry = db.ClientInfos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(ClientName))
                qry = qry.Where(a => a.ClientName.Contains(ClientName));

            if (!string.IsNullOrWhiteSpace(ClientDeviceID))
                qry = qry.Where(a => a.ClientDeviceID.Contains(ClientDeviceID));

            if (!string.IsNullOrWhiteSpace(ClientStatus))
                qry = qry.Where(a => a.ClientStatus == ClientStatus);

            var model = qry.OrderByDescending(a => a.ClientID).ToPagedList(id, 10);
            if (Request.IsAjaxRequest())
                return PartialView("_ClientSearchGet", model);
            return View(model);
        }
        public JsonResult BatCheck(string cids, bool flag)
        {
            int result = 0;
            int realupdatecount = 0;
            bool represult = true;
            string[] ids = cids.TrimEnd(new char[] { '|' }).Split(new char[] { '|' });
            try
            {
                foreach (var id in ids)
                {
                    ClientInfo clientinfo = db.ClientInfos.Find(int.Parse(id));
                    if (flag)
                    {
                        if (clientinfo.ClientStatus == "待验证")
                        {
                            clientinfo.ClientStatus = "验证通过";
                            db.Entry(clientinfo).State = EntityState.Modified;
                            realupdatecount++;
                        }
                    }
                    else
                    {
                        db.ClientInfos.Remove(clientinfo);
                        realupdatecount++;
                    }
                }
                result = db.SaveChanges();
                log.Info("批量验证通过终端：[" + cids + "]");
            }
            catch (Exception ex)
            {
                log.Error("批量验证通过终端：[" + cids + "]", ex);
                represult = false;
            }
            if (result == realupdatecount)
            {
                represult = true;
            }
            return Json(new { RepResults = represult });
        }
        public ActionResult Pass(int id, bool flag)
        {
            int result = 0;
            try
            {
                ClientInfo clientinfo = db.ClientInfos.Find(id);
                if (flag)
                {
                    clientinfo.ClientStatus = "验证通过";
                    db.Entry(clientinfo).State = EntityState.Modified;
                }
                else
                {
                    db.ClientInfos.Remove(clientinfo);
                }
                result = db.SaveChanges();
                log.Info("审核通过终端：[" + id + "]");
            }
            catch (Exception ex)
            {
                log.Error("审核通过终端：[" + id + "]", ex);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Command(int id, string flag)
        {
            var sid = db.ClientInfos.Find(id).Point;
            PushMsgToClient(sid, flag);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 推送数据到客户端
        /// </summary>
        /// <param name="sid">点位</param>
        /// <returns></returns>
        private string PushMsgToClient(string sid, string flag)
        {
            var client = new HttpClient();
            var strUrl = baseUrl + "Command/" + sid + "/" + flag;
            var response = client.Get(strUrl);
            response.EnsureStatusIsSuccessful();
            return response.Content.ReadAsString();
        }
        public ActionResult Details(int id = 0)
        {
            ClientInfo clientinfo = db.ClientInfos.Find(id);
            if (clientinfo == null)
            {
                return HttpNotFound();
            }
            return View(clientinfo);
        }

        private ActionResult ajaxSearchTotalGetResult(string ProName, string DepName,  int id = 1)
        {
            string str = "select * from View_ClinetOnline";
            var qry = db.Database.SqlQuery<OlEntity>(str);

            if (!string.IsNullOrWhiteSpace(ProName))
                qry = qry.Where(a => a.Project.Contains(ProName));

            if (!string.IsNullOrWhiteSpace(DepName))
                qry = qry.Where(a => a.DepartMent.Contains(DepName));

            var model = qry.OrderByDescending(a => a.Project).ToPagedList(id, 10);
            if (Request.IsAjaxRequest())
                return PartialView("_TotalSearchGet", model);
            return View(model);
        }
        public ActionResult Total(string ProName, string DepName, int id = 1)
        {
            return ajaxSearchTotalGetResult(ProName,DepName,id);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}