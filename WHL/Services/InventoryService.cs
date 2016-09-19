using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.IO;
using System.Data;

using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.FileDatas;
using WHL.Models.Entity.Stores;
using WHL.Models.Entity.MapRules;


namespace WHL.Services
{
    /// <summary>
    /// InventoryService：we put all inventory biz logic handing here
    /// Author: Pango
    /// </summary>
    public class InventoryService : BaseService
    {
        #region List
        /// <summary>
        /// Get Inventory LINQ Query List
        /// Notice: it will retun a LINQ Query only, not the real data, you should call "toList()" to get the real data.
        /// </summary>
        /// <param name="queryInventory">Query Inventory Object</param>
        /// <returns>LINQ Query List，you should call "toList()" to get the real data.</returns>
        public IQueryable<Inventory> GetInventoryQueryList(Inventory queryInventory)
        {
            var inventoryQueryList = from s in db.InventoriesDB select s;

            if (queryInventory != null)
            {   // 我只写了一小部分的查询，要补完
                if ((queryInventory.Product != null) && (!String.IsNullOrEmpty(queryInventory.Product.ShortTitle)))
                {
                    inventoryQueryList = inventoryQueryList.Where(i => i.Product.ShortTitle.ToUpper().Contains(queryInventory.Product.ShortTitle.ToUpper()));
                }
                if ((queryInventory.Product != null) && (!String.IsNullOrEmpty(queryInventory.Product.SKU)))
                {
                    inventoryQueryList = inventoryQueryList.Where(i => i.Product.SKU.ToUpper().Contains(queryInventory.Product.SKU.ToUpper()));
                }
                if (queryInventory.QueryAvail1 > 0)
                {
                    inventoryQueryList = inventoryQueryList.Where(i => i.Avail >= queryInventory.QueryAvail1);
                }

                if (queryInventory.QueryAvail2 > 0)
                {
                    inventoryQueryList = inventoryQueryList.Where(i => i.Avail <= queryInventory.QueryAvail2);
                }

                if (queryInventory.QueryUpdateDate1 != null && queryInventory.QueryUpdateDate1 != Convert.ToDateTime("0001/1/1 0:00:00"))
                {
                    inventoryQueryList = inventoryQueryList.Where(i => i.UpdateDate >= queryInventory.QueryUpdateDate1);
                }

                if (queryInventory.QueryUpdateDate2 != null && queryInventory.QueryUpdateDate2 != Convert.ToDateTime("0001/1/1 0:00:00"))
                {
                    string UpdateTime2 = Convert.ToString(queryInventory.QueryUpdateDate2).Replace("0:00:00", "23:59:59");
                    DateTime searchUpdateTime = Convert.ToDateTime(UpdateTime2);
                    inventoryQueryList = inventoryQueryList.Where(i => i.UpdateDate <= searchUpdateTime);
                }


                if ((queryInventory.StoreID > 0))
                {
                    inventoryQueryList = inventoryQueryList.Where(i => i.StoreID == queryInventory.StoreID);
                }
            }
            return inventoryQueryList;
        }


        /// <summary>
        /// Get the inventory result in jquery data table format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Inventory Object</param>
        /// <returns>the DataTable Result with paging and sorting</returns>
        public DTResult<Inventory> GetInventoryDataTableResult(DTParams dtParams, Inventory queryInventory)
        {
            var queryList = GetInventoryQueryList(queryInventory);

            int count = queryList.Count();

            var data = new List<Inventory>();

            string sortOrder = "";

            if ((dtParams == null) || (dtParams.SortOrder == null))
            {   // 如果不是从界面进来的，是接口来的，就没有dtParams
                dtParams.Start = 0;
                dtParams.Length = count;
                dtParams.Order = null;
                sortOrder = "ID";
            }
            else
            {
                for (int i = 0; i < dtParams.Order.Length; i++)
                {
                    var order = dtParams.Order[i].Column;
                    var sort = dtParams.Order[i].Dir;
                    var thenByStr = dtParams.Columns[order].Data.Replace("Layout", "");
                    sortOrder += thenByStr + " " + sort + ",";
                }

                sortOrder = sortOrder.Substring(0, sortOrder.Length - 1);

            }

            data = queryList.OrderBy(sortOrder).Skip(dtParams.Start).Take(dtParams.Length).ToList();

            DTResult<Inventory> result = new DTResult<Inventory>
            {
                flag = SUCCESS,             // return call flag
                message = "Call Success",   // return call message
                draw = dtParams.Draw,       // how many time it have been call for this method
                data = data,                // the data of datatable
                recordsFiltered = count,    // records filter count
                recordsTotal = count        // total records count
            };

            return result;
        }

        #endregion

        #region Export and Import
        /// <summary>
        /// Export a excel stream by the inventory query.
        /// </summary>
        /// <param name="queryInventory">the query of inventory</param>
        /// <param name="submitMapRule">the map rule submit</param>
        /// <returns>Excel memory stream</returns>
        public JsonNetPackResult Export(FileData submitFileData, Inventory queryInventory)
        {
            string message = LangHelper.Get("Inventory_Result_Success_ExportSuccess");
            string downloadPath = "";                       // the return donwload path
            int totalCount = 0;                             // the data count in database
            int successCount = 0;                           // the success exported row count
            int duration = 0;                               // the process duration
            int status = (int)FileStatusEnum.Success;       // the export status

            // 把用户定义的EXCEL文件名拼后缀为完成名： export_20160905030507 + .xlsx
            submitFileData.FileName = FileData.GetFullExportFileName(submitFileData.FileType, submitFileData.FileName);

            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内
                    var inventoryQueryList = GetInventoryQueryList(queryInventory);
                    List<Object> inventoryDataList = inventoryQueryList.ToList<Object>();
                    totalCount = inventoryDataList.Count;

                    List<MapRuleItem> submitMapRuleItemList = submitFileData.MapRule.RuleItemList.ToList();// map rule item list.

                    MemoryStream ms = ExcelHelper.GetExportMemoryStream(typeof(Inventory), inventoryDataList, submitFileData, out successCount);
                    downloadPath = GenerateExportFile(ms, submitFileData.FileName);    // get the down load path by store the stream in server file

                    submitFileData.ProcessType = (int)FileProcessTypeEnum.All;
                    submitFileData.FileName = submitFileData.FileName;
                    submitFileData.Status = status;
                    submitFileData.TotalCount = totalCount;
                    submitFileData.SuccessCount = successCount;
                    submitFileData.Duration = duration;
                    submitFileData.UpdateEmployeeID = CurrentEmployee.ID;
                    submitFileData.UpdateDate = CurrentUpdateDate;
                    db.FileDatasDB.Add(submitFileData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, submitFileData);
            }

            message = String.Format(message, totalCount, successCount);
            return new JsonNetPackResult(SUCCESS, message, submitFileData);

        }



        /// <summary>
        /// Read the Excel File and get the data list , import into our database
        /// </summary>
        /// <param name="submitFileData">submitFileData from UI</param>
        /// <returns>Json net pack result: containing success or error, and the imported row total counts </returns>
        public JsonNetPackResult Import(FileData submitFileData, int storeID)
        {
            string message = LangHelper.Get("Inventory_Result_Success_ImportSuccess");
            int totalCount = 0;                                 // the row count in imported excel
            int successCount = 0;                               // the success count update db
            int duration = 0;                                   // the process duration
            int status = (int)FileStatusEnum.Success;           // the import status


            // 检查 storeID是否有：必须要选择一个Store进行导入
            if (storeID < 1)
            {
                return new JsonNetPackResult(ERROR, LangHelper.Get("Inventory_Result_Error_ImportStoreNoSelect"), 0);
            }

            // 检查文件后缀名是否正确：用FileType与当前上传文件后缀名对比！
            string suffix = submitFileData.UploadFile.FileName.Substring(submitFileData.UploadFile.FileName.LastIndexOf(".")).ToLower();//获取后缀
            if ((submitFileData.FileType == (int)FileTypeEnum.XLS) && (!suffix.ToLower().Equals(".xls")))
            { //2003
                // not a excel 2003 format file, it is not.xls file : 上传文件不是.xls Excel 2003 文件
                return new JsonNetPackResult(ERROR, LangHelper.Get("FileData_Result_Error_NotXLS"), 0);
            }

            if ((submitFileData.FileType == (int)FileTypeEnum.XLSX) && (!suffix.ToLower().Equals(".xlsx")))
            { //2007
                // not a excel 2007 format file, it is not.xlsx file : 上传文件不是.xlsx Excel 2007 文件
                return new JsonNetPackResult(ERROR, LangHelper.Get("FileData_Result_Error_NotXLSX"), 0);
            }

            if ((submitFileData.FileType == (int)FileTypeEnum.CSV) && (!suffix.ToLower().Equals(".csv")))
            { //csv
                // not a excel 2007 format file, it is not.xlsx file : 上传文件不是.csv文件
                return new JsonNetPackResult(ERROR, LangHelper.Get("FileData_Result_Error_NotCSV"), 0);
            }

            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内

                    List<MapRuleItem> submitMapRuleItemList = submitFileData.MapRule.RuleItemList.ToList();
                    List<Object> importInventoryList = ExcelHelper.GetImportDataList(typeof(Inventory), submitFileData, out totalCount);

                    // the store you will import.
                    Store effStore = db.StoresDB.Find(storeID);

                    foreach (object obj in importInventoryList)
                    {
                        // Lion: 这里处理到数据库
                        Inventory inv = (Inventory)obj;
                        int existProduct = db.ProductsDB.Count(p => p.SKU == inv.Product.SKU);

                        if (existProduct == 0)//product.sku not exist
                        {
                            string msg = LangHelper.Get("Inventory_Result_Error_ImportSkuNotExist");
                            msg = string.Format(msg, inv.Product.SKU);
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }


                        Product product = db.ProductsDB.Where(p => p.SKU == inv.Product.SKU).First();
                        int existInv = db.InventoriesDB.Where(i => i.StoreID == storeID).Count(i => i.ProductID == product.ID);


                        if (existInv == 0)//  Inventory not exist the product insert new  inv
                        {
                            if ((submitFileData.ProcessType == (int)FileProcessTypeEnum.All) ||
                                (submitFileData.ProcessType == (int)FileProcessTypeEnum.NewOnly))
                            { // allow add!

                                Inventory newInv = new Inventory();
                                newInv.ProductID = product.ID;
                                newInv.StoreID = storeID;
                                newInv.Avail = inv.Avail;
                                newInv.OnOut = inv.OnOut;
                                newInv.OnIn = inv.OnIn;
                                newInv.InSum = inv.InSum;
                                newInv.OutSum = inv.OutSum;
                                newInv.ShipMiss = inv.ShipMiss;
                                newInv.OnHold = inv.OnHold;
                                newInv.DiffSum = inv.DiffSum;
                                newInv.UpdateEmployeeID = CurrentEmployee.ID;
                                newInv.UpdateDate = CurrentUpdateDate;
                                successCount++;
                                db.InventoriesDB.Add(newInv);
                            }

                        }
                        else // Inventory  exist the product  update this 
                        {
                            if ((submitFileData.ProcessType == (int)FileProcessTypeEnum.All) ||
                               (submitFileData.ProcessType == (int)FileProcessTypeEnum.UpdateOnly))
                            { // allow update!

                                Inventory getInv = db.InventoriesDB.Where(i => i.StoreID == storeID).Where(i => i.ProductID == product.ID).First();
                                getInv.Avail += (inv.Avail == null ? 0 : inv.Avail);
                                getInv.OnOut += (inv.OnOut == null ? 0 : inv.OnOut);
                                getInv.OnIn += (inv.OnIn == null ? 0 : inv.OnIn);
                                getInv.InSum += (inv.InSum == null ? 0 : inv.InSum);
                                getInv.OutSum += (inv.OutSum == null ? 0 : inv.OutSum);
                                getInv.ShipMiss += (inv.ShipMiss == null ? 0 : inv.ShipMiss);
                                getInv.OnHold += (inv.OnHold == null ? 0 : inv.OnHold);
                                getInv.DiffSum += (inv.DiffSum == null ? 0 : inv.DiffSum);
                                getInv.UpdateEmployeeID = CurrentEmployee.ID;
                                getInv.UpdateDate = CurrentUpdateDate;
                                successCount++;
                            }
                        }

                    }


                    submitFileData.FileName = submitFileData.UploadFile.FileName;
                    submitFileData.Status = status;
                    submitFileData.TotalCount = totalCount;
                    submitFileData.SuccessCount = successCount;
                    submitFileData.Duration = duration;
                    submitFileData.UpdateEmployeeID = CurrentEmployee.ID;
                    submitFileData.UpdateDate = CurrentUpdateDate;
                    submitFileData.UploadFile = null; // 把上传文件清空，避免返回数据量过大！
                    db.FileDatasDB.Add(submitFileData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, submitFileData);
            }


            message = String.Format(message, totalCount, successCount);
            return new JsonNetPackResult(SUCCESS, message, submitFileData);
        }


        /// <summary>
        /// generate the excel export file path for user download it.
        /// </summary>
        /// <param name="ms">EXCEL MemoryStream</param>
        /// <param name="filePrefixName">the export file prefix name, ex: inventory_20160817,delivery_20160803..</param>
        /// <returns>web file path： such as : /Temp/inventory_20160831105552.xls</returns>
        public String GenerateExportFile(MemoryStream ms, String fileFullName)
        {
            string folderName = CommonHelper.USER_FILE_FOLDER;                              //  svn请豁免掉这个目录不要提交，属于临时目录
            string localFolderPath = AppDomain.CurrentDomain.BaseDirectory + folderName;    //  本地物理路径

            if (!Directory.Exists(localFolderPath))
            { // Create the directory it does not exist.
                Directory.CreateDirectory(localFolderPath);
            }
            string localFilePath = localFolderPath + "\\" + fileFullName;        // 物理路径
            string webFilePath = "/" + folderName + "/" + fileFullName;            // 网站相对路径，比如（前面的部分自己加）： http://localhost:6659/FILES/inventory_20160831105552.xls
            CommonHelper.SaveFile(ms, localFilePath); // 这个方式我没试过，你试通它
            return webFilePath;
        }
        #endregion
    }


}