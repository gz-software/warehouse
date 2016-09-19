using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;

using WHL.DAL;
using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Stores;



namespace WHL.Services
{
    /// <summary>
    /// Deliver Service：we put all deliver biz logic handing here~
    /// Author: Pango
    /// </summary>
    public class DeliveryService : BaseService
    {

        #region List
        /// <summary>
        /// Get Delivery LINQ Query List
        /// Notice: it will retun a LINQ Query only, not the real data, you should call "toList()" to get the real data.
        /// </summary>
        /// <param name="queryInventory">Query Delivery Object</param>
        /// <returns>LINQ Query List，you should call "toList()" to get the real data.</returns>
        public IQueryable<Delivery> GetDeliveryQueryList(Delivery queryDelivery)
        {

            var deliveryQueryList = from s in db.DeliveriesDB select s;

            if (queryDelivery != null)
            {
                if (queryDelivery.ClientID > 0)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.ClientID == queryDelivery.ClientID);
                }
                if (queryDelivery.QueryStatus1 > 0)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.Status >= queryDelivery.QueryStatus1);
                }
                if (queryDelivery.QueryStatus2 > 0)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.Status <= queryDelivery.QueryStatus2);
                }
                if (queryDelivery.QueryStatus1 >= (int)DeliveryStatusEnum.New && queryDelivery.QueryStatus2 <= (int)DeliveryStatusEnum.Delivering)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.DeliveryType != (int)DeliveryTypeEnum.PurchaseIn);
                }
                if (queryDelivery.QueryStatus1 >= (int)DeliveryStatusEnum.Delivering && queryDelivery.QueryStatus2 <= (int)DeliveryStatusEnum.Success)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.DeliveryType != (int)DeliveryTypeEnum.SellingOut);
                }
                if (queryDelivery.SrcStoreID > 0)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.SrcStoreID == queryDelivery.SrcStoreID);
                }

                if (queryDelivery.TarStoreID > 0)
                {
                    deliveryQueryList = deliveryQueryList.Where(i => i.TarStoreID == queryDelivery.TarStoreID);
                }
            }
            return deliveryQueryList;
        }

        /// <summary>
        /// Get the delivery result in jquery data table format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Delivery Object</param>
        /// <returns>the DataTable Result with paging and sorting</returns>
        public DTResult<Delivery> GetDeliveryDataTableResult(DTParams dtParams, Delivery queryDelivery)
        {
            var queryList = GetDeliveryQueryList(queryDelivery);

            int count = queryList.Count();

            var data = new List<Delivery>();


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

            DTResult<Delivery> result = new DTResult<Delivery>
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

        #region Outbound

        /// <summary>
        /// Submit the outbound delivery draft,including create and update
        /// Process:
        /// 1. if update case: update db delivery by submit delivery, remove the old db out items, add new out items from submit. and update the time and operator
        /// 2. if add case: add new delivery, generate new DON, save and add new out items from submit,. with the time and operator
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult OutboundSubmitDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_SubmitOutboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                if (submitDelivery.ID > 0)
                {  // 1. if update case: update db delivery by form delivery,remove the old db out items, add new out items from submit. and update the time and operator

                    using (db)
                    { // transation 习惯使用using以保证数据操作在事务内
                        // update exist delivery by submit delivery
                        Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                        Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);

                        if (!bol)
                        {
                            string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                        existDelivery.CarrierID = submitDelivery.CarrierID;
                        existDelivery.SrcStoreID = submitDelivery.SrcStoreID;
                        existDelivery.TarStoreID = submitDelivery.TarStoreID;
                        existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                        existDelivery.UpdateDate = CurrentUpdateDate;
                        existDelivery.ClientID = submitDelivery.ClientID = CurrentEmployee.ClientID;
                        existDelivery.Version += 1;

                        if (existDelivery.SrcStoreID == existDelivery.TarStoreID)
                        {
                            String msg = LangHelper.Get("Delivery_Result_Error_SrcStoreCannotEqualTarStore");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                        if (existDelivery.TarStoreID == null)
                        {
                            existDelivery.DeliveryType = (int)DeliveryTypeEnum.SellingOut;
                        }
                        else
                        {
                            existDelivery.DeliveryType = (int)DeliveryTypeEnum.Transfer;
                        }

                        // remove old delivery items
                        if (existDelivery.DeliveryItemList != null)
                        {
                            db.DeliveryItemsDB.RemoveRange(existDelivery.DeliveryItemList);

                        }

                        int dyLogID = LogDeliveryChange(db, existDelivery.ID, (int)DeliveryStatusEnum.Submit, (int)DeliveryStatusEnum.Submit);

                        // add new delivery items
                        if (submitDelivery.DeliveryItemList != null)
                        {
                            foreach (DeliveryItem d in submitDelivery.DeliveryItemList)
                            {
                                d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                                d.Product = null;
                                d.UpdateEmployee = null;
                                d.UpdateEmployeeID = CurrentEmployee.ID;
                                d.UpdateDate = CurrentUpdateDate;
                            }
                            db.DeliveryItemsDB.AddRange(submitDelivery.DeliveryItemList);
                        }
                        db.SaveChanges();
                        returnDeliveryID = existDelivery.ID;
                    }
                }
                else
                { // 2. if add case: add new delivery, generate new DON, save and add new out items from submit,. with the time and operator
                    using (db)
                    { // transation 习惯使用using以保证数据操作在事务内

                        // save new delivery
                        Delivery newDelivery = new Delivery();

                        newDelivery.CarrierID = submitDelivery.CarrierID;
                        newDelivery.SrcStoreID = submitDelivery.SrcStoreID;
                        newDelivery.TarStoreID = submitDelivery.TarStoreID;

                        if (newDelivery.SrcStoreID == newDelivery.TarStoreID)
                        {
                            String msg = LangHelper.Get("Delivery_Result_Error_SrcStoreCannotEqualTarStore");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                        newDelivery.Version = 1;
                        newDelivery.DeliveryType = submitDelivery.DeliveryType;
                        newDelivery.ClientID = CurrentEmployee.ClientID;
                        newDelivery.Status = (int)DeliveryStatusEnum.Submit;
                        newDelivery.DON = GenerateNumber(submitDelivery.ClientID);
                        newDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                        newDelivery.UpdateDate = CurrentUpdateDate;



                        db.DeliveriesDB.Add(newDelivery);
                        db.SaveChanges();
                        int dyLogID = LogDeliveryChange(db, newDelivery.ID, (int)DeliveryStatusEnum.Submit, (int)DeliveryStatusEnum.Submit);

                        // save new delivery item list
                        if (submitDelivery.DeliveryItemList != null)
                        {
                            foreach (DeliveryItem d in submitDelivery.DeliveryItemList)
                            {
                                d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                                d.Product = null;
                                d.UpdateEmployee = null;
                                d.UpdateEmployeeID = CurrentEmployee.ID;
                                d.UpdateDate = CurrentUpdateDate;
                                d.DeliveryID = newDelivery.ID;
                            }
                            db.DeliveryItemsDB.AddRange(submitDelivery.DeliveryItemList);
                        }

                        db.SaveChanges();
                        returnDeliveryID = newDelivery.ID;
                    }
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }

            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }

        /// <summary>
        /// Delete an outbound submited delivery.
        /// Processs:
        /// 1. delete the outbound delivery and its items
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult OutboundDeleteDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_DeleteOutboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内

                    Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                    Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);

                    if (!bol)
                    {
                        string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                        return new JsonNetPackResult(ERROR, msg, 0);
                    }
                    db.DeliveriesDB.Remove(existDelivery);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }
            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }


        /// <summary>
        /// Process an submited outbound delivery.
        /// Processs:
        /// 1. delivery update: status from [submit] to [process] , and update the updateDate and operator
        /// 2. source storage update: [onHold+] [avail-], and update the updateDate and operator
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult OutboundProcessDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_ProcessOutboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内

                    // 1. delivery update: status from [draft] to [processing] , and update the updateDate and operator
                    Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                    Store srcStore = db.StoresDB.Find(existDelivery.SrcStoreID);
                    Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);

                    if (!bol)
                    {
                        string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                        return new JsonNetPackResult(ERROR, msg, 0);
                    }
                    existDelivery.Status = (int)DeliveryStatusEnum.Processing;
                    existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                    existDelivery.UpdateDate = CurrentUpdateDate;
                    existDelivery.Version += 1;

                    // remove old delivery items
                    if (existDelivery.DeliveryItemList != null)
                    {
                        db.DeliveryItemsDB.RemoveRange(existDelivery.DeliveryItemList);

                    }

                    int dyLogID = LogDeliveryChange(db, existDelivery.ID, (int)DeliveryStatusEnum.Submit, (int)DeliveryStatusEnum.Processing);

                    // 2. source storage update: [onHold+] [avail-], and update the updateDate and operator
                    foreach (DeliveryItem outItem in submitDelivery.DeliveryItemList)
                    {


                        outItem.UpdateDate = new DateTime(DateTime.Now.Ticks);
                        outItem.Product = null;
                        outItem.UpdateEmployee = null;
                        outItem.UpdateEmployeeID = CurrentEmployee.ID;
                        outItem.UpdateDate = CurrentUpdateDate;


                        if (srcStore != null)
                        {
                            Inventory srcInv = GetInventory((int)submitDelivery.SrcStoreID, (int)outItem.ProductID);
                            Inventory srcInv1 = (Inventory)srcInv.Clone();

                            if (srcInv == null)
                            { // check if inventory exist~
                                String msg = string.Format(LangHelper.Get("Product_Empty"), outItem.Product.SKU, srcStore.Name);
                                return new JsonNetPackResult(ERROR, msg, 0);
                            }

                            srcInv.Avail -= outItem.OutQty;
                            if (srcInv.Avail < 0)
                            {

                                String msg = string.Format(LangHelper.Get("Delivery_Avail_Not_Enough"), srcStore.Name, outItem.OutQty, srcInv.Avail);
                                return new JsonNetPackResult(ERROR, msg, 0);
                            }
                            srcInv.OnHold += outItem.OutQty;
                            srcInv.UpdateEmployeeID = CurrentEmployee.ID;
                            srcInv.UpdateDate = CurrentUpdateDate;

                            Inventory srcInv2 = (Inventory)srcInv.Clone();
                            int invLogID = LogInventoryChange(db, srcInv.ID, dyLogID, srcInv1, srcInv2);
                        }
                        else
                        {
                            String msg = LangHelper.Get("Delivery_Result_Error_SrcStore_Empty");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                    }
                    db.DeliveryItemsDB.AddRange(submitDelivery.DeliveryItemList);
                    db.SaveChanges();
                    returnDeliveryID = existDelivery.ID;

                }

            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }
            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }


        /// <summary>
        /// Cancel an outbound processing delivery.
        /// Processs:
        /// 1. delivery update: status from [process] to [submit](rollback the process step) , and update the updateDate and operator
        /// 2. source storage update: [onHold-] [avail+], and update the updateDate and operator
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult OutboundCancelDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_CancelOutboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内

                    // 1. delivery update: status from [draft] to [processing] , and update the updateDate and operator
                    Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                    Store srcStore = db.StoresDB.Find(submitDelivery.SrcStoreID);
                    Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);

                    if (!bol)
                    {
                        string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                        return new JsonNetPackResult(ERROR, msg, 0);
                    }
                    existDelivery.Status = (int)DeliveryStatusEnum.Submit;
                    existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                    existDelivery.UpdateDate = CurrentUpdateDate;
                    existDelivery.Version += 1;

                    int dyLogID = LogDeliveryChange(db, existDelivery.ID, (int)DeliveryStatusEnum.Processing, (int)DeliveryStatusEnum.Submit);

                    // 2. source storage update: [onHold+] [avail-], and update the updateDate and operator
                    foreach (DeliveryItem outItem in submitDelivery.DeliveryItemList)
                    {
                        Inventory srcInv = GetInventory((int)srcStore.ID, (int)outItem.ProductID);
                        Inventory srcInv1 = (Inventory)srcInv.Clone();

                        if (srcInv == null)
                        { // check if inventory exist~

                            String msg = string.Format(LangHelper.Get("Product_Empty"), outItem.Product.SKU, srcStore.Name);
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                        srcInv.Avail += outItem.OutQty;
                        srcInv.OnHold -= outItem.OutQty;
                        srcInv.UpdateEmployeeID = CurrentEmployee.ID;
                        srcInv.UpdateDate = CurrentUpdateDate;

                        Inventory srcInv2 = (Inventory)srcInv.Clone();

                        int invLogID = LogInventoryChange(db, srcInv.ID, dyLogID, srcInv1, srcInv2);
                    }
                    db.SaveChanges();
                    returnDeliveryID = existDelivery.ID;
                }

            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }
            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }


        /// <summary>
        /// Ship and outbound processing delivery
        /// Processs:
        /// 1. delivery update: status from [processing] to [delivering] , and update the updateDate and operator
        /// 2. source storage update: [onHold-][onOut+][outSum+], and update the updateDate and operator
        /// 3. target storage update: [onIn+] , and update the updateDate and operator
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult OutboundShipDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_ConfirmOutboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内

                    // 1. delivery update: status from [draft] to [processing] , and update the updateDate and operator
                    Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                    Store srcStore = db.StoresDB.Find(submitDelivery.SrcStoreID);
                    Store tarStore = db.StoresDB.Find(submitDelivery.TarStoreID);
                    Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);

                    if (!bol)
                    {
                        string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                        return new JsonNetPackResult(ERROR, msg, 0);
                    }
                    existDelivery.Status = (int)DeliveryStatusEnum.Delivering;
                    existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                    existDelivery.UpdateDate = CurrentUpdateDate;
                    existDelivery.Version += 1;

                    int dyLogID = LogDeliveryChange(db, existDelivery.ID, (int)DeliveryStatusEnum.Processing, (int)DeliveryStatusEnum.Delivering);

                    // 2. source storage update: [onHold-][onOut+][outSum+], and update the updateDate and operator
                    foreach (DeliveryItem outItem in submitDelivery.DeliveryItemList)
                    {
                        Inventory srcInv = GetInventory((int)srcStore.ID, (int)outItem.ProductID);
                        Inventory srcInv1 = (Inventory)srcInv.Clone();

                        if (srcInv == null)
                        { // check if inventory exist~

                            String msg = string.Format(LangHelper.Get("Product_Empty"), outItem.Product.SKU, srcStore.Name);
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                        srcInv.OnHold -= outItem.OutQty;
                        if (srcInv.OnHold < 0)
                        {

                            String msg = string.Format(LangHelper.Get("Delivery_Avail_Not_Enough"), srcStore.Name, outItem.OutQty, srcInv.OnHold);
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }
                        srcInv.OnOut += outItem.OutQty;
                        srcInv.OutSum += outItem.OutQty;
                        srcInv.UpdateEmployeeID = CurrentEmployee.ID;
                        srcInv.UpdateDate = CurrentUpdateDate;


                        Inventory srcInv2 = (Inventory)srcInv.Clone();
                        int invLogID = LogInventoryChange(db, srcInv.ID, dyLogID, srcInv1, srcInv2);
                    }

                    /// 3. target storage update: [onIn+] , and update the updateDate and operator
                    foreach (DeliveryItem outItem in submitDelivery.DeliveryItemList)
                    {
                        Inventory tarInv = GetInventory((int)tarStore.ID, (int)outItem.ProductID);
                        Inventory tarInv1 = (Inventory)tarInv.Clone();

                        if (tarInv == null)
                        { // check if inventory exist~,if not exist add one~(Lion)
                            String msg = string.Format(LangHelper.Get("Product_Empty"), outItem.Product.SKU, tarStore.Name);
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }


                        tarInv.OnIn += outItem.OutQty;

                        tarInv.UpdateEmployeeID = CurrentEmployee.ID;
                        tarInv.UpdateDate = CurrentUpdateDate;


                        Inventory tarInv2 = (Inventory)tarInv.Clone();
                        int invLogID = LogInventoryChange(db, tarInv.ID, dyLogID, tarInv1, tarInv2);
                    }


                    db.SaveChanges();
                    returnDeliveryID = existDelivery.ID;
                }

            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }
            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }

        #endregion

        #region Inbound

        /// <summary>
        /// Submit the inbound delivery draft,including create and update
        /// Process:
        /// 1. if update case: update db delivery by submit delivery, remove the old db out items, add new out items from submit. and update the time and operator
        /// 2. if add case: add new delivery, generate new DON, save and add new out items from submit,. with the time and operator
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult InboundSubmitDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_SubmitInboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                if (submitDelivery.ID > 0)
                {  // 1. if update case: update db delivery by form delivery,remove the old db out items, add new out items from submit. and update the time and operator

                    using (db)
                    { // transation 习惯使用using以保证数据操作在事务内
                        // update exist delivery by submit delivery
                        Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                        Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);

                        if (!bol)
                        {
                            string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }
                        existDelivery.CarrierID = submitDelivery.CarrierID;
                        existDelivery.SrcStoreID = submitDelivery.SrcStoreID;
                        existDelivery.TarStoreID = submitDelivery.TarStoreID;
                        existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                        existDelivery.UpdateDate = CurrentUpdateDate;
                        existDelivery.ClientID = submitDelivery.ClientID = CurrentEmployee.ClientID;
                        existDelivery.Status = (int)DeliveryStatusEnum.Delivering;
                        existDelivery.Version += 1;

                        if (existDelivery.SrcStore == null)//没有发货仓
                        {
                            existDelivery.DeliveryType = (int)DeliveryTypeEnum.PurchaseIn;//购货入库
                        }
                        else
                        {
                            existDelivery.DeliveryType = (int)DeliveryTypeEnum.Transfer;//调拨
                        }


                        // remove old delivery items
                        if (existDelivery.DeliveryItemList != null)
                        {
                            db.DeliveryItemsDB.RemoveRange(existDelivery.DeliveryItemList);
                        }


                        // add new delivery items
                        if (submitDelivery.DeliveryItemList != null)
                        {
                            foreach (DeliveryItem d in submitDelivery.DeliveryItemList)
                            {
                                d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                                d.Product = null;
                                d.UpdateEmployee = null;
                                d.UpdateEmployeeID = CurrentEmployee.ID;
                                d.UpdateDate = CurrentUpdateDate;
                            }
                            db.DeliveryItemsDB.AddRange(submitDelivery.DeliveryItemList);
                        }
                        db.SaveChanges();
                        returnDeliveryID = existDelivery.ID;
                    }
                }
                else
                { // 2. if add case: add new delivery, generate new DON, save and add new out items from submit,. with the time and operator
                    using (db)
                    { // transation 习惯使用using以保证数据操作在事务内

                        // save new delivery
                        Delivery newDelivery = new Delivery();

                        if (submitDelivery.SrcStoreID == submitDelivery.TarStoreID)
                        {
                            String msg = LangHelper.Get("Delivery_Result_Error_SrcStoreCannotEqualTarStore");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }

                        newDelivery.CarrierID = submitDelivery.CarrierID;
                        newDelivery.SrcStoreID = null;
                        newDelivery.TarStoreID = submitDelivery.TarStoreID;
                        newDelivery.DeliveryType = (int)DeliveryTypeEnum.PurchaseIn;//购货入库
                        newDelivery.ClientID = CurrentEmployee.ClientID;
                        newDelivery.Status = (int)DeliveryStatusEnum.Delivering;
                        newDelivery.DON =Convert.ToString(CurrentEmployee.ClientID);
                        newDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                        newDelivery.UpdateDate = CurrentUpdateDate;
                        newDelivery.Version = 1;

                        db.DeliveriesDB.Add(newDelivery);
                        db.SaveChanges();
                        int dyLogID = LogDeliveryChange(db, newDelivery.ID, (int)DeliveryStatusEnum.Submit, (int)DeliveryStatusEnum.Delivering);
                        // save new delivery item list
                        if (submitDelivery.DeliveryItemList != null)
                        {
                            foreach (DeliveryItem d in submitDelivery.DeliveryItemList)
                            {
                                // d.InQty=
                                d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                                d.Product = null;
                                d.UpdateEmployee = null;
                                d.UpdateEmployeeID = CurrentEmployee.ID;
                                d.UpdateDate = CurrentUpdateDate;
                                d.DeliveryID = newDelivery.ID;
                            }
                            db.DeliveryItemsDB.AddRange(submitDelivery.DeliveryItemList);
                        }

                        db.SaveChanges();
                        returnDeliveryID = newDelivery.ID;
                    }
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }

            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }

        /// <summary>
        ///  confirm  inbound delivery  receive
        /// Processs:
        /// 1. delivery update: status from [delivering] to [receive] , and update the updateDate and operator
        /// 2.  source storage update:  if  srcStore is null ,update  Target [inSum+] [avail+] , and update the updateDate and operator
        /// 3. source storage update:  if  srcStore is not null ,update Source:[onOut-]  , and update the updateDate and operator
        /// 4. target storage update: if  srcStore is not null ,update [onIn-][inSum+] [avail+]  , and update the updateDate and operator
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult InboundReceiveDeliveryJson(Delivery receiveDelivery)
        {
            string message = LangHelper.Get("Delivery_Result_Success_ReceiveInboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                if (receiveDelivery.ID > 0)
                {
                    using (db)
                    { // transation 习惯使用using以保证数据操作在事务内
                        // update exist delivery by submit delivery
                        Delivery existDelivery = db.DeliveriesDB.Find(receiveDelivery.ID);
                        Boolean bol = VersionEqual(receiveDelivery.Version, existDelivery.Version);

                        if (!bol)
                        {
                            string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }
                        existDelivery.CarrierID = receiveDelivery.CarrierID;
                        existDelivery.SrcStoreID = receiveDelivery.SrcStoreID;
                        existDelivery.TarStoreID = receiveDelivery.TarStoreID;
                        existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                        existDelivery.UpdateDate = CurrentUpdateDate;
                        existDelivery.Version += 1;
                        existDelivery.ClientID = receiveDelivery.ClientID = CurrentEmployee.ClientID;
                        existDelivery.Status = (int)DeliveryStatusEnum.Received;
                        Store tarStore = db.StoresDB.Find(receiveDelivery.TarStoreID);// tarStore

                        int dyLogID = LogDeliveryChange(db, existDelivery.ID, (int)DeliveryStatusEnum.Delivering, (int)DeliveryStatusEnum.Received);

                        // remove old delivery items
                        if (existDelivery.DeliveryItemList != null)
                        {
                            db.DeliveryItemsDB.RemoveRange(existDelivery.DeliveryItemList);
                        }



                        if (existDelivery.DeliveryType == (int)DeliveryTypeEnum.PurchaseIn)//购货入库
                        {
                            foreach (DeliveryItem d in receiveDelivery.DeliveryItemList)
                            {
                                //tarStore Inventory  目的仓  库存   [inSum+] [avail+]
                                Inventory tarInv = GetInventory((int)tarStore.ID, (int)d.ProductID);
                                Inventory tarInv1 = (Inventory)tarInv.Clone();

                                if (tarInv == null)
                                { // check if inventory exist~

                                    String msg = string.Format(LangHelper.Get("Product_Empty"), d.Product.SKU, tarStore.Name);
                                    return new JsonNetPackResult(ERROR, msg, 0);
                                }
                                //add InQty   按照入库数量增加
                                tarInv.InSum += d.InQty;
                                tarInv.Avail += d.InQty;

                                tarInv.UpdateEmployeeID = CurrentEmployee.ID;
                                tarInv.UpdateDate = CurrentUpdateDate;

                                Inventory tarInv2 = (Inventory)tarInv.Clone();

                                int invLogID = LogInventoryChange(db, tarInv.ID, dyLogID, tarInv1, tarInv2);


                                d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                                d.Product = null;
                                d.UpdateEmployee = null;
                                d.UpdateEmployeeID = CurrentEmployee.ID;
                                d.UpdateDate = CurrentUpdateDate;
                            }
                        }
                        else if (existDelivery.DeliveryType == (int)DeliveryTypeEnum.Transfer)//调拨
                        {
                            Store srcStore = db.StoresDB.Find(receiveDelivery.SrcStoreID);

                            if (receiveDelivery.DeliveryItemList != null)
                            {
                                foreach (DeliveryItem d in receiveDelivery.DeliveryItemList)
                                {
                                    //srcStore Inventory  发货仓 
                                    Inventory srcInv = GetInventory((int)srcStore.ID, (int)d.ProductID);
                                    Inventory srcInv1 = (Inventory)srcInv.Clone();

                                    if (srcInv == null)
                                    { // check if inventory exist~
                                        String msg = string.Format(LangHelper.Get("Product_Empty"), d.Product.SKU, tarStore.Name);
                                        return new JsonNetPackResult(ERROR, msg, 0);
                                    }

                                    //发货仓 库存  [onOut-]
                                    srcInv.OnOut -= d.InQty;
                                    srcInv.UpdateEmployeeID = CurrentEmployee.ID;
                                    srcInv.UpdateDate = CurrentUpdateDate;

                                    Inventory srcInv2 = (Inventory)srcInv.Clone();
                                    int invSrcLogID = LogInventoryChange(db, srcInv.ID, dyLogID, srcInv1, srcInv2);


                                    //tarStore Inventory  目的仓 
                                    Inventory tarInv = GetInventory((int)tarStore.ID, (int)d.ProductID);
                                    Inventory tarInv1 = (Inventory)tarInv.Clone();

                                    if (tarInv == null)
                                    { // check if inventory exist~

                                        String msg = string.Format(LangHelper.Get("Product_Empty"), d.Product.SKU, tarStore.Name);
                                        return new JsonNetPackResult(ERROR, msg, 0);
                                    }

                                    //目的仓 库存   [onIn-][inSum+] [avail+]
                                    tarInv.InSum += d.InQty;
                                    tarInv.Avail += d.InQty;
                                    tarInv.OnIn -= d.InQty;



                                    tarInv.UpdateEmployeeID = CurrentEmployee.ID;
                                    tarInv.UpdateDate = CurrentUpdateDate;

                                    Inventory tarInv2 = (Inventory)tarInv.Clone();
                                    int invTarLogID = LogInventoryChange(db, tarInv.ID, dyLogID, tarInv1, tarInv2);

                                    d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                                    d.Product = null;
                                    d.UpdateEmployee = null;
                                    d.UpdateEmployeeID = CurrentEmployee.ID;
                                    d.UpdateDate = CurrentUpdateDate;
                                }

                            }
                        }
                        db.DeliveryItemsDB.AddRange(receiveDelivery.DeliveryItemList);
                        // add new delivery items
                        db.SaveChanges();
                        returnDeliveryID = existDelivery.ID;
                    }
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }

            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);
        }

        /// <summary>
        /// Delete an inbound submited delivery.
        /// Processs:
        /// 1. delete the inbound delivery and its items
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult InboundDeleteDelivery(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_DeleteInboundDeliverySuccess");

            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内

                    Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);
                    Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);
                    if (!bol)
                    {
                        string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                        return new JsonNetPackResult(ERROR, msg, 0);
                    }
                    Store tarStore = db.StoresDB.Find(submitDelivery.TarStoreID);
                    if (submitDelivery.DeliveryType == (int)DeliveryTypeEnum.PurchaseIn)
                    {
                        foreach (DeliveryItem d in submitDelivery.DeliveryItemList)
                        {
                            //tarStore Inventory  入货仓库存   [inSum+] [avail+]
                            Inventory tarInv = GetInventory(tarStore.ID, d.ProductID);

                            if (tarInv == null)// check if inventory exist~
                            {
                                String msg = string.Format(LangHelper.Get("Product_Empty"), d.Product.SKU, tarStore.Name);
                                return new JsonNetPackResult(ERROR, msg, 0);
                            }

                            tarInv.UpdateEmployeeID = CurrentEmployee.ID;
                            tarInv.UpdateDate = CurrentUpdateDate;


                            d.UpdateDate = new DateTime(DateTime.Now.Ticks);
                            d.Product = null;
                            d.UpdateEmployee = null;
                            d.UpdateEmployeeID = CurrentEmployee.ID;
                            d.UpdateDate = CurrentUpdateDate;
                        }

                        db.DeliveriesDB.Remove(existDelivery);
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }
            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }


        /// <summary>
        /// Finish an  inbound submited delivery.
        /// Processs:
        /// 1. change the inbound delivery and its items
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetPackResult InboundFinishDeliveryJson(Delivery submitDelivery)
        {

            string message = LangHelper.Get("Delivery_Result_Success_FinishInboundDeliverySuccess");
            int returnDeliveryID = 0; // the (new or updated)delivery object which return to UI
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内


                    Store tarStore = db.StoresDB.Find(submitDelivery.TarStoreID);

                    // update exist delivery by submit delivery
                    Delivery existDelivery = db.DeliveriesDB.Find(submitDelivery.ID);

                    Boolean bol = VersionEqual(submitDelivery.Version, existDelivery.Version);
                    if (!bol)
                    {
                        string msg = LangHelper.Get("Common_Result_Error_VersionNotEqual");
                        return new JsonNetPackResult(ERROR, msg, 0);
                    }
                    existDelivery.CarrierID = submitDelivery.CarrierID;
                    existDelivery.SrcStoreID = submitDelivery.SrcStoreID;
                    existDelivery.TarStoreID = submitDelivery.TarStoreID;
                    existDelivery.Version += 1;
                    existDelivery.UpdateEmployeeID = CurrentEmployee.ID;
                    existDelivery.UpdateDate = CurrentUpdateDate;
                    existDelivery.ClientID = submitDelivery.ClientID = CurrentEmployee.ClientID;
                    existDelivery.Status = (int)DeliveryStatusEnum.Success;

                    int dyLogID = LogDeliveryChange(db, existDelivery.ID, (int)DeliveryStatusEnum.Received, (int)DeliveryStatusEnum.Success);

                    if (existDelivery.DeliveryItemList != null)
                    {
                        //compare exist delivery and submit delivery
                        foreach (DeliveryItem ed in existDelivery.DeliveryItemList)
                        {
                            foreach (DeliveryItem sd in submitDelivery.DeliveryItemList)
                            {
                                if (ed.ID == sd.ID)//对比数据库与新的同个货物OutQty是否有变化
                                {
                                    Store srcStore = db.StoresDB.Find(submitDelivery.SrcStoreID);
                                    Inventory tarInv = GetInventory((int)submitDelivery.TarStoreID, (int)sd.ProductID);
                                    //tarStore Inventory  
                                    if (tarInv == null)// check if inventory exist~
                                    {
                                        String msg = string.Format(LangHelper.Get("Product_Empty"), sd.Product.SKU, tarStore.Name);
                                        return new JsonNetPackResult(ERROR, msg, 0);
                                    }
                                    Inventory tarInv1 = (Inventory)tarInv.Clone();
                                    //正常数据库操作: 收货方：[onIn-] [inSum+] [avail+] 的数量以输入的inQty为准
                                    if (ed.OutQty < sd.OutQty)//发货方 发货多了
                                    {
                                        //目的仓 库存  计算出多收了多少 diffValue ,然后tarInv 中的[OnIn-  InSum+     Avail+]diffValue
                                        int diffValue = sd.OutQty - ed.OutQty;

                                        tarInv.OnIn += diffValue;
                                        Inventory tarInv2 = (Inventory)tarInv.Clone();
                                        int invTarLogID = LogInventoryChange(db, tarInv.ID, dyLogID, tarInv1, tarInv2);


                                        if (submitDelivery.DeliveryType == (int)DeliveryTypeEnum.Transfer)//如果是调拨  有发货仓的
                                        {
                                            //srcStore Inventory  发货仓 库存   [avail-] [outSum+]  
                                            Inventory srcInv = GetInventory((int)submitDelivery.SrcStoreID, (int)sd.ProductID);
                                            Inventory srcInv1 = (Inventory)srcInv.Clone();
                                            if (srcInv == null)// check if inventory exist~
                                            {
                                                String msg = string.Format(LangHelper.Get("Product_Empty"), sd.Product.SKU, srcStore.Name);
                                                return new JsonNetPackResult(ERROR, msg, 0);
                                            }
                                            // 发货仓 多发出 数量  Avail-   OutSum+
                                            srcInv.Avail -= diffValue;
                                            srcInv.OutSum += diffValue;
                                            srcInv.OnOut += diffValue;

                                            Inventory srcInv2 = (Inventory)srcInv.Clone();
                                            int invSrcLogID = LogInventoryChange(db, tarInv.ID, dyLogID, srcInv1, srcInv2);
                                        }
                                    }
                                    else if (sd.InQty < sd.OutQty)//不可协商遗失
                                    {
                                        //tarStore Inventory  发货仓 库存   [shipMIss+] 
                                        int missValue = sd.OutQty - sd.InQty;

                                        tarInv.OnIn -= missValue;
                                        Inventory tarInv2 = (Inventory)tarInv.Clone();
                                        int invTarLogID = LogInventoryChange(db, tarInv.ID, dyLogID, tarInv1, tarInv2);

                                        if (submitDelivery.DeliveryType == (int)DeliveryTypeEnum.PurchaseIn)//购入 记收货方
                                        {
                                            Inventory srcInv1 = (Inventory)tarInv.Clone();
                                            tarInv.ShipMiss += missValue;
                                            Inventory srcInv2 = (Inventory)tarInv.Clone();
                                            int invSrcLogID = LogInventoryChange(db, tarInv.ID, dyLogID, srcInv1, srcInv2);
                                        }
                                        if (submitDelivery.DeliveryType == (int)DeliveryTypeEnum.Transfer)//调拨  记发货方
                                        {
                                            Inventory srcInv = GetInventory((int)submitDelivery.SrcStoreID, (int)sd.ProductID);
                                            Inventory srcInv1 = (Inventory)srcInv.Clone();
                                            srcInv.OnOut -= missValue;
                                            srcInv.OutSum -= missValue;
                                            srcInv.ShipMiss += missValue;
                                            Inventory srcInv2 = (Inventory)srcInv.Clone();
                                            int invSrcLogID = LogInventoryChange(db, tarInv.ID, dyLogID, srcInv1, srcInv2);
                                        }

                                     
                                    }
                                    else if (ed.OutQty > sd.OutQty)//可协商遗失
                                    {
                                        int diffValue = ed.OutQty - sd.OutQty;// 发货少的数量

                                        //tarStore Inventory  目的仓 库存   不变
                                        tarInv.OnIn -= diffValue;
                                        Inventory tarInv2 = (Inventory)tarInv.Clone();
                                        int invTarLogID = LogInventoryChange(db, tarInv.ID, dyLogID, tarInv1, tarInv2);

                                        if (submitDelivery.DeliveryType == (int)DeliveryTypeEnum.Transfer)//如果是调拨  有发货仓的
                                        {
                                            //srcStore Inventory  发货仓 库存  
                                            Inventory srcInv = GetInventory((int)submitDelivery.SrcStoreID, (int)sd.ProductID);
                                            Inventory srcInv1 = (Inventory)srcInv.Clone();
                                            if (srcInv == null)// check if inventory exist~
                                            {
                                                String msg = string.Format(LangHelper.Get("Product_Empty"), sd.Product.SKU, srcStore.Name);
                                                return new JsonNetPackResult(ERROR, msg, 0);
                                            }

                                         
                                            //如果出货 入货协商调平 出货仓 [avail+] [outSum-]  
                                            srcInv.Avail += diffValue;
                                            srcInv.OutSum -= diffValue;
                                            srcInv.OnOut -= diffValue;

                                            Inventory srcInv2 = (Inventory)srcInv.Clone();
                                            int invSrcLogID = LogInventoryChange(db, tarInv.ID, dyLogID, srcInv1, srcInv2);
                                        }
                                    }
                                }
                            }
                        }
                        foreach (DeliveryItem sd in submitDelivery.DeliveryItemList)
                        {
                            sd.UpdateDate = new DateTime(DateTime.Now.Ticks);
                            sd.Product = null;
                            sd.UpdateEmployee = null;
                            sd.UpdateEmployeeID = CurrentEmployee.ID;
                            sd.UpdateDate = CurrentUpdateDate;
                        }

                        db.DeliveryItemsDB.RemoveRange(existDelivery.DeliveryItemList);// remove old delivery items
                        db.DeliveryItemsDB.AddRange(submitDelivery.DeliveryItemList);// add new delivery items
                    }


                    db.SaveChanges();
                    returnDeliveryID = existDelivery.ID;
                }
            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.Message, 0);
            }
            return new JsonNetPackResult(SUCCESS, message, returnDeliveryID);

        }

        #endregion

        //判断 版本号是否相同
        public static bool VersionEqual(int Version1, int Version2)
        {
            Boolean bol;

            if (Version1 == Version2)
            {
                bol = true;
            }
            else
            {
                bol = false;
            }
            return bol;
        }

    }
}