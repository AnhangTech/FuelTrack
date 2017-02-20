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
    public class DepositesController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        // GET: Deposites
        public ActionResult Index(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.StationAccountId = accountId;

            var depositeHistories = db.DepositeHistories.Where(d => d.StationAccountId == accountId);
            return View(depositeHistories.ToList().OrderByDescending(o=>o.Timestamp));
        }

        // GET: Deposites/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepositeHistory depositeHistory = db.DepositeHistories.Find(id);
            if (depositeHistory == null)
            {
                return HttpNotFound();
            }
            return View(depositeHistory);
        }

        // GET: Deposites/Create
        public ActionResult Recharge(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var depositeHistory = new DepositeHistory()
            {
                StationAccountId = accountId.Value
            };
            
            return View(depositeHistory);
        }

        // POST: Deposites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Recharge([Bind(Include = "StationAccountId,Amount,Description,Timestamp")] DepositeHistory depositeHistory)
        {
            DateTime time = depositeHistory.Timestamp;

            if (ModelState.IsValid)
            {
                StationAccount account = db.StationAccounts.Find(depositeHistory.StationAccountId);
                account.Deposite += depositeHistory.Amount;
                depositeHistory.Timestamp = time;
                depositeHistory.ChangeType = DepositeChangeType.Recharge;
                db.DepositeHistories.Add(depositeHistory);

                db.Entry(account).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = depositeHistory.StationAccountId });
            }
            
            return View(depositeHistory);
        }

        // GET: Deposites/Cashout/5
        public ActionResult Cashout(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StationAccount account = db.StationAccounts.Find(accountId);

            if(account == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(account.Deposite < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DepositeHistory depositeHistory = new DepositeHistory()
            {
                StationAccountId = accountId.Value,
                Amount = account.Deposite
            };

            return View(depositeHistory);
        }

        // POST: Deposites/Cashout/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cashout([Bind(Include = "StationAccountId,Amount,ChangeType,Description,Timestamp")] DepositeHistory depositeHistory)
        {
            DateTime time = depositeHistory.Timestamp;

            if (ModelState.IsValid)
            {
                StationAccount account = db.StationAccounts.Find(depositeHistory.StationAccountId);

                if(account.Deposite <0 || account.Deposite < depositeHistory.Amount || depositeHistory.Amount < 0)
                {
                    return View("The deposite is not enough to cashout.");
                }

                account.Deposite -= depositeHistory.Amount;
                depositeHistory.Amount = depositeHistory.Amount * -1;
                depositeHistory.Timestamp = time;
                depositeHistory.ChangeType = DepositeChangeType.Cashout;
                db.DepositeHistories.Add(depositeHistory);

                db.Entry(account).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = depositeHistory.StationAccountId });
            }
            
            return View(depositeHistory);
        }

        // GET: Deposites/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepositeHistory depositeHistory = db.DepositeHistories.Find(id);
            if (depositeHistory == null)
            {
                return HttpNotFound();
            }
            return View(depositeHistory);
        }

        // POST: Deposites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            DepositeHistory depositeHistory = db.DepositeHistories.Find(id);
            db.DepositeHistories.Remove(depositeHistory);
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
