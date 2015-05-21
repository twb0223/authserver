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
    public class VersionInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// 版本编号
        /// </summary>
        [Display(Name = "版本ID")]
        public int VersionID { get; set; }

        /// <summary>
        /// 版本名称
        /// </summary>
        [Display(Name = "版本名称")]
        public string Name { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [Display(Name = "是否发布")]
        public int IsPublish { get; set; }


        /// <summary>
        /// 终端类型
        /// </summary>
        [Display(Name = "终端类型")]
        [Required(ErrorMessage = "终端类型不能为空")]
        public int ClientType { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "版本号")]
        [Required(ErrorMessage = "版本号不能为空")]
        public string VersionNo { get; set; }




        /// <summary>
        ///文件路径
        /// </summary>
        [MaxLength(300)]
        [Display(Name = "文件路径")]
        public string FielPath { get; set; }


        /// <summary>
        /// 版本MD5
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "文件MD5")]
        public string MD5 { get; set; }

        /// <summary>
        /// 版本描述
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "版本描述")]
        public string Description { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "创建人")]
        public string CreatBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime? CreatAt { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [Display(Name = "更新人")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary> 
        [Display(Name = "更新时间")]
        public DateTime? UpdateAt { get; set; }

        [Display(Name = "对应终端组件")]
        [Required(ErrorMessage = "对应终端组件不能为空")]
        public string ClientTypeName { get; set; }
    }
}
