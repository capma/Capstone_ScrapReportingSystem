using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stackpole.Models;

namespace Stackpole.Controllers
{
    public class HomeController : Controller
    {
        private StackpoleEntities db;

        public HomeController()
        {
            db = new StackpoleEntities();
        }

        public ActionResult Index()
        {
            return View(db.Plants.Where(p=>p.id <= 3).ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Scrap System.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";

            return View();
        }
    }
}