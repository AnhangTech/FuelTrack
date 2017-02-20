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
        public ActionResult Apply([Bind(Include = "StationAccountId, Amount, Reason")] PaymentRequestApplicationViewModel paymentRequest)
        {
            if (ModelState.IsValid)
            {
                db.PaymentRequests.Add(
                    new PaymentRequest()
                    {

                    }
                    );
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paymentRequest.StationAccountId);
        }
    }
}