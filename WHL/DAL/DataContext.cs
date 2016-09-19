using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.FileDatas;
using WHL.Models.Entity.Stores;
using WHL.Models.Entity.Carriers;
using WHL.Models.Entity.Shipments;
using WHL.Models.Entity.Shipments.Epackets;
using WHL.Models.Entity.MapRules;


namespace WHL.DAL
{
    /// <summary>
    /// Author: Pango
    /// 数据库表链接类：放各个ORM表的注册
    /// </summary>
    public class DataContext : DbContext
    {

        /// <summary>
        /// 用户实体
        /// </summary>
        public DbSet<Client> ClientsDB { get; set; }
        
        /// <summary>
        /// 用户实体
        /// </summary>
        public DbSet<Employee> EmployeesDB { get; set; }
    

        /// <summary>
        /// 产品实体
        /// </summary>
        public DbSet<Product> ProductsDB { get; set; }

        /// <summary>
        /// 库存实体
        /// </summary>
        public DbSet<Inventory> InventoriesDB { get; set; }

        /// <summary>
        /// 库存变更记录实体
        /// </summary>
        public DbSet<InventoryLog> InventoryLogsDB { get; set; }

        /// <summary>
        /// 仓库实体
        /// </summary>
        public DbSet<Store> StoresDB { get; set; }


        /// <summary>
        /// 仓库实体
        /// </summary>
        public DbSet<Delivery> DeliveriesDB { get; set; }


        /// <summary>
        /// 仓库实体
        /// </summary>
        public DbSet<DeliveryItem> DeliveryItemsDB { get; set; }

        /// <summary>
        /// 仓库实体
        /// </summary>
        public DbSet<DeliveryLog> DeliveryLogsDB { get; set; }


        /// <summary>
        /// 导入导出后的文件记录
        /// </summary>
        public DbSet<FileData> FileDatasDB { get; set; }

        /// <summary>
        /// 导入导出规则实体
        /// </summary>
        public DbSet<MapRule> MapRulesDB { get; set; }

        /// <summary>
        /// 导入导出规则的子项实体
        /// </summary>
        public DbSet<MapRuleItem> MapRuleItemsDB { get; set; }

        /// <summary>
        /// 导入导出规则的子项可选择项实体
        /// </summary>
        public DbSet<MapRuleOption> MapRuleOptionsDB { get; set; }

        /// <summary>
        /// 承运商实体
        /// </summary>
        public DbSet<Carrier> CarriersDB { get; set; }

        /// <summary>
        /// 运单货运信息实体
        /// </summary>
        public DbSet<Shipment> ShipmentsDB { get; set; }

        /// <summary>
        /// Epacket 运单信息
        /// </summary>
        public DbSet<EpacketShipData> EpacketShipDatasDB { get; set; }

        /// <summary>
        /// Epacket授权信息实体
        /// </summary>
        public DbSet<EpacketAuth> EpacketAuthsDB { get; set; }


        // 在创建时会执行的方法，用于加强一些绑定的声明
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 因为USER表有自反关系
            modelBuilder.Entity<Employee>().HasRequired(employee => employee.UpdateEmployee).WithMany().HasForeignKey(employee => employee.UpdateEmployeeID);
        }
       
    }
}