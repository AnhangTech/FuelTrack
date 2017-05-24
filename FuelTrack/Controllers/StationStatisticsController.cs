using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Controllers
{
    public class StationStatisticsController : Controller
    {
        FuelTrackContext context = new FuelTrackContext();

        public const long AllStationAccountId = 0;
        public const string AllStationAccountName = "全部油站";

        // GET: ClientStatistics
        public ActionResult Query()
        {
            var accounts = context.StationAccounts;

            var stationOptions = new SelectList(
                accounts.Select(
                    n => new SelectListItem()
                    {
                        Selected = false,
                        Text = n.StationName,
                        Value = n.StationAccountId.ToString()
                    }).ToList().Concat(new List<SelectListItem>() { new SelectListItem() { Text = AllStationAccountName, Value = AllStationAccountId.ToString() } }),
                "Value",
                "Text",
                0);

            var model = new StationStatisticsViewModel()
            {
                StationAccountList = stationOptions
            };

            return View(model);
        }

        // POST: ClientStatisticsViewModel/Query
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query([Bind(Include = "StationAccountId, StartDate, EndDate")] StationStatisticsViewModel stationStatistics)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(
                    "Show",
                    new
                    {
                        accountId = stationStatistics.StationAccountId,
                        startDate = stationStatistics.StartDate,
                        endDate = stationStatistics.EndDate
                    });
            }
            else
            {
                throw new InvalidOperationException("非法请求");
            }
        }

        public ActionResult Show(long accountId, DateTime startDate, DateTime endDate)
        {
            var allStatistics = new StationStatisticsViewModel();

            // The statistics for all the clients
            if (accountId == AllStationAccountId)
            {
                var accounts = context.StationAccounts;

                allStatistics.StationAccountId = StationStatisticsController.AllStationAccountId;
                allStatistics.StationAccountName = StationStatisticsController.AllStationAccountName;
                allStatistics.StartDate = startDate;
                allStatistics.EndDate = endDate;

                foreach (var account in accounts)
                {
                    var statistics = GetStationStatistics(startDate, endDate, account);

                    allStatistics.TotalIncompleteQuantity += statistics.TotalIncompleteQuantity;
                    allStatistics.TotalIncreasedBalance += statistics.TotalIncreasedBalance;
                    allStatistics.TotalIncreasedLoan += statistics.TotalIncreasedLoan;
                    allStatistics.TotalIncreasedSubscriptionAmount += statistics.TotalIncreasedSubscriptionAmount;
                    allStatistics.TotalPaidAmount += statistics.TotalPaidAmount;
                    allStatistics.TotalQuantity += statistics.TotalQuantity;
                }
            }
            else
            {
                var account = context.StationAccounts.Find(accountId);

                allStatistics = GetStationStatistics(startDate, endDate, account);
            }

            return View(allStatistics);
        }

        private StationStatisticsViewModel GetStationStatistics(DateTime startDate, DateTime endDate, StationAccount account)
        {
            DateTime endRange = endDate.AddDays(1);

            double balanceAmount = (from dh in context.DepositeHistories
                                    where dh.StationAccountId == account.StationAccountId
                                    && dh.Timestamp < endRange
                                    && dh.Timestamp >= startDate
                                    select (double?)dh.Amount).Sum() ?? 0.0;

            double loan = (from dh in context.DepositeHistories
                           where dh.StationAccountId == account.StationAccountId
                                    && dh.Timestamp < endRange
                           select (double?)dh.Amount).Sum() ?? 0.0;

            loan = loan < 0 ? -1 * loan : 0;

            double quantity = (from s in context.Subscriptions
                               join
                                     sh in context.SubscriptionHistories
                                     on s.SubscriptionId equals sh.SubscriptionId
                               where s.StationAccountId == account.StationAccountId
                               && sh.Timestamp >= startDate
                                  && sh.Timestamp < endRange
                                  && (sh.State == SubscriptionState.Paid)
                               select (double?)sh.Quantity).Sum() ?? 0.0;
            double refundQuantity = (from s in context.Subscriptions
                                     join
                                           sh in context.SubscriptionHistories
                                           on s.SubscriptionId equals sh.SubscriptionId
                                     where s.StationAccountId == account.StationAccountId
                                     && sh.Timestamp >= startDate
                                        && sh.Timestamp < endRange
                                        && (sh.State == SubscriptionState.Refunded)
                                     select (double?)sh.Quantity).Sum() ?? 0.0;

            quantity = quantity - refundQuantity;


            var allQuantities = (from s in context.Subscriptions
                                 join
                                       sh in context.SubscriptionHistories
                                       on s.SubscriptionId equals sh.SubscriptionId
                                 where s.StationAccountId == account.StationAccountId
                                    && sh.Timestamp < endRange
                                    && (sh.State == SubscriptionState.Paid) 
                                    //&& (s.State == SubscriptionState.Paid || s.State == SubscriptionState.PartialDelivered)
                                 select (double?)sh.Quantity).Sum() ?? 0.0;

            var deliveredQuantities = (from s in context.Subscriptions
                                       join
                                             sh in context.SubscriptionHistories
                                             on s.SubscriptionId equals sh.SubscriptionId
                                       where s.StationAccountId == account.StationAccountId
                                          && sh.Timestamp < endRange
                                          && (sh.State == SubscriptionState.PartialDelivered || sh.State == SubscriptionState.Delivered || sh.State == SubscriptionState.Refunded) &&
                                          (s.State == SubscriptionState.Paid || s.State == SubscriptionState.PartialDelivered || s.State== SubscriptionState.Refunded || s.State == SubscriptionState.Closed || s.State== SubscriptionState.Delivered)
                                       select (double?)sh.Quantity).Sum() ?? 0.0;

            double incompleteQuantity = allQuantities - deliveredQuantities;

            double payment = -1 * ((from dh in context.DepositeHistories
                                    where dh.StationAccountId == account.StationAccountId
                                         && dh.Timestamp < endRange
                                         && dh.Timestamp >= startDate
                                         && (dh.ChangeType == DepositeChangeType.Pay || dh.ChangeType == DepositeChangeType.Refund)
                                    select (double?)dh.Amount).Sum() ?? 0.0);

            double amount = (from s in context.Subscriptions
                             join
                                   sh in context.SubscriptionHistories
                   on s.SubscriptionId equals sh.SubscriptionId
                             where s.StationAccountId == account.StationAccountId
                             && sh.Timestamp < endRange
                                  && sh.Timestamp >= startDate && sh.State == SubscriptionState.Paid
                             select (double?)(sh.Quantity * sh.UnitPrice)).Sum() ?? 0.0;

            double refundAmount = (from s in context.Subscriptions
                                   join
                                         sh in context.SubscriptionHistories
                         on s.SubscriptionId equals sh.SubscriptionId
                                   where s.StationAccountId == account.StationAccountId
                                   && sh.Timestamp < endRange
                                        && sh.Timestamp >= startDate && sh.State == SubscriptionState.Refunded
                                   select (double?)(sh.Quantity * sh.UnitPrice)).Sum() ?? 0.0;

            amount = amount - refundAmount;

            return new StationStatisticsViewModel()
            {
                StationAccountId = account.StationAccountId,
                StationAccountName = account.StationName,
                StartDate = startDate,
                EndDate = endDate,
                TotalIncompleteQuantity = incompleteQuantity,
                TotalIncreasedBalance = balanceAmount,
                TotalIncreasedLoan = loan,
                TotalIncreasedSubscriptionAmount = amount,
                TotalPaidAmount = payment,
                TotalQuantity = quantity
            };
        }
    }
}