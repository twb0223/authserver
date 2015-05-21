using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Model
{
    public class UpdateTask
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "计划编号")]
        public int TaskID { get; set; }

        [MaxLength(200)]
        [Display(Name = "计划名称")]
        public string TaskName { get; set; }

        [Display(Name = "版本")]
        [Required]
        public int VersionID { get; set; }

        [Display(Name = "版本编号")]
        [MaxLength(50)]
        public string VersionNo { get; set; }

        [Display(Name = "更新方式")]
        public int UpdateType { get; set; }

        [Display(Name = "更新范围")]
        public int UpdateScope { get; set; }

        [Display(Name = "集合")]
        public string IDList { get; set; }

        [Display(Name = "是否立即更新")]
        public int IsExecNow { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

    }
}
