using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
                        EmployeeId = (string)Membership.GetUser().ProviderUserKey,
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

        // GET: PaymentRequest
        public ActionResult FinanceApprove(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PaymentRequest paymentRequest = db.PaymentRequests.Find(id.Value);

            if (paymentRequest == null)
            {
                return HttpNotFound("请款单未找到.");
            }

            var yesOrNo = new SelectList(
                new List<SelectListItem>() {
                    new SelectListItem() { Text = "否", Value = "No" },
                    new SelectListItem() { Text = "是", Value = "Yes" } },
                "Value",
                "Text",
                0);

            PaymentRequestFinanceManagerApproveViewModel model = new PaymentRequestFinanceManagerApproveViewModel()
            {
                Amount = paymentRequest.Amount,
                BankAccountName = paymentRequest.BankAccountName,
                BankAccountNumber = paymentRequest.BankAccountNumber,
                BankBranch = paymentRequest.BankBranch,
                Employee = Membership.GetUser(Guid.Parse(paymentRequest.EmployeeId)).UserName,
                PaymentRequestId = paymentRequest.PaymentRequestId,
                Reason = paymentRequest.Reason,
                StartTimestamp = paymentRequest.StartTimestamp,
                StationAccountId = paymentRequest.StationAccountId,
                Station = db.StationAccounts.Find(paymentRequest.StationAccountId),
                YesOrNo = yesOrNo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinanceApprove([Bind(Include = "StationAccountId, PaymentRequestId, FinanceManagerComments, IsApprove")] PaymentRequestFinanceManagerApproveViewModel financeApprove)
        {
            if (ModelState.IsValid)
            {
                PaymentRequest request = db.PaymentRequests.Find(financeApprove.PaymentRequestId);

                request.FinanceManagerComments = financeApprove.FinanceManagerComments;

                if(financeApprove.IsApprove == "Yes")
                {
                    request.State = PaymentRequestState.FinanceManagerApproved;
                }
                else
                {
                    request.State = PaymentRequestState.FinanceManagerRejected;
                }

                request.FinanceManagerCommentsTimestamp = DateTime.Now;
                request.FinanceManagerId = (string)Membership.GetUser().ProviderUserKey;          

                db.Entry(request).State = EntityState.Modified;

                db.SaveChanges();
            }

            return RedirectToAction("Index", new { accountId = financeApprove.StationAccountId });
        }

        // GET: PaymentRequest
        public ActionResult BusinessApprove(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PaymentRequest paymentRequest = db.PaymentRequests.Find(id.Value);

            if (paymentRequest == null)
            {
                return HttpNotFound("请款单未找到.");
            }

            var yesOrNo = new SelectList(
                new List<SelectListItem>() {
                    new SelectListItem() { Text = "否", Value = "No" },
                    new SelectListItem() { Text = "是", Value = "Yes" } },
                "Value",
                "Text",
                0);

            PaymentRequestBusinessManagerApproveViewModel model = new PaymentRequestBusinessManagerApproveViewModel()
            {
                Amount = paymentRequest.Amount,
                BankAccountName = paymentRequest.BankAccountName,
                BankAccountNumber = paymentRequest.BankAccountNumber,
                BankBranch = paymentRequest.BankBranch,
                Employee = Membership.GetUser(Guid.Parse(paymentRequest.EmployeeId)).UserName,
                FinanceManager = Membership.GetUser(Guid.Parse(paymentRequest.FinanceManagerId)).UserName,
                FinanceManagerComments = paymentRequest.FinanceManagerComments,
                FinanceManagerCommentsTimestamp = paymentRequest.FinanceManagerCommentsTimestamp,
                PaymentRequestId = paymentRequest.PaymentRequestId,
                Reason = paymentRequest.Reason,
                StartTimestamp = paymentRequest.StartTimestamp,
                StationAccountId = paymentRequest.StationAccountId,
                Station = db.StationAccounts.Find(paymentRequest.StationAccountId),
                YesOrNo = yesOrNo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BusinessApprove([Bind(Include = "StationAccountId, PaymentRequestId, BusinessManagerComments, IsApprove")] PaymentRequestBusinessManagerApproveViewModel businessApprove)
        {
            if (ModelState.IsValid)
            {
                PaymentRequest request = db.PaymentRequests.Find(businessApprove.PaymentRequestId);

                request.BusinessManagerComments = businessApprove.FinanceManagerComments;

                if (businessApprove.IsApprove == "Yes")
                {
                    request.State = PaymentRequestState.BusinessManagerApproved;
                }
                else
                {
                    request.State = PaymentRequestState.BusinessManagerApproved;
                }

                request.BusinessManagerCommentsTimestamp = DateTime.Now;


                db.Entry(request).State = EntityState.Modified;

                db.SaveChanges();
            }

            return RedirectToAction("Index", new { accountId = businessApprove.StationAccountId });
        }
    }
}