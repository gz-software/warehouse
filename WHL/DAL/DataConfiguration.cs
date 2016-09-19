using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;


namespace WHL.DAL
{
    /// <summary>
    /// 数据库链接规则设置类。
    /// </summary>
    public class DataConfiguration : DbConfiguration
    {
        public DataConfiguration()
        {
            
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}