using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;

using WHL.Helpers;
using WHL.Services;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;



namespace WHL.Controllers
{
    /// <summary>
    /// BaseController: All common method or properites put here, and any biz controller can impletement it.
    /// Author: Pango
    /// </summary>
    public class BaseController : Controller
    {
        // the json result flag
        public const int SUCCEED = 1;
        public const int ERROR = 0;

        // all service put here
        protected InventoryService inventoryService;
        protected DeliveryService deliveryService;
        protected StoreService storeService;
        protected EmployeeService employeeService;
        protected MapRuleService mapRuleService;
        protected CarrierService carrierService;
        protected FileDataService fileDataService;
        protected EpacketService epacketService;


        public BaseController()
        {
            inventoryService = new InventoryService() { CurrentEmployee = GetCurrentEmployee() };
            deliveryService = new DeliveryService() { CurrentEmployee = GetCurrentEmployee() };
            storeService = new StoreService() { CurrentEmployee = GetCurrentEmployee() };
            employeeService = new EmployeeService() { CurrentEmployee = GetCurrentEmployee() };
            mapRuleService = new MapRuleService() { CurrentEmployee = GetCurrentEmployee() };
            carrierService = new CarrierService() { CurrentEmployee = GetCurrentEmployee() };
            fileDataService = new FileDataService() { CurrentEmployee = GetCurrentEmployee() };
            epacketService = new EpacketService() { CurrentEmployee = GetCurrentEmployee() };
        }

        /// <summary>
        /// generate a test employee in the session.
        /// </summary>
        public void GenerateTestSessionEmployee()
        {
            if ((System.Web.HttpContext.Current.Session != null) || (System.Web.HttpContext.Current.Session["CurrentEmployee"] == null))
            {
                Employee testEmployee = employeeService.GetTestEmployee();
                System.Web.HttpContext.Current.Session["CurrentEmployee"] = testEmployee;
            }
        }

        /// <summary>
        /// get the employee in session
        /// </summary>
        /// <returns></returns>
        public Employee GetCurrentEmployee()
        {
            var a = System.Web.HttpContext.Current.Session;
            if (System.Web.HttpContext.Current.Session != null)
            {

                Employee curEmployee = (Employee)System.Web.HttpContext.Current.Session["CurrentEmployee"];
                return curEmployee;
            }
            else
            {
                return null;
            }

        }

        // GET: Layout - Header
        public ActionResult Header()
        {
            // generate a test employee in session.
            GenerateTestSessionEmployee();
            // do something
            return PartialView();
        }

        // GET: Layout - Menu
        public ActionResult Menu()
        {
            // do something
            return PartialView();
        }

        // GET: Base Index
        public ActionResult Index()
        {
            return View();
        }

       


      
    }





}