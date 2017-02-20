using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuelTrack.Models;
using System.Data.Entity.Validation;

namespace FuelTrack.Controllers
{
    public class ClientBalanceHistoriesController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        // GET: ClientBalanceHistories
        public ActionResult Index(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClientAccountId = accountId;

            return View(db.ClientBalanceHistories.OrderByDescending(cb => cb.Timestamp).ToList().Where(s => s.Client.ClientAccountId == accountId));
        }

        // GET: ClientBalanceHistories/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientBalanceHistory clientBalanceHistory = db.ClientBalanceHistories.Find(id);
            if (clientBalanceHistory == null)
            {
                return HttpNotFound();
            }
            return View(clientBalanceHistory);
        }

        // GET: ClientBalanceHistories/Create
        public ActionResult Create(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var balanceHistory = new ClientBalanceHistory()
            {
                ClientAccountId = accountId.Value
            };

            return View(balanceHistory);
        }

        // POST: ClientBalanceHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientAccountId, Amount,BalanceType,Description,Timestamp")] ClientBalanceHistory clientBalanceHistory)
        {
            if (ModelState.IsValid)
            {
                if(clientBalanceHistory.BalanceType == BalanceChangeType.Cashout)
                {
                    if(clientBalanceHistory.Amount > 0)
                    {
                        clientBalanceHistory.Amount = clientBalanceHistory.Amount * -1;
                    }
                }

                db.ClientBalanceHistories.Add(clientBalanceHistory);

                var client = db.ClientAccounts.Find(clientBalanceHistory.ClientAccountId);


                client.Balance += clientBalanceHistory.Amount;


                db.Entry(client).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = clientBalanceHistory.ClientAccountId });

            }

            return View(clientBalanceHistory);
        }

        // GET: ClientBalanceHistories/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientBalanceHistory clientBalanceHistory = db.ClientBalanceHistories.Find(id);
            if (clientBalanceHistory == null)
            {
                return HttpNotFound();
            }
            return View(clientBalanceHistory);
        }

        // POST: ClientBalanceHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientBalanceHistoryId,Amount,BalanceType,Description,Timestamp")] ClientBalanceHistory clientBalanceHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientBalanceHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientBalanceHistory);
        }

        // GET: ClientBalanceHistories/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientBalanceHistory clientBalanceHistory = db.ClientBalanceHistories.Find(id);
            if (clientBalanceHistory == null)
            {
                return HttpNotFound();
            }
            return View(clientBalanceHistory);
        }

        // POST: ClientBalanceHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientBalanceHistory clientBalanceHistory = db.ClientBalanceHistories.Find(id);
            var clientId = clientBalanceHistory.ClientAccountId;

            db.ClientBalanceHistories.Remove(clientBalanceHistory);

            var client = db.ClientAccounts.Find(clientId);
            client.Balance -= clientBalanceHistory.Amount;
            db.Entry(client).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { accountId = clientId });
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
