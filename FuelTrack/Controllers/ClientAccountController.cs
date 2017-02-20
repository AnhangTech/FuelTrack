using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Controllers
{
    public class ClientAccountController : Controller
    {
        FuelTrackContext context = new FuelTrackContext();

        // GET: ClientAccount
        public ActionResult Index()
        {
            return View(context.ClientAccounts);
        }

        // GET: ClientAccount/Details/5
        [HandleError(View="Error")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                throw new Exception("必须输入客户编号。");
            }
            ClientAccount clientAccounts = context.ClientAccounts.Find(id);
            if (clientAccounts == null)
            {
                return HttpNotFound();
            }

            clientAccounts.BalanceHistory = clientAccounts.BalanceHistory.OrderByDescending(cb => cb.Timestamp).Take(10).ToList();

            return View(clientAccounts);
        }

        // GET: ClientAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientAccount/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "ClientAccountName,LoanLimit", Exclude = "Balance")]ClientAccount account)
        {
            if (ModelState.IsValid)
            {

                account.Balance = 0.0;
                context.ClientAccounts.Add(account);

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: ClientAccount/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientAccount clientAccounts = context.ClientAccounts.Find(id);
            if (clientAccounts == null)
            {
                return HttpNotFound();
            }
            return View(clientAccounts);
        }

        // POST: ClientAccount/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, [Bind(Include = "ClientAccountName,LoanLimit", Exclude = "Balance")]ClientAccount account)
        {
            if (ModelState.IsValid)
            {
                ClientAccount clientAccount = context.ClientAccounts.Find(id);

                if (clientAccount == null)
                {
                    return HttpNotFound();
                }

                clientAccount.ClientAccountName = account.ClientAccountName;
                clientAccount.LoanLimit = account.LoanLimit;

                context.Entry(clientAccount).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: ClientAccount/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ClientAccount clientAccounts = context.ClientAccounts.Find(id);
            if (clientAccounts == null)
            {
                return HttpNotFound();
            }
            return View(clientAccounts);
        }

        // POST: ClientSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientAccount clientAccount = context.ClientAccounts.Find(id);
            context.ClientAccounts.Remove(clientAccount);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
