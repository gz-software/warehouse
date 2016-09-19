using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ePacket;
using ePacket.AddPackage;
using ePacket.GetAPACShippingRate;

using WHL.Services;
using WHL.Models.Virtual;
using WHL.Models.Entity.Deliveries;
using WHL.Models.Entity.Carriers;

using WHL.Models.Entity.Shipments;
using WHL.Models.Entity.Shipments.Epackets;




namespace WHL.Controllers
{
    public class EpacketController : BaseController
    {

        /// <summary>
        /// Debug Test: http://localhost:6659/zh_CN/Epacket/AddPackageJson?carrierID=1&employeeID=1&srcStoreID=1&debug=true
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
        /// <param name="debug">if debug, it will not call the api and return some test value</param>
        /// <returns>
        /// A formated return data:
        /// JsonNetPackResult.message:  if success, it return the DON number, otherwise, return the error message
        /// JsonApiReturnData.ApiRequest: the api calling request
        /// JsonApiReturnData.ApiResponse: the api calling response
        /// JsonApiReturnData.BizData: a full data of a delivery , containing delivery info, shipment info, src store info and so on.
        /// </returns>
        public JsonNetResult AddPackageJson(int employeeID, int carrierID, int srcStoreID, AddAPACPackageRequest myRequest,Boolean debug)
        {
            JsonNetPackResult packResult = epacketService.AddPackage(employeeID, carrierID, srcStoreID, myRequest, debug);
            return new JsonNetResult(packResult);
        }


        /// <summary>
        /// Debug Test: http://localhost:6659/zh_CN/Epacket/CheckPackageRateJson?carrierID=1&debug=true 
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
        public JsonNetResult CheckPackageRateJson(int carrierID, GetRateRequest myRequest, Boolean debug)
        {

            JsonNetPackResult packResult = epacketService.CheckPackageRate(carrierID, myRequest , debug);
            return new JsonNetResult(packResult);
        }





        /// <summary>
        /// Debug Test: http://localhost:6659/zh_CN/Epacket/ConfirmPackageJson?carrierID=1&trackCode=123456&debug=true 
        /// Epacket Confirm  a package
        /// </summary>
        /// <param name="carrierID">the carrier caller defined</param>
        /// <param name="trackCode">the package trackCode</param>
        /// <param name="debug">if debug, it will not call the api and return some test value</param>
        /// <returns></returns>
        public JsonNetResult ConfirmPackageJson(int carrierID, string trackCode, Boolean debug)
        {
            JsonNetPackResult packResult = epacketService.ConfirmPackage(carrierID, trackCode, debug);
            return new JsonNetResult(packResult);
        }
    }
}