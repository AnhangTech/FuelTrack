using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Controllers
{
    public class ClientStatisticsController : Controller
    {
        FuelTrackContext context = new FuelTrackContext();

        public const long AllClientAccountId = 0;
        public const string AllClientAccountName = "全部用户";

        // GET: ClientStatistics
        public ActionResult Query()
        {
            var accounts = context.ClientAccounts;

            var stationOptions = new SelectList(
                accounts.Select(
                    n => new SelectListItem()
                    {
                        Selected = false,
                        Text = n.ClientAccountName,
                        Value = n.ClientAccountId.ToString()
                    }).ToList().Concat(new List<SelectListItem>() { new SelectListItem() { Text = AllClientAccountName, Value = AllClientAccountId.ToString() } }),
                "Value",
                "Text",
                0);

            var model = new ClientStatisticsViewModel()
            {
                ClientAccountList = stationOptions
            };

            return View(model);
        }

        // POST: ClientStatisticsViewModel/Query
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query([Bind(Include = "ClientAccountId, StartDate, EndDate")] ClientStatisticsViewModel clientStatistics)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(
                    "Show",
                    new
                    {
                        accountId = clientStatistics.ClientAccountId,
                        startDate = clientStatistics.StartDate,
                        endDate = clientStatistics.EndDate
                    });
            }
            else
            {
                throw new InvalidOperationException("非法请求");
            }
        }

        public ActionResult Show(long accountId, DateTime startDate, DateTime endDate)
        {
            var allStatistics = new ClientStatisticsViewModel();

            // The statistics for all the clients
            if (accountId == AllClientAccountId)
            {
                var accounts = context.ClientAccounts;

                allStatistics.ClientAccountId = ClientStatisticsController.AllClientAccountId;
                allStatistics.ClientAccountName = ClientStatisticsController.AllClientAccountName;
                allStatistics.StartDate = startDate;
                allStatistics.EndDate = endDate;
                allStatistics.ClientSubscriptions = new List<ClientSubscription>();


                foreach (var account in accounts)
                {
                    var statistics = GetClientStatistics(startDate, endDate, account);

                    allStatistics.TotalIncompleteQuantity += statistics.TotalIncompleteQuantity;
                    allStatistics.TotalIncreasedBalance += statistics.TotalIncreasedBalance;
                    allStatistics.TotalIncreasedLoan += statistics.TotalIncreasedLoan;
                    allStatistics.TotalIncreasedSubscriptionAmount += statistics.TotalIncreasedSubscriptionAmount;
                    allStatistics.TotalPaidAmount += statistics.TotalPaidAmount;
                    allStatistics.TotalQuantity += statistics.TotalQuantity;
                    allStatistics.ClientSubscriptions.AddRange(statistics.ClientSubscriptions);
                    allStatistics.Profit += statistics.Profit;
                }
            }
            else
            {
                var account = context.ClientAccounts.Find(accountId);

                allStatistics = GetClientStatistics(startDate, endDate, account);
            }

            return View(allStatistics);
        }

        private ClientStatisticsViewModel GetClientStatistics(DateTime startDate, DateTime endDate, ClientAccount account)
        {
            DateTime endRange = endDate.AddDays(1);

            double balanceAmount = (from bh in context.ClientBalanceHistories
                                    where bh.ClientAccountId == account.ClientAccountId
                                    && bh.Timestamp < endRange
                                    && bh.Timestamp >= startDate
                                    select (double?)bh.Amount).Sum() ?? 0.0;

            double loan = (from lh in context.LoanHistories
                           join l in context.Loans on lh.LoanId equals l.LoanId
                           where l.ClientAccountId == account.ClientAccountId
                                    && lh.DealDate < endRange
                                    && lh.DealDate >= startDate
                           select (double?)lh.Amount).Sum() ?? 0.0;

            double quantity = (from cs in context.ClientSubscriptions
                               join
                                     csh in context.ClientSubscriptionHistories
                                     on cs.ClientSubscriptionId equals csh.ClientSubscriptionId
                               where cs.ClientAccountId == account.ClientAccountId
                               && csh.Timestamp >= startDate
                                  && csh.Timestamp < endRange
                                  && (csh.State == ClientSubscriptionState.Paid)
                               select (double?)csh.Quantity).Sum() ?? 0.0;

            var allQuantities = (from cs in context.ClientSubscriptions
                                 join
                                       csh in context.ClientSubscriptionHistories
                                       on cs.ClientSubscriptionId equals csh.ClientSubscriptionId
                                 where cs.ClientAccountId == account.ClientAccountId
                                    && csh.Timestamp < endRange
                                    && csh.State == ClientSubscriptionState.Paid &&
                                          (cs.State == ClientSubscriptionState.Paid || cs.State == ClientSubscriptionState.PartialDelivered)
                                 select (double?)csh.Quantity).Sum() ?? 0.0;

            var deliveredQuantities = (from cs in context.ClientSubscriptions
                                       join
                                             csh in context.ClientSubscriptionHistories
                                             on cs.ClientSubscriptionId equals csh.ClientSubscriptionId
                                       where cs.ClientAccountId == account.ClientAccountId
                                          && csh.Timestamp < endRange
                                          && csh.State == ClientSubscriptionState.PartialDelivered &&
                                          (cs.State == ClientSubscriptionState.Paid || cs.State == ClientSubscriptionState.PartialDelivered)
                                       select (double?)csh.Quantity).Sum() ?? 0.0;


            var refundedQuantities = (from cs in context.ClientSubscriptions
                                      join csh in context.ClientSubscriptionHistories
                                      on cs.ClientSubscriptionId equals csh.ClientSubscriptionId
                                      where cs.ClientAccountId == account.ClientAccountId
                                      && csh.Timestamp < endRange
                                          && csh.State == ClientSubscriptionState.Refunded
                                      select (double?)csh.Quantity).Sum() ?? 0.0;

            quantity = quantity - refundedQuantities;

            double incompleteQuantity = allQuantities - deliveredQuantities;

            double payment = -1 * ((from bh in context.ClientBalanceHistories
                                    where bh.ClientAccountId == account.ClientAccountId
                                             && bh.Timestamp < endRange
                                             && bh.Timestamp >= startDate
                                             && (bh.BalanceType == BalanceChangeType.Pay)
                                    select (double?)bh.Amount).Sum() ?? 0.0);

            double refunded = -1 * ((from bh in context.ClientBalanceHistories
                                     where bh.ClientAccountId == account.ClientAccountId
                                              && bh.Timestamp < endRange
                                              && bh.Timestamp >= startDate
                                              && (bh.BalanceType == BalanceChangeType.Refund)
                                     select (double?)bh.Amount).Sum() ?? 0.0);

            payment = payment + refunded;

            double amount = (from cs in context.ClientSubscriptions
                             join csh in context.ClientSubscriptionHistories
                             on cs.ClientSubscriptionId equals csh.ClientSubscriptionId
                             where cs.ClientAccountId == account.ClientAccountId
                             && csh.Timestamp < endRange
                                  && csh.Timestamp >= startDate && csh.State == ClientSubscriptionState.Paid
                             select (double?)(csh.Quantity * csh.UnitPrice)).Sum() ?? 0.0;

            amount = amount + refunded;

            var result = (from csh in context.ClientSubscriptionHistories
                          join cs in context.ClientSubscriptions on csh.ClientSubscriptionId equals cs.ClientSubscriptionId
                          where csh.Timestamp >= startDate && csh.Timestamp < endRange && cs.ClientAccountId == account.ClientAccountId
                          select cs).Distinct().ToList();

            double profit = (from sh in context.SubscriptionHistories
                             join cs in context.ClientSubscriptions
                             on sh.ClientSubscriptionId equals cs.ClientSubscriptionId
                             where sh.ClientSubscriptionId != null && sh.Timestamp >= startDate && sh.Timestamp < endRange && cs.ClientAccountId == account.ClientAccountId
                             select ((double?)cs.UnitPrice - (double?)sh.UnitPrice) * (float?)sh.Quantity).Sum() ?? 0;

            return new ClientStatisticsViewModel()
            {
                ClientAccountId = account.ClientAccountId,
                ClientAccountName = account.ClientAccountName,
                StartDate = startDate,
                EndDate = endDate,
                TotalIncompleteQuantity = incompleteQuantity,
                TotalIncreasedBalance = balanceAmount,
                TotalIncreasedLoan = loan,
                TotalIncreasedSubscriptionAmount = amount,
                TotalPaidAmount = payment,
                TotalQuantity = quantity,
                ClientSubscriptions = result,
                Profit = profit
            };
        }
    }
}