using Auth.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Auth.Model;
using System.Security.Cryptography;


namespace Auth.Web
{
    public class Utils
    {

        private AuthDataContext db = new AuthDataContext();
        public List<SelectListItem> Selectlists(string flag)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (flag == "ctype")
            {
                items.Add(new SelectListItem { Text = "X86", Value = "1" });
                items.Add(new SelectListItem { Text = "Android", Value = "2" });
                items.Add(new SelectListItem { Text = "Linux", Value = "3" });
                return items;
            }
            if (flag == "ispub")
            {
                items.Add(new SelectListItem { Text = "是", Value = "1" });
                items.Add(new SelectListItem { Text = "否", Value = "0" });
                return items;
            }
            if (flag == "version")
            {
                var list = db.VersionInfos.Where(x => x.IsPublish == 1);
                foreach (var item in list)
                {
                    items.Add(new SelectListItem { Text =item.ClientTypeName+"_"+item.VersionNo, Value = item.VersionID.ToString() });
                }
                return items;
            }
            if (flag == "utp")
            {
                items.Add(new SelectListItem { Text = "升级", Value = "1" });
                items.Add(new SelectListItem { Text = "回滚", Value = "2" });
                return items;
            }
            if (flag == "scop")
            {
                items.Add(new SelectListItem { Text = "全部", Value = "0" });
                items.Add(new SelectListItem { Text = "按项目", Value = "1" });
                items.Add(new SelectListItem { Text = "按部门", Value = "2" });
                items.Add(new SelectListItem { Text = "按点位", Value = "3" });
                return items;
            } 
            if (flag == "typeName")
            {
                items.Add(new SelectListItem { Text = "M", Value = "M" });
                items.Add(new SelectListItem { Text = "P", Value = "P" });
                return items;
            }
            return items;
        }
        public string GetName(string type, int key)
        {
            var keystr = key.ToString();
            var list = Selectlists(type);
            var name = from lis in list
                       where lis.Value == keystr
                       select lis.Text;
            return name.FirstOrDefault();
        }


        #region 加密
   
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        public  string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        public  string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        #endregion

    }
}