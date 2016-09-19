using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using WHL.DAL;
using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.Deliveries;


namespace WHL.Services
{
    /// <summary>
    /// This class put all biz logic in services.
    /// Author: Pango
    /// </summary>
    public class BaseService
    {
        public const int SUCCESS = 1;
        public const int ERROR = 0;

        


        /// <summary>
        /// the employee from controller set in, which should update the db as current operator
        /// </summary>
        public Employee CurrentEmployee { get; set; }

        /// <summary>
        /// return the db now date 
        /// </summary>
        public DateTime CurrentUpdateDate {
            get { return new DateTime(DateTime.Now.Ticks);}
        
        }

        // init the database
        protected DataContext db = new DataContext();


        // dispose the database
        public void Dispose(){
           db.Dispose();
           
        }


        #region Log
        /// <summary>
        /// Save the delivery change history log into deliveryLog
        /// </summary>
        /// <param name="db">current db</param>
        /// <param name="status1">from status</param>
        /// <param name="status2">to status</param>
        /// <returns>saved delivery log id</returns>
        public int LogDeliveryChange(DataContext db, int deliveryID, int status1, int status2)
        {
            DeliveryLog dyLog = new DeliveryLog();
            dyLog.DeliveryID = deliveryID;
            dyLog.Status1 = status1;
            dyLog.Status2 = status2;
            dyLog.UpdateDate = CurrentUpdateDate;
            dyLog.UpdateEmployeeID = CurrentEmployee.ID;
            db.DeliveryLogsDB.Add(dyLog);
            db.SaveChanges();
            return dyLog.ID;
        }

        /// <summary>
        /// Save the inventory change history log into invetoryLog
        /// </summary>
        /// <param name="db">current db</param>
        /// <param name="deliveryLogID">deliveryLOGID cause this change</param>
        /// <param name="inv1">From inventory</param>
        /// <param name="inv2">To inventory</param>
        /// <returns></returns>
        public int LogInventoryChange(DataContext db, int deliveryID, int deliveryLogID, Inventory inv1, Inventory inv2)
        {
            InventoryLog invLog = new InventoryLog(inv1, inv2);
            invLog.InventoryID = deliveryID;
            invLog.DeliveryLogID = deliveryLogID;
            invLog.ChangeType = (int)InventoryLogTypeEnum.Delivery;
            invLog.UpdateDate = CurrentUpdateDate;
            invLog.UpdateEmployeeID = CurrentEmployee.ID;
            db.InventoryLogsDB.Add(invLog);
            db.SaveChanges();
            return invLog.ID;
        }
        #endregion

        #region Tools
        /// <summary>
        /// Generate delivery number : D + yyyymmddhhmmss + random number
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns>return like 'D00012016080106542202'</returns>
        public String GenerateNumber(int clientID)
        {

            string prefixStr = "D"; // prefix
            string clientStr = clientID.ToString().PadLeft(3, '0');
            string nowDateStr = DateTime.Now.ToString("yyyyMMddhhmmss");//获取当前时间  
            string ramStr = (new Random().Next(0, 99)).ToString().PadLeft(2, '0'); // ram number(0~100) , add zero to left

            string numberStr = prefixStr + clientStr + nowDateStr + ramStr;
            return numberStr;

        }

        #endregion

        /// <summary>
        /// Get the inventory by specified store.id and sku
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="sku"></param>
        /// <returns></returns>
        public Inventory GetInventory(int storeID, string sku) {
            Inventory inv = null;
            List<Inventory> invList = db.InventoriesDB.Where(i => i.StoreID == storeID).Where(i => i.Product.SKU == sku).ToList();
            if (invList.Count > 0) {
                inv =  invList.First();
            }
            return inv;
        }

        /// <summary>
        /// Get the inventory by specified store.id and product.id
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="sku"></param>
        /// <returns></returns>
        public Inventory GetInventory(int storeID, int productID)
        {
            Inventory inv = null;
            List<Inventory> invList = db.InventoriesDB.Where(i => i.StoreID == storeID).Where(i => i.Product.ID == productID).ToList();
            if (invList.Count > 0)
            {
                inv = invList.First();
            }
            return inv;
        }


    }
}