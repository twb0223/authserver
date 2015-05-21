using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.Model;
using System.Data.Entity;

namespace Auth.DataAccess
{
    public class BaseDataContext : DbContext
    {
        private static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BaseDataCS"].ToString();
        public DbSet<BaseClientInfo> BaseClientInfos { get; set; }


        public DbSet<BasePoint> BasePoints { get; set; }

        public DbSet<BaseVersionInfo> BaseVersionInfos { get; set; }

        public BaseDataContext()
            : base(ConnectionString)
        {
            //延迟加载
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.AutoDetectChangesEnabled = true;  //自动监测变化，默认值为 true  
        }
        static BaseDataContext()
        {
            //EF不修改数据库
            Database.SetInitializer<BaseDataContext>(null);
        }
    }
}
