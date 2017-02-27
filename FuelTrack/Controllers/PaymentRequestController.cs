using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Controllers
{
    [Authorize]
    public class PaymentRequestController : Controller
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

            var requests = db.PaymentRequests.Where(d => d.StationAccountId == accountId);

            return View(requests.ToList().OrderByDescending(o => o.StartTimestamp));
        }

        // GET: Deposites/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var paymentRequest = db.PaymentRequests.Find(id);

            if (paymentRequest == null)
            {
                return HttpNotFound();
            }
            return View(paymentRequest);
        }

        // GET: PaymentRequest
        public ActionResult Apply(long? stationId)
        {
            if (stationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StationAccount stationAccount = db.StationAccounts.Find(stationId.Value);

            if (stationAccount == null)
            {
                return HttpNotFound("Station not found.");
            }

            PaymentRequestApplicationViewModel model = new PaymentRequestApplicationViewModel()
            {
                StationAccountId = stationId.Value,
                Station = stationAccount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply([Bind(Include = "StationAccountId, Amount, Reason, BankBranch, BankAccountName, BankAccountNumber")] PaymentRequestApplicationViewModel paymentRequest)
        {
            if (ModelState.IsValid)
            {
                db.PaymentRequests.Add(
                    new PaymentRequest()
                    {
                         StationAccountId = paymentRequest.StationAccountId,
                         Amount = paymentRequest.Amount,
                         Reason = paymentRequest.Reason,
                         StartTimestamp = DateTime.Now,
                         BankBranch = paymentRequest.BankBranch,
                         BankAccountName = paymentRequest.BankAccountName,
                         BankAccountNumber = paymentRequest.BankAccountNumber,
                         State = PaymentRequestState.Start
                    }
                    );
                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = paymentRequest.StationAccountId });
            }

            return View(paymentRequest.StationAccountId);
        }
    }
}