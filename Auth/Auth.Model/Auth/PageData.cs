using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Model
{/// <summary>
    /// 分页数据集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T>
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 数据集
        /// </summary>
        public IList<T> DataList { get; set; }
    }
}
