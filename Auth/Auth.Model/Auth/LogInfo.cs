using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Model
{
    public class LogInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Display(Name = "日志编号")]
        public int LogID { get; set; }

        [Display(Name = "记录日期")]
        public DateTime Log_date { get; set; }

        [Display(Name = "线程")]
        [MaxLength(255)]
        public string Thread { get; set; }

        [Display(Name = "日志级别")]
        [MaxLength(50)]
        public string Log_level { get; set; }

        [Display(Name = "记录器")]
        [MaxLength(255)]
        public string Logger { get; set; }

        [Display(Name = "日志信息")]
        [MaxLength(400)]
        public string Message { get; set; }

        [Display(Name = "异常信息")]
        [MaxLength(4000)]
        public string Exception { get; set; }


    }
}
