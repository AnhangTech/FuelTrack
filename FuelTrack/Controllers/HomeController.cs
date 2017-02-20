using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Controllers
{
    public class HomeController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            DateTime DueSoonDate = DateTime.Today.AddDays(-5);

            model.DueSoonLoans = db.Loans.Where(l => DbFunctions.AddDays(l.StartDate, l.FreeDays) < DueSoonDate
            && DbFunctions.AddDays(l.StartDate, l.FreeDays) > DateTime.Today
            && l.CurrentAmount > 0).ToList();


            model.DueLoans = db.Loans.Where(l => DbFunctions.AddDays(l.StartDate, l.FreeDays) < DateTime.Today && l.CurrentAmount > 0).ToList();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}