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
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.Stores;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Inventories;
using WHL.Models.Entity.Carriers;
using WHL.Models.Entity.Shipments;
using WHL.Models.Entity.Shipments.Epackets;

using ePacket;
using ePacket.AddPackage;
using ePacket.GetAPACShippingRate;
using ePacket.ConfirmPackage;

namespace WHL.Services
{
    /// <summary>
    /// Epacket Service: an external interface which handing epackage shipment calling
    /// </summary>
    public class EpacketService : BaseService
    {
        // the api calling function
        CallFunction myCall = new CallFunction();

        /// <summary>
        /// Epacket Check Delivery Rate
        /// </summary>
        /// <param name="carrierID">the carrier caller defined</param>
        /// <param name="myRequest">the epacket api request</param>
        /// <param name="debug">if debug, it will not call the api and return some test value</param>
        /// <returns>
        ///  A formated return data:
        ///  JsonNetPackResult.message:  the currency rate for this ship.
        ///  JsonApiReturnData.ApiRequest: the api calling request
        ///  JsonApiReturnData.ApiResponse: the api calling response
        ///  JsonApiReturnData.BizData: nothing here , not any WHL effect
        /// </returns>
        public JsonNetPackResult CheckPackageRate(int carrierID, GetRateRequest myRequest , Boolean debug) {
            GetRateResponse myResponse = new GetRateResponse();
            if (debug)
            { // if in debug mode , we generate a test request
                myRequest = GenTestGetRateRequest();
            }

            Carrier carrier = db.CarriersDB.Find(carrierID);
            if (carrier == null)
            {
                return new JsonNetPackResult(ERROR, "Carrier not exist", new JsonApiReturnData(myRequest, myResponse, null));
            }
            EpacketAuth epacketAuth = carrier.Auth;
            if (epacketAuth == null)
            {
                return new JsonNetPackResult(ERROR, "Auth not exist", new JsonApiReturnData(myRequest, myResponse, null));
            }

            CommonHelper.CopyObjectValue(epacketAuth, myRequest.Auth); // 把carrier里面的auth 复制赋值到apiRequest.auth

            double deliveryCharge = 0;

            if (debug){//debug
                deliveryCharge = 18.8;

            }else { // not debug
                myResponse = myCall.GetAPACShippingRate(myRequest);
                if (myResponse.Ack.ToUpper() != "SUCCESS")
                {
                    return new JsonNetPackResult(ERROR, "Epacket API Calling Fail, Please see Data.ApiResponse to see the problem!", new JsonApiReturnData(myRequest, myResponse, null));
                }
                deliveryCharge = myResponse.DeliveryCharge;
            }
            return new JsonNetPackResult(SUCCESS, deliveryCharge.ToString(), new JsonApiReturnData(myRequest, myResponse, null));

        }

        /// <summary>
        /// Epacket Add Package:
        /// 1. check the param data correct or not
        /// 2. check there are enough inventory
        /// 3. call the epacket add package api and response the track code
        /// 4. if #3 succeed, create new delivery(new->process), add items from apiRequest
        /// 5. add shipment and shipData( we will save the api request full data in stream object to database) 
        /// </summary>
        /// <param name="employeeID">the employee who call this method</param>
        /// <param name="carrierID">the carrier caller defined</param>
        /// <param name="srcStoreID">the store which will out post the items</param>
        /// <param name="myRequest">the epacket api request</param>
        /// <param name="debugDON">default should be null, it is for the initial data importing</param>
        /// <returns>
        /// A formated return data:
        /// JsonNetPackResult.message:  if success, it return the DON number, otherwise, return the error message
        /// JsonApiReturnData.ApiRequest: the api calling request
        /// JsonApiReturnData.ApiResponse: the api calling response
        /// JsonApiReturnData.BizData: a full data of a delivery , containing delivery info, shipment info, src store info and so on.
        /// </returns>
        public JsonNetPackResult AddPackage(int employeeID, int carrierID, int srcStoreID, AddAPACPackageRequest myRequest, Boolean debug)
        {
            AddAPACPackageResponse myResponse = new AddAPACPackageResponse();
            if (debug) { // if debug , generate a test request 
                myRequest = GenTestAddPackageRequest();
            }
           
            Delivery newDelivery = null;

            // 1. check the param data correct or not
            Employee employee = db.EmployeesDB.Find(employeeID);
            if (employee == null)
            {
                return new JsonNetPackResult(ERROR, "Employee not exist", new JsonApiReturnData(myRequest, myResponse, newDelivery));
            }

            Carrier carrier = db.CarriersDB.Find(carrierID);
            if (carrier == null)
            {
                return new JsonNetPackResult(ERROR, "Carrier not exist", new JsonApiReturnData(myRequest, myResponse, newDelivery));
            }
            Store srcStore = db.StoresDB.Find(srcStoreID);
            if (srcStore == null)
            {
                return new JsonNetPackResult(ERROR, "Store not exist", new JsonApiReturnData(myRequest, myResponse, newDelivery));
            }

            EpacketAuth epacketAuth = carrier.Auth;
            if (epacketAuth == null)
            {
                return new JsonNetPackResult(ERROR, "Auth not exist", new JsonApiReturnData(myRequest, myResponse, newDelivery));
            }

            CommonHelper.CopyObjectValue(epacketAuth, myRequest.Auth); // 把carrier里面的auth 复制赋值到apiRequest.auth
            
            try
            {
                using (db)
                { // transation 习惯使用using以保证数据操作在事务内


                    // 2. check there are enough inventory 先判断是否有这个SKU，并且够数目
                    foreach (Item reqItem in myRequest.ItemList)
                    {
                        string sku = reqItem.SKUID;
                        int postedQTY = reqItem.PostedQTY;
                        Inventory srcInv = GetInventory(srcStore.ID, sku);

                        if (srcInv == null)
                        {
                            return new JsonNetPackResult(ERROR, "Sorry,Sku " + sku + " not exist in " + srcStore.Name, new JsonApiReturnData(myRequest, myResponse, newDelivery));
                        }

                        if (postedQTY > srcInv.Avail)
                        {
                            return new JsonNetPackResult(ERROR, "Sorry,Sku " + sku + " not enough avail", new JsonApiReturnData(myRequest, myResponse, newDelivery));
                        }
                    }
                    // 3. call the epacket add package api and response the track code 进行调用，如果有错误，返回之
                    
                    string trackCode = "";

                    if (debug) { // debug
                        string ramStr = "TEST" + (new Random().Next(0, 99)).ToString().PadLeft(2, '0'); // ram number(0~100) , add zero to left
                        trackCode = ramStr;
                    }
                    else { // not debug

                        myResponse = myCall.AddAPACShippingPackage(myRequest);
                        if ((myResponse.Ack.ToUpper() != "SUCCESS") && (String.IsNullOrEmpty(myResponse.TrackCode)))
                        {
                            return new JsonNetPackResult(ERROR, "Epacket API Calling Fail, More information please see Data.ApiResponse", new JsonApiReturnData(myRequest, myResponse, newDelivery));
                        }
                        trackCode = myResponse.TrackCode;
                    }
                  
                    
                   


                    // 接口调用成功，并且检查数据正常，则创建Delivery
                    newDelivery = new Delivery();
                    newDelivery.DeliveryType = (int)DeliveryTypeEnum.SellingOut;
                    newDelivery.CarrierID = carrierID;
                    newDelivery.SrcStoreID = srcStoreID;
                    newDelivery.SrcStore = null;
                    newDelivery.TarStoreID = null;
                    newDelivery.ClientID = employee.ClientID;
                    newDelivery.Client = null;
                    newDelivery.UpdateEmployee = null;
                    newDelivery.Status = (int)DeliveryStatusEnum.Processing;
                    newDelivery.DON = GenerateNumber(employee.ClientID);
                 
                    
                    newDelivery.UpdateEmployeeID = employee.ID;
                    newDelivery.UpdateDate = CurrentUpdateDate;
                    newDelivery.Version = 1;
                    db.DeliveriesDB.Add(newDelivery);
                    db.SaveChanges();

                    List<DeliveryItem> newDyItemList = new List<DeliveryItem>();
                    // 接口调用的sku items塞进这个Delivery
                    if (myRequest.ItemList != null)
                    {
                        foreach (Item reqItem in myRequest.ItemList)
                        {
                            DeliveryItem dyItem = new DeliveryItem();
                            Product prod = db.ProductsDB.Where(p => p.SKU == reqItem.SKUID).First();

                            dyItem.ProductID = prod.ID;
                            dyItem.OutQty = reqItem.PostedQTY;
                            dyItem.UpdateDate = new DateTime(DateTime.Now.Ticks);
                            dyItem.Product = null;
                            dyItem.UpdateEmployee = null;
                            dyItem.UpdateEmployeeID = employee.ID;
                            dyItem.UpdateDate = CurrentUpdateDate;
                            dyItem.DeliveryID = newDelivery.ID;
                            newDyItemList.Add(dyItem);

                        }
                    }
                    db.DeliveryItemsDB.AddRange(newDyItemList);
                    db.SaveChanges();

                    // Log Delivery
                    int dyLogID = 0;

                    if (!debug) { 
                        dyLogID = LogDeliveryChange(db, newDelivery.ID, (int)DeliveryStatusEnum.New, (int)DeliveryStatusEnum.Processing);
                    }
               

                    // 影响Inventory的可用
                    foreach (DeliveryItem outItem in newDyItemList)
                    {
                        Inventory srcInv = GetInventory(srcStore.ID, outItem.ProductID);

                        Inventory srcInv1 = (Inventory)srcInv.Clone();

                        if (srcInv == null)
                        { // check if inventory exist~
                            String msg = string.Format(LangHelper.Get("Product_Empty"), outItem.Product.SKU, srcStore.Name);
                            return new JsonNetPackResult(ERROR, msg, new JsonApiReturnData(myRequest, myResponse, newDelivery));
                        }

                        srcInv.Avail -= outItem.OutQty;
                        if (srcInv.Avail < 0)
                        {

                            String msg = string.Format(LangHelper.Get("Delivery_Avail_Not_Enough"), srcStore.Name, outItem.OutQty, srcInv.Avail);
                            return new JsonNetPackResult(ERROR, msg, 0);
                        }
                        srcInv.OnHold += outItem.OutQty;
                        srcInv.UpdateEmployeeID = employee.ID;
                        srcInv.UpdateDate = CurrentUpdateDate;

                        Inventory srcInv2 = (Inventory)srcInv.Clone();

                        // Log Inventory
                        if (!debug)
                        {
                            int invLogID = LogInventoryChange(db, srcInv.ID, dyLogID, srcInv1, srcInv2);
                        }
                    }
                    db.SaveChanges();

                    // 创建Shipment
                    Shipment newShipment = new Shipment();
                    newShipment.DeliveryID = newDelivery.ID;
                    newShipment.UpdateEmployeeID = employee.ID;
                    newShipment.TrackCode = trackCode;
                    newShipment.UpdateDate = CurrentUpdateDate;
                    db.ShipmentsDB.Add(newShipment);
                    db.SaveChanges();

                    // 创建EpackageShipData
                    EpacketShipData newShipData = new EpacketShipData();
                    newShipData.ShipmentID = newShipment.ID;
                    newShipData.UpdateEmployeeID = employee.ID;
                    newShipData.UpdateDate = CurrentUpdateDate;
                    newShipData.PackageStreamData = new JsonNetResult(myRequest).ToJsonString();
                    db.EpacketShipDatasDB.Add(newShipData);
                    db.SaveChanges();

                    newDelivery = db.DeliveriesDB.Find(newDelivery.ID);


                }

                // 构造一个完整并返回

                return new JsonNetPackResult(SUCCESS, newDelivery.DON, new JsonApiReturnData(myRequest, myResponse, newDelivery));

            }
            catch (Exception e)
            {
                return new JsonNetPackResult(ERROR, e.StackTrace, new JsonApiReturnData(myRequest, myResponse, newDelivery));
            }
        }


        /// <summary>
        /// Epacket Confirm  a package
        /// </summary>
        /// <param name="carrierID">the carrier caller defined</param>
        /// <param name="trackCode">the package trackCode</param>
        /// <param name="debug">if debug, it will not call the api and return some test value</param>
        /// <returns></returns>
        public JsonNetPackResult ConfirmPackage(int carrierID, string trackCode, Boolean debug)
        {
            ConfirmPackageRequest myRequest = new ConfirmPackageRequest();
            ConfirmPackageResponse myResponse = new ConfirmPackageResponse();
            Carrier carrier = db.CarriersDB.Find(carrierID);
            if (carrier == null)
            {
                return new JsonNetPackResult(ERROR, "Carrier not exist", new JsonApiReturnData(myRequest, myResponse, null));
            }
            EpacketAuth epacketAuth = carrier.Auth;
            if (epacketAuth == null)
            {
                return new JsonNetPackResult(ERROR, "Auth not exist", new JsonApiReturnData(myRequest, myResponse, null));
            }

            CommonHelper.CopyObjectValue(epacketAuth, myRequest.Auth); // 把carrier里面的auth 复制赋值到apiRequest.auth

            myRequest.TrackCode = trackCode;
            myRequest.PickUpRequestDateTime = CurrentUpdateDate;

            if (debug)
            {  // debug
                // do nothing
            }
            else
            { // not debug
                myResponse = myCall.ConfirmAPACShippingPackage(myRequest);
                if ((myResponse.Ack.ToUpper() != "SUCCESS"))
                {
                    return new JsonNetPackResult(ERROR, "Epacket API Calling Fail, More information please see Data.ApiResponse", new JsonApiReturnData(myRequest, myResponse, null));
                }
            }
            return new JsonNetPackResult(SUCCESS, "This Package confirmed succeed", new JsonApiReturnData(myRequest, myResponse, null));

        }

        /// <summary>
        /// 获得一个测试GetRateRequest用例
        /// </summary>
        /// <returns></returns>
        public GetRateRequest GenTestGetRateRequest()
        {
            GetRateRequest myRequest = new GetRateRequest();
            myRequest.Service = "EPACK";
            myRequest.ShipFromAddress.Add(new ShipFromAddressInfo()
            {
                City = "430100",
                Company = "",
                Contact = "Koyomi",
                CountryCode = "CN",
                District = "430102",
                Email = "nickhua03191@outlook.com",
                Mobile = "13420737774",
                Phone = "",
                Postcode = "511400",
                Province = "430000",
                Street = "Test Street"
            });
            myRequest.ShipToAddress.Add(new ShipToAddressInfo()
            {
                //City = "Shunde",
                Company = "",
                Contact = "Shirobu",
                CountryCode = "RU",
                District = "",
                Email = "445031599@qq.com",
                Mobile = "",
                Phone = "",
                //Postcode = "528300",
                //Province = "Guangdong",
                Street = "Test Street2"
            });
            myRequest.Weight = 1;
            return myRequest;
        
        }

        /// <summary>
        /// 获得一个测试AddPackageRequest用例
        /// </summary>
        /// <returns></returns>
        public AddAPACPackageRequest GenTestAddPackageRequest()
        {

            AddAPACPackageRequest myRequest = new AddAPACPackageRequest();
            myRequest.EMSPickUpType = 1;
            myRequest.PickUpAddress.Add(new PickUpAddressInfo()
            {
                Contact = "Nick",
                Street = "YuShan WestRoad NO.329",
                District = "440113",
                City = "440100",
                Province = "440000",
                CountryCode = "CN",
                Mobile = "13420737774",
                Phone = "13420737774",
                Email = "nickhua03191@outlook.com"
            });
            myRequest.ShipFromAddress.Add(new ShipFromAddressInfo()
            {
                Contact = "Nick",
                Street = "YuShan WestRoad NO.329",
                District = "440113",
                City = "440100",
                Province = "440000",
                CountryCode = "CN",
                Mobile = "13420737774",
                //Phone = "",
                Email = "nickhua03191@outlook.com",
                Postcode = "511400",
            });
            myRequest.ShipToAddress.Add(new ShipToAddressInfo()
            {
                City = "Milpitas",
                Company = "",
                Contact = "Daisy Fung",
                CountryCode = "US",
                District = "",
                Email = "445031599@qq.com",
                Mobile = "",
                Phone = "",
                Postcode = "95035",
                Province = "CA",
                Street = "328 South Abbott Ave."
            });
            myRequest.ReturnAddress.Add(new ReturnAddressInfo()
            {
                Contact = "Nick",
                Street = "YuShan WestRoad NO.329",
                District = "440113",
                City = "440100",
                Province = "440000",
                CountryCode = "CN",
                Postcode = "511400",
            });
            myRequest.ItemList.Add(new Item()
            {
                SKUID = "A007",
                EBayItemID = "110183318797",
                EBayTransactionID = "0",
                EBayBuyerID = "nichua-8",
                PostedQTY = 2,
                DeclaredValue = 100,
                Weight = 1,
                CustomsTitle = "Test",
                CustomsTitleEN = "Test",
                OriginCountryCode = "CN",

            });
            return myRequest;
        }
    }
}