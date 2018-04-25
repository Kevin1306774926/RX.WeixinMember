using RX.WXMember.Filters;
using RX.WXMember.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RX.WXMember.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyDbContext db = new MyDbContext();
        // GET: Admin/Home
        [CheckLoginFilter]
        public ActionResult Index()
        {
            ViewBag.CustomerTotal = db.Customers.Count();
            ViewBag.GroundCodeTotal = db.RegRecords.GroupBy(t => t.GroundCode).Count();
            return View();
        }
    }
}