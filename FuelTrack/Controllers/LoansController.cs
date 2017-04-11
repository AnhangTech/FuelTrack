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
    public class LoansController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        // GET: Loans
        public ActionResult Index(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClientAccountId = accountId;

            return View(db.Loans.OrderByDescending(cb => cb.Timestamp).ToList().Where(s => s.Client.ClientAccountId == accountId));
        }

        // GET: Loans/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var loan = new Loan()
            {
                ClientAccountId = accountId.Value
            };

            return View(loan);
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientAccountId,StartAmount,StartDate,FreeDays,InterestRate, Notes")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                DateTime time = DateTime.Now;
                loan.Timestamp = time;
                loan.EndDate = null;
                loan.CurrentAmount = loan.StartAmount;

                db.Loans.Add(loan);

                var client = db.ClientAccounts.Find(loan.ClientAccountId);

                if (client.Loan + loan.StartAmount > client.LoanLimit)
                {
                    throw new InvalidOperationException("超出贷款上限");
                }

                client.Loan += loan.StartAmount;

                db.Entry(client).State = EntityState.Modified;

                db.SaveChanges();

                client.Balance += loan.StartAmount;

                var clientBalanceHistory = new ClientBalanceHistory()
                {
                    Amount = loan.StartAmount,
                    BalanceType = BalanceChangeType.Loan,
                    ClientAccountId = client.ClientAccountId,
                    Description = string.Format("Loan for LoanId: {0}.", loan.LoanId),
                    Timestamp = time
                };

                db.ClientBalanceHistories.Add(clientBalanceHistory);

                var loanHistory = new LoanHistory()
                {
                    Amount = loan.StartAmount,
                    DealDate = loan.StartDate,
                    ChangeType = LoanChangeType.Loan,
                    Interest = 0,
                    LoanId = loan.LoanId,
                    Notes = loan.Notes,
                    Timestamp = time
                };

                db.LoanHistories.Add(loanHistory);
                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = loan.ClientAccountId });
            }

            return View(loan);
        }

        // GET: Loans/Edit/5
        public ActionResult Repay(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);

            if (loan == null)
            {
                return HttpNotFound();
            }

            LoanRepayViewModel model = new LoanRepayViewModel()
            {
                ClientAccountId = loan.ClientAccountId,
                ClientAccountName = loan.Client.ClientAccountName,
                CurrentAmount = loan.CurrentAmount,
                FreeDays = loan.FreeDays,
                InterestRate = loan.InterestRate,
                CurrentInterest = GetRepayInterest(loan.CurrentAmount, loan.InterestRate, loan.StartDate, DateTime.Now.Date, loan.FreeDays),
                LoanId = loan.LoanId,
                StartAmount = loan.StartAmount,
                StartDate = loan.StartDate
            };

            return View(model);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Repay")]
        [ValidateAntiForgeryToken]
        public ActionResult RepayConfirmed([Bind(Include = "LoanId,RepayDate, RepayAmount, Notes")] LoanRepayViewModel loanRepayViewModel)
        {
            DateTime time = DateTime.Now;

            if (ModelState.IsValid)
            {
                var loan = db.Loans.Find(loanRepayViewModel.LoanId);

                if (loan == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                double totalRepayAmount = GetRepayAmountWithInterest(
                    loanRepayViewModel.RepayAmount,
                    loan.InterestRate,
                    loan.StartDate,
                    loanRepayViewModel.RepayDate,
                    loan.FreeDays);

                var client = db.ClientAccounts.Find(loan.ClientAccountId);

                if (client == null)
                {
                    return HttpNotFound();
                }

                if (client.Balance < totalRepayAmount)
                {
                    return View(string.Format("The balance is not enough to pay the loan with interest {0}.", totalRepayAmount));
                }

                client.Loan -= loanRepayViewModel.RepayAmount;
                client.Balance -= totalRepayAmount;
                db.Entry(client).State = EntityState.Modified;

                var loanHistory = new LoanHistory()
                {
                    Amount = loanRepayViewModel.RepayAmount * -1,
                    DealDate = loanRepayViewModel.RepayDate,
                    Interest = totalRepayAmount - loanRepayViewModel.RepayAmount,
                    ChangeType = LoanChangeType.Repay,
                    LoanId = loan.LoanId,
                    Notes = loan.Notes,
                    Timestamp = time
                };

                db.LoanHistories.Add(loanHistory);

                loan.CurrentAmount -= loanRepayViewModel.RepayAmount;
                loan.Timestamp = time;

                if (loan.CurrentAmount == 0)
                {
                    loan.EndDate = loanRepayViewModel.RepayDate;
                }

                db.Entry(loan).State = EntityState.Modified;

                var clientBalanceHistory = new ClientBalanceHistory()
                {
                    Amount = totalRepayAmount * -1,
                    BalanceType = BalanceChangeType.LoanRepay,
                    ClientAccountId = client.ClientAccountId,
                    Description = string.Format("Repay for Loan: {0}.", loan.LoanId),
                    Timestamp = time
                };

                db.ClientBalanceHistories.Add(clientBalanceHistory);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    string errorMessage = "";

                    foreach (var result in e.EntityValidationErrors)
                    {
                        foreach(var error in result.ValidationErrors)
                        {
                            errorMessage += error.ErrorMessage;
                        }
                    }

                    throw new Exception(errorMessage);
                }
                return RedirectToAction("Index", new { accountId = client.ClientAccountId });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Loans/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Loan loan = db.Loans.Find(id);
        //    if (loan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(loan);
        //}

        //// POST: Loans/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    Loan loan = db.Loans.Find(id);
        //    db.Loans.Remove(loan);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private double GetRepayAmountWithInterest(double amount, double interestRate, DateTime startDate, DateTime repayDate, int freeDays)
        {
            return Math.Round(amount + GetRepayInterest(amount, interestRate, startDate, repayDate, freeDays), 2);
        }

        private double GetRepayInterest(double amount, double interestRate, DateTime startDate, DateTime repayDate, int freeDays)
        {
            int interestDays = repayDate.Date.Subtract(startDate.Date).Days - freeDays;
            double insterest = 0;

            if (interestDays > 0)
            {
                double dailyInterestRate = interestRate * interestDays / 365.0 / 100.0;
                insterest = Math.Round(amount * dailyInterestRate, 2);
            }

            return insterest;
        }
    }
}
