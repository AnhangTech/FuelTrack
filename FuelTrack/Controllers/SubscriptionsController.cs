using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuelTrack.Models;

namespace FuelTrack.Controllers
{
    public class SubscriptionsController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        // GET: Subscriptions
        public ActionResult Index(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.StationAccountId = accountId;

            return View(db.Subscriptions.OrderByDescending(cs => cs.Timestamp).ToList().Where(s => s.Station.StationAccountId == accountId));
        }

        // GET: Subscriptions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // GET: Subscriptions/Create
        public ActionResult Create(long? accountId)
        {
            var account = db.StationAccounts.Find(accountId);

            if (account != null)
            {

                var model = new Subscription()
                {
                    StationAccountId = account.StationAccountId
                };

                return View(model);
            }

            return HttpNotFound();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubscriptionId,StationAccountId,StartQuantity,UnitPrice,Notes,Timestamp")] Subscription subscription)
        {
            DateTime time = subscription.Timestamp;

            if (ModelState.IsValid)
            {
                subscription.Timestamp = time;
                subscription.CurrentQuantity = subscription.StartQuantity;
                subscription.State = SubscriptionState.Created;

                db.Subscriptions.Add(subscription);
                db.SaveChanges();

                var subscriptionHistory = new SubscriptionHistory()
                {

                    UnitPrice = subscription.UnitPrice,
                    State = SubscriptionState.Created,
                    Quantity = subscription.StartQuantity,
                    Notes = subscription.Notes,
                    SubscriptionId = subscription.SubscriptionId,
                    Timestamp = time
                };

                db.SubscriptionHistories.Add(subscriptionHistory);
                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = subscription.StationAccountId });
            }

            ViewBag.StationAccountId = new SelectList(db.StationAccounts, "StationAccountId", "StationName", subscription.StationAccountId);
            return View(subscription);
        }

        // GET: Subscriptions/Pay/5
        public ActionResult Pay(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);

            if (subscription == null)
            {
                return HttpNotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Pay")]
        [ValidateAntiForgeryToken]
        public ActionResult PayConfirmed([Bind(Include = "SubscriptionId, Notes, Timestamp")]Subscription s)
        {
            if (ModelState.IsValid)
            {
                DateTime time = s.Timestamp;

                Subscription subscription = db.Subscriptions.Find(s.SubscriptionId);
                long accountId = subscription.StationAccountId;
                StationAccount station = db.StationAccounts.Find(accountId);


                subscription.State = SubscriptionState.Paid;
                subscription.Timestamp = time;
                db.Entry(subscription).State = EntityState.Modified;


                station.Deposite -= subscription.UnitPrice * subscription.StartQuantity;
                db.Entry(station).State = EntityState.Modified;

                var subscriptionHistory = new SubscriptionHistory()
                {
                    UnitPrice = subscription.UnitPrice,
                    State = SubscriptionState.Paid,
                    Quantity = subscription.StartQuantity,
                    Notes = subscription.Notes,
                    SubscriptionId = subscription.SubscriptionId,
                    Timestamp = time
                };

                db.SubscriptionHistories.Add(subscriptionHistory);

                var depositeHistory = new DepositeHistory()
                {
                    Amount = subscription.UnitPrice * subscription.StartQuantity * -1,
                    ChangeType = DepositeChangeType.Pay,
                    StationAccountId = accountId,
                    Description = string.Format("Pay for Subscription: {0}.", subscription.SubscriptionId),
                    Timestamp = time
                };

                db.DepositeHistories.Add(depositeHistory);

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = accountId });
            }

            return View("The state is not valid.");
        }


        public ActionResult Deliver(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            SubscriptionDeliverViewModel model = new SubscriptionDeliverViewModel()
            {
                StationAccountId = subscription.StationAccountId,
                SubscriptionId = subscription.SubscriptionId,
                StationAccountName = subscription.Station.StationName,
                Quantity = subscription.CurrentQuantity
            };

            return View(model);
        }


        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Deliver")]
        [ValidateAntiForgeryToken]
        public ActionResult DeliverConfirmed([Bind(Include = "SubscriptionId, Quantity,ClientSubscriptionId, Notes, Timestamp")] SubscriptionDeliverViewModel model)
        {
            if (ModelState.IsValid)
            {
                DateTime time = model.Timestamp;
                Subscription subscription = db.Subscriptions.Find(model.SubscriptionId);
                ClientSubscription cs = db.ClientSubscriptions.Find(model.ClientSubscriptionId);
                long csId = cs.ClientSubscriptionId;

                if (cs.State != ClientSubscriptionState.Paid)
                {
                    if (cs.State != ClientSubscriptionState.PartialDelivered)
                    {
                        throw new Exception("The client subscription state must be 'paid'.");
                    }
                }

                if (cs.StationAccountId != subscription.StationAccountId)
                {
                    throw new Exception("The station does not match.");
                }

                string clientAccountName = cs.Client.ClientAccountName;
                string vesselName = cs.VesselName;


                long accountId = subscription.StationAccountId;

                float leftQuantity = subscription.CurrentQuantity - model.Quantity;

                if (leftQuantity == 0)
                {
                    subscription.State = SubscriptionState.Delivered;
                }
                else
                {
                    subscription.State = SubscriptionState.PartialDelivered;
                }

                subscription.CurrentQuantity = leftQuantity;
                subscription.Timestamp = time;
                subscription.Notes = model.Notes;

                db.Entry(subscription).State = EntityState.Modified;

                var subscriptionHistory = new SubscriptionHistory()
                {

                    UnitPrice = subscription.UnitPrice,
                    State = subscription.State,
                    Quantity = model.Quantity,
                    Notes = model.Notes,
                    ClientSubscriptionId = model.ClientSubscriptionId,
                    ClientAccountName = clientAccountName,
                    VesselName = vesselName,
                    SubscriptionId = subscription.SubscriptionId,
                    Timestamp = time
                };

                db.SubscriptionHistories.Add(subscriptionHistory);

                float csLeftQuantity = cs.CurrentQuantity - model.Quantity;

                if (csLeftQuantity == 0)
                {
                    cs.State = ClientSubscriptionState.Delivered;
                }
                else
                {
                    cs.State = ClientSubscriptionState.PartialDelivered;
                }

                cs.CurrentQuantity = csLeftQuantity;
                cs.Timestamp = time;
                cs.Notes = model.Notes;

                db.Entry(cs).State = EntityState.Modified;

                var clientSubscriptionHistory = new ClientSubscriptionHistory()
                {

                    UnitPrice = subscription.UnitPrice,
                    State = cs.State,
                    Quantity = model.Quantity,
                    Notes = model.Notes,
                    VesselName = vesselName,
                    ClientSubscriptionId = cs.ClientSubscriptionId,
                    Timestamp = time
                };

                db.ClientSubscriptionHistories.Add(clientSubscriptionHistory);

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = accountId });
            }

            return View("Invalid state.");
        }

        public ActionResult Close(long? id)
        {
            return GetSnapshot(id);
        }


        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed([Bind(Include = "SubscriptionId, Notes,Timestamp")] Subscription subscription)
        {
            return ModifyState(subscription, SubscriptionState.Closed);
        }


        public ActionResult Refund(long? id)
        {
            return GetSnapshot(id);
        }


        // POST: Subscriptions/Refund
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Refund")]
        [ValidateAntiForgeryToken]
        public ActionResult RefundConfirmed([Bind(Include = "SubscriptionId, Notes, Timestamp")] Subscription s)
        {
            if (ModelState.IsValid)
            {
                DateTime time = s.Timestamp;

                Subscription subscription = db.Subscriptions.Find(s.SubscriptionId);
                long accountId = subscription.StationAccountId;
                StationAccount station = db.StationAccounts.Find(accountId);

                subscription.State = SubscriptionState.Refunded;
                db.Entry(subscription).State = EntityState.Modified;
                subscription.Timestamp = time;
                station.Deposite += subscription.UnitPrice * subscription.CurrentQuantity;
                db.Entry(station).State = EntityState.Modified;

                var subscriptionHistory = new SubscriptionHistory()
                {
                    UnitPrice = subscription.UnitPrice,
                    State = SubscriptionState.Refunded,
                    Quantity = subscription.CurrentQuantity,
                    Notes = subscription.Notes,
                    SubscriptionId = subscription.SubscriptionId,
                    Timestamp = time
                };

                db.SubscriptionHistories.Add(subscriptionHistory);

                var depositeHistory = new DepositeHistory()
                {
                    Amount = subscription.UnitPrice * subscription.CurrentQuantity,
                    ChangeType = DepositeChangeType.Refund,
                    StationAccountId = accountId,
                    Description = string.Format("Refund for Subscription: {0}.", subscription.SubscriptionId),
                    Timestamp = time
                };

                db.DepositeHistories.Add(depositeHistory);

                subscription.CurrentQuantity = 0;
                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = accountId });
            }

            return View("The state is not valid.");
        }

        // GET: Subscriptions/Delete/5
        public ActionResult Delete(long? id)
        {
            return GetSnapshot(id);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Subscription subscription = db.Subscriptions.Find(id);
            long accountId = subscription.StationAccountId;
            db.Subscriptions.Remove(subscription);
            db.SaveChanges();
            return RedirectToAction("Index", new { accountId = accountId });
        }

        private ActionResult GetSnapshot(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        private ActionResult ModifyState(Subscription s, SubscriptionState state)
        {
            if (ModelState.IsValid)
            {
                DateTime time = s.Timestamp;
                Subscription subscription = db.Subscriptions.Find(s.SubscriptionId);
                long stationAccountId = subscription.StationAccountId;
                subscription.State = state;
                subscription.Timestamp = s.Timestamp;
                subscription.Notes = s.Notes;

                db.Entry(subscription).State = EntityState.Modified;

                var subscriptionHistory = new SubscriptionHistory()
                {

                    UnitPrice = subscription.UnitPrice,
                    State = state,
                    Quantity = subscription.CurrentQuantity,
                    Notes = subscription.Notes,
                    SubscriptionId = subscription.SubscriptionId,
                    Timestamp = time
                };

                db.SubscriptionHistories.Add(subscriptionHistory);

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = stationAccountId });
            }

            return View("Invalid state.");
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
