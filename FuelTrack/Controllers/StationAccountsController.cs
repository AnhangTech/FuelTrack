using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuelTrack.Models;

namespace FuelTrack.Controllers
{
    public class StationAccountsController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        // GET: StationAccounts
        public ActionResult Index()
        {
            return View(db.StationAccounts.ToList());
        }

        // GET: StationAccounts/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationAccount stationAccount = db.StationAccounts.Find(id);
            if (stationAccount == null)
            {
                return HttpNotFound();
            }
            return View(stationAccount);
        }


        public JsonResult DetailsJson(long id)
        {
            StationAccount stationAccount = db.StationAccounts.Find(id);

            if (stationAccount == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json("Station Not Found",
                JsonRequestBehavior.AllowGet);
            }

            return Json(
                new
                {
                    StationAccountId = stationAccount.StationAccountId,
                    Name = stationAccount.StationName,
                    BankAccountName = stationAccount.BankAccountName,
                    BankAccountNumber = stationAccount.BankAccountNumber,
                    Deposite = stationAccount.Deposite              
                }, 
                JsonRequestBehavior.AllowGet);
        }

        // GET: StationAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StationAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StationAccountId,StationName,BankAccountName,BankAccountNumber,BankBranch")] StationAccount stationAccount)
        {
            if (ModelState.IsValid)
            {
                db.StationAccounts.Add(stationAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stationAccount);
        }

        // GET: StationAccounts/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationAccount stationAccount = db.StationAccounts.Find(id);
            if (stationAccount == null)
            {
                return HttpNotFound();
            }
            return View(stationAccount);
        }

        // POST: StationAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StationAccountId,StationName,BankAccountName,BankAccountNumber,BankBranch")] StationAccount stationAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stationAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stationAccount);
        }

        // GET: StationAccounts/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StationAccount stationAccount = db.StationAccounts.Find(id);
            if (stationAccount == null)
            {
                return HttpNotFound();
            }
            return View(stationAccount);
        }

        // POST: StationAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            StationAccount stationAccount = db.StationAccounts.Find(id);
            //db.StationAccounts.Remove(stationAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
