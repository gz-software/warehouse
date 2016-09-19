using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHL.DAL;

namespace WHL.Controllers
{
    public class HomeController : BaseController
    {
        private DataContext db = new DataContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}