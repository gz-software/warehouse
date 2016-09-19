using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

using WHL.Services;
using WHL.Models.Virtual;


namespace WHL.Controllers
{
    /// <summary>
    /// ModalController: we put all modals(pop up partial view) here.
    /// </summary>
    public class ModalController : BaseController
    {
        /// <summary>
        /// Outbound popup partial view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Outbound()
        {
            return PartialView();
        }
        public ActionResult Import()
        {
            return PartialView();
        }

        public ActionResult Export()
        {
            return PartialView();
        }

        public ActionResult Inbound()
        {
            return PartialView();
        }

        public ActionResult Epacket() {
            return PartialView();
        }

    }
}