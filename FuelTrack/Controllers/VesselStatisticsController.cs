using FuelTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelTrack.Controllers
{
    public class VesselStatisticsController : Controller
    {
        FuelTrackContext context = new FuelTrackContext();

        //public const string AllVesselName = "全部船舶";

        // GET: ClientStatistics
        public ActionResult Query()
        {
            var vessels = context.ClientSubscriptionHistories.Where(
                csh => (csh.State == ClientSubscriptionState.PartialDelivered || csh.State == ClientSubscriptionState.Delivered))
                .Select(v => v.VesselName).Distinct();

            var vesselOptions = new SelectList(
                vessels.Select(
                    n => new SelectListItem()
                    {
                        Selected = false,
                        Text = n,
                        Value = n
                    }).ToList(),
                "Value",
                "Text",
                0);

            var model = new VesselStatisticsViewModel()
            {
                VesselList = vesselOptions
            };

            return View(model);
        }

        // POST: VesselViewModel/Query
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query([Bind(Include = "VesselName, StartDate, EndDate")] VesselStatisticsViewModel vesselStatistics)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(
                    "Show",
                    new
                    {
                        vesselName = vesselStatistics.VesselName,
                        startDate = vesselStatistics.StartDate,
                        endDate = vesselStatistics.EndDate
                    });
            }
            else
            {
                throw new InvalidOperationException("非法请求");
            }
        }

        public ActionResult Show(string vesselName, DateTime startDate, DateTime endDate)
        {
            var allStatistics = new VesselStatisticsViewModel();

            DateTime endRange = endDate.AddDays(1);
            // The statistics for all the clients

            var history = context.ClientSubscriptionHistories;

            allStatistics.VesselName = vesselName;
            allStatistics.StartDate = startDate;
            allStatistics.EndDate = endDate;

            //var result = context.ClientSubscriptionHistories.Where(
            //    csh => csh.Timestamp >= startDate && csh.Timestamp < endRange && csh.VesselName == vesselName).Distinct();

            var result = (from csh in context.ClientSubscriptionHistories
                          join cs in context.ClientSubscriptions on csh.ClientSubscriptionId equals cs.ClientSubscriptionId
                          where csh.Timestamp >= startDate && csh.Timestamp < endRange && csh.VesselName == vesselName
                          select cs).Distinct();

            allStatistics.TotalCount = result.Count();
            allStatistics.TotalQuantity = result.Sum(r => (double?)(r.StartQuantity) - (double?)(r.CurrentQuantity)) ?? 0.0;
            allStatistics.ClientSubscriptions = result.ToList();

            return View(allStatistics);
        }
    }
}