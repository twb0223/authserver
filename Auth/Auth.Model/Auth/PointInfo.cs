using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Model
{
    public class PointInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "点位ID")]
        public int PointInfoID { get; set; }
        [Display(Name = "点位编号")]
        public string PointNo { get; set; }

        [Display(Name = "点位名称")]
        public string PointName { get; set; }

        [Required]
        [Display(Name = "所属项目")]
        public string ProjectInfoID { get; set; }

        [Required]
        [Display(Name = "所属部门")]
        public string DepartInfoID { get; set; }
        [Display(Name = "是否分配")]
        public string IsHaveClient { get; set; }
    }
}
