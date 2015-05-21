using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Auth.Model
{
    public class BaseVersionInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// 版本编号
        /// </summary>
        public int VersionID { get; set; }

        /// <summary>
        /// 版本名称
        /// </summary>

        public String VersionName { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>

        public bool IsPublish { get; set; }

        /// <summary>
        /// 版本类型
        /// </summary>

        public string VersionType { get; set; }

        /// <summary>
        /// 版本类型
        /// </summary>

        public string FileURL { get; set; }

        /// <summary>
        /// 文件MD5
        /// </summary>
        public string FileMD5 { get; set; }

        /// <summary>
        /// 终端类型
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNo { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatAt { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>

        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary> 
        public DateTime? UpdateAt { get; set; }



    }
}
