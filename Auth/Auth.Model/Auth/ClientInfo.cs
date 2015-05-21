using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Model
{
    public class ClientInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// 终端信息ID
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// 终端类型
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "终端类型")]
        public string ClientType { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "终端名称")]
        public string ClientName { get; set; }
        /// <summary>
        /// 终端IP
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "终端IP")]
        public string ClientIP { get; set; }

        /// <summary>
        /// 终端PORT
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "终端PORT")]
        public string ClientPort { get; set; } 

        /// <summary>
        /// 终端MAC地址
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "有线MAC")]
        public string ClientMAC { get; set; }

        [MaxLength(50)]
        [Display(Name = "无线MAC")]
        public string ClientWifiMAC { get; set; }
        /// <summary>
        /// 终端设备ID
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "设备标识")]
        public string ClientDeviceID { get; set; }
        /// <summary>
        /// 终端CPU号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "CPU")]
        public string ClientCpu { get; set; }

        /// <summary>
        /// 终端硬盘NO
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "存储空间")]
        public string ClientMemory { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [MaxLength(250)]
        [Display(Name = "所属项目")]
        public string Project { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [MaxLength(250)]
        [Display(Name = "所属部门")]
        public string Department { get; set; }
        /// <summary>
        /// 点位
        /// </summary>
        /// 
        [MaxLength(250)]
        [Display(Name = "所属点位")]
        public string Point { get; set; }

        /// <summary>
        /// 点位名称
        /// </summary>
        /// 
        [MaxLength(250)]
        [Display(Name = "点位名称")]
        public string PointName { get; set; }

        /// <summary>
        /// 客户端版本
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "客户端版本")]
        public string ClientVersion { get; set; }
        /// <summary>
        /// 客户端初始化日期
        /// </summary>
        [Display(Name = "客户端初始化日期")]
        public DateTime? ClientInitDate { get; set; }
        /// <summary>
        /// 客户端状态
        /// </summary>
        [MaxLength(10)]
        [Display(Name = "客户端状态")]
        public string ClientStatus { get; set; }
        /// <summary>
        /// 终端变更标识
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "终端变更标识")]
        public string ClientChangeFlag { get; set; }
        /// <summary>
        /// 更新状态
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "更新状态")]
        public string UpdateStaus { get; set; }
        /// <summary>
        /// 最后心跳时间
        /// </summary>

        [Display(Name = "最后心跳时间")]
        public DateTime? LastHitTime { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        [MaxLength(10)]
        [Display(Name = "在线状态")]
        public string OnlineStatus { get; set; }

        /// <summary>
        /// 通讯密钥
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "通讯密钥")]
        public string CommKeys { get; set; }

    }
}
