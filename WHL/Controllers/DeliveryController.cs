using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

using WHL.Models.Virtual;
using WHL.Models.Entity.Deliveries;

namespace WHL.Controllers
{
    /// <summary>
    /// DeliveryController：we put all delivery view controllers here
    /// </summary>
    public class DeliveryController : BaseController
    {

        /// <summary>
        /// Get the delivery list result in jquery data table JSON format, return the DataTable Result with paging and sorting.
        /// </summary>
        /// <param name="dtParams">Jquery Data Table Parameter</param>
        /// <param name="queryInventory">Query Delivery Object</param>
        /// <returns>JSON: DataTable Result with paging and sorting</returns>
        public JsonNetResult GetDeliveryDataTableJson(DTParams dtParams, Delivery queryDelivery)
        {
            DTResult<Delivery> deliveryDataTableResult = deliveryService.GetDeliveryDataTableResult(dtParams, queryDelivery);
            return new JsonNetResult(deliveryDataTableResult);
        }

        /// <summary>
        /// Save the outbound delivery submit,including create and update
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult OutboundSubmitDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.OutboundSubmitDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Delete an outbound submited delivery.
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult OutboundDeleteDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.OutboundDeleteDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Process an submited outbound delivery.
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult OutboundProcessDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.OutboundProcessDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Cancel an outbound processing delivery.
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult OutboundCancelDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.OutboundCancelDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Ship and outbound processing delivery
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult OutboundShipDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.OutboundShipDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Save the inbound delivery submit,including create and update
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult InboundSubmitDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.InboundSubmitDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Save the inbound delivery receive
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult InboundReceiveDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.InboundReceiveDeliveryJson(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Delete an inbound submited delivery.
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult InboundDeleteDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.InboundDeleteDelivery(submitDelivery);
            return new JsonNetResult(packResult);
        }

        /// <summary>
        /// Finish an inbound submited delivery.
        /// </summary>
        /// <param name="submitDelivery">delivery from UI post</param>
        /// <returns>Json package result</returns>
        public JsonNetResult InboundFinishDeliveryJson(Delivery submitDelivery)
        {
            JsonNetPackResult packResult = deliveryService.InboundFinishDeliveryJson(submitDelivery);
            return new JsonNetResult(packResult);
        }


    }
}