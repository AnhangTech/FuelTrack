using FuelTrack.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
        private ApplicationUserManager userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // GET: Deposites
        public ActionResult Index(long? accountId)
        {

            ViewBag.StationAccountId = accountId;

            var requests = db.PaymentRequests;

            if (accountId == null)
            {
                requests.Where(d => d.StationAccountId == accountId);
            }

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
            return View(new PaymentRequestDetailsViewModel()
            {
                Amount = paymentRequest.Amount,
                BankAccountName = paymentRequest.BankAccountName,
                BankAccountNumber = paymentRequest.BankAccountNumber,
                BankBranch = paymentRequest.BankBranch,
                BusinessManager = paymentRequest.BusinessManagerId == null ? string.Empty : userManager.FindById(paymentRequest.BusinessManagerId).UserName,
                BusinessManagerComments = paymentRequest.BusinessManagerComments,
                BusinessManagerCommentsTimestamp = paymentRequest.BusinessManagerCommentsTimestamp,
                Employee = paymentRequest.EmployeeId == null ? string.Empty : userManager.FindById(paymentRequest.EmployeeId).UserName,
                FinanceManager = userManager.FindById(paymentRequest.FinanceManagerId).UserName,
                FinanceManagerComments = paymentRequest.FinanceManagerComments,
                FinanceManagerCommentsTimestamp = paymentRequest.FinanceManagerCommentsTimestamp,
                PaymentRequestId = paymentRequest.PaymentRequestId,
                Reason = paymentRequest.Reason,
                StartTimestamp = paymentRequest.StartTimestamp,
                State = paymentRequest.State,
                StationAccountId = paymentRequest.StationAccountId,
                WithdrawedTimestamp = paymentRequest.WithdrawedTimestamp,
                Notes = paymentRequest.Notes,
                Station = paymentRequest.Station
            }
            );
        }

        // GET: PaymentRequest
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
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
                        EmployeeId = System.Web.HttpContext.Current.User.Identity.GetUserId(),
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
        [Authorize(Roles = "FinanceManager")]
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

            if (paymentRequest.State != PaymentRequestState.Start)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "请款单不是Start状态.");
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
                Employee = userManager.FindById(paymentRequest.EmployeeId).UserName,
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
        [Authorize(Roles = "FinanceManager")]
        public ActionResult FinanceApprove([Bind(Include = "StationAccountId, PaymentRequestId, FinanceManagerComments, IsApprove")] PaymentRequestFinanceManagerApproveViewModel financeApprove)
        {
            if (ModelState.IsValid)
            {
                PaymentRequest request = db.PaymentRequests.Find(financeApprove.PaymentRequestId);

                if (request.State != PaymentRequestState.Start)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "请款单不是Start状态.");
                }

                request.FinanceManagerComments = financeApprove.FinanceManagerComments;

                if (financeApprove.IsApprove == "Yes")
                {
                    request.State = PaymentRequestState.FinanceManagerApproved;
                }
                else
                {
                    request.State = PaymentRequestState.FinanceManagerRejected;
                }

                request.FinanceManagerCommentsTimestamp = DateTime.Now;
                request.FinanceManagerId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                db.Entry(request).State = EntityState.Modified;

                db.SaveChanges();
            }

            return RedirectToAction("Index", new { accountId = financeApprove.StationAccountId });
        }

        // GET: PaymentRequest
        [Authorize(Roles = "BusinessManager")]
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

            if (paymentRequest.State != PaymentRequestState.FinanceManagerApproved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "请款单必须是FinanceManagerApproved状态,才能进行业务总监批准.");
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
                Employee = userManager.FindById(paymentRequest.EmployeeId).UserName,
                FinanceManager = userManager.FindById(paymentRequest.FinanceManagerId).UserName,
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
        [Authorize(Roles = "BusinessManager")]
        public ActionResult BusinessApprove([Bind(Include = "StationAccountId, PaymentRequestId, BusinessManagerComments, IsApprove")] PaymentRequestBusinessManagerApproveViewModel businessApprove)
        {
            if (ModelState.IsValid)
            {
                PaymentRequest request = db.PaymentRequests.Find(businessApprove.PaymentRequestId);

                DateTime now = DateTime.Now;

                if (request.State != PaymentRequestState.FinanceManagerApproved)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "请款单不是FinanceManagerApproved状态.");
                }

                request.BusinessManagerComments = businessApprove.BusinessManagerComments;

                if (businessApprove.IsApprove == "Yes")
                {
                    request.State = PaymentRequestState.BusinessManagerApproved;                
                    
                    // Add Deposite logs
                    StationAccount account = db.StationAccounts.Find(businessApprove.StationAccountId);
                    account.Deposite += request.Amount;

                    var depositeHistory = new DepositeHistory()
                    {
                        StationAccountId = businessApprove.StationAccountId
                    };

                    depositeHistory.Timestamp = now;
                    depositeHistory.ChangeType = DepositeChangeType.Recharge;
                    depositeHistory.Amount = request.Amount;
                    db.DepositeHistories.Add(depositeHistory);

                    db.Entry(account).State = EntityState.Modified;
                }
                else
                {
                    request.State = PaymentRequestState.BusinessManagerRejected;
                }

                request.BusinessManagerCommentsTimestamp = now;
                request.BusinessManagerId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                db.Entry(request).State = EntityState.Modified;

                db.SaveChanges();
            }

            return RedirectToAction("Index", new { accountId = businessApprove.StationAccountId });
        }


        [Authorize(Roles = "Employee")]
        public ActionResult Withdraw(long? id)
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

            if (paymentRequest.State == PaymentRequestState.BusinessManagerApproved
                || paymentRequest.State == PaymentRequestState.BusinessManagerRejected
                || paymentRequest.State == PaymentRequestState.FinanceManagerRejected)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "请款单批准已完结,无法撤回.");
            }

            return View(new PaymentRequestDetailsViewModel()
            {
                Amount = paymentRequest.Amount,
                BankAccountName = paymentRequest.BankAccountName,
                BankAccountNumber = paymentRequest.BankAccountNumber,
                BankBranch = paymentRequest.BankBranch,
                BusinessManager = paymentRequest.BusinessManagerId == null ? string.Empty : userManager.FindById(paymentRequest.BusinessManagerId).UserName,
                BusinessManagerComments = paymentRequest.BusinessManagerComments,
                BusinessManagerCommentsTimestamp = paymentRequest.BusinessManagerCommentsTimestamp,
                Employee = userManager.FindById(paymentRequest.EmployeeId).UserName,
                FinanceManager = paymentRequest.FinanceManagerId == null ? string.Empty : userManager.FindById(paymentRequest.FinanceManagerId).UserName,
                FinanceManagerComments = paymentRequest.FinanceManagerComments,
                FinanceManagerCommentsTimestamp = paymentRequest.FinanceManagerCommentsTimestamp,
                PaymentRequestId = paymentRequest.PaymentRequestId,
                Reason = paymentRequest.Reason,
                StartTimestamp = paymentRequest.StartTimestamp,
                State = paymentRequest.State,
                StationAccountId = paymentRequest.StationAccountId,
                WithdrawedTimestamp = paymentRequest.WithdrawedTimestamp,
                Notes = paymentRequest.Notes,
                Station = paymentRequest.Station
            }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult WithdrawSave([Bind(Include = "StationAccountId, PaymentRequestId, WithdrawedComments")] PaymentRequestDetailsViewModel withDrawDetails)
        {
            if (ModelState.IsValid)
            {
                PaymentRequest request = db.PaymentRequests.Find(withDrawDetails.PaymentRequestId);

                if (request.State == PaymentRequestState.BusinessManagerApproved
                    || request.State == PaymentRequestState.BusinessManagerRejected
                    || request.State == PaymentRequestState.FinanceManagerRejected)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "请款单批准已完结,无法撤回.");
                }

                request.State = PaymentRequestState.Withdrawed;

                request.WithdrawedComments = withDrawDetails.WithdrawedComments;

                request.WithdrawedTimestamp = DateTime.Now;

                request.BusinessManagerId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                db.Entry(request).State = EntityState.Modified;

                db.SaveChanges();
            }

            return RedirectToAction("Index", new { accountId = withDrawDetails.StationAccountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        [MultipleButton(Name = "action", Argument = "Cancel")]
        public ActionResult WithdrawCancel([Bind(Include = "StationAccountId")] PaymentRequestDetailsViewModel withDrawDetails)
        {
            return RedirectToAction("Index", new { accountId = withDrawDetails.StationAccountId });
        }
    }
}