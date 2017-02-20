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
    public class ClientSubscriptionsController : Controller
    {
        private FuelTrackContext db = new FuelTrackContext();

        // GET: ClientSubscriptions
        public ActionResult Index(long? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClientAccountId = accountId;

            return View(db.ClientSubscriptions.OrderByDescending(cs => cs.Timestamp).ToList().Where(s => s.Client.ClientAccountId == accountId));
        }

        // GET: ClientSubscriptions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = db.ClientSubscriptions.Find(id);
            if (clientSubscription == null)
            {
                return HttpNotFound();
            }
            return View(clientSubscription);
        }

        // GET: ClientSubscriptions/Create
        public ActionResult Create(long accountId)
        {
            var account = db.ClientAccounts.Find(accountId);

            var stationOptions = new SelectList(
                db.StationAccounts.Select(
                    n => new SelectListItem()
                    {
                        Selected = false,
                        Text = n.StationName,
                        Value = n.StationAccountId.ToString()
                    }).ToList(),
                "Value",
                "Text",
                0);

            var model = new ClientSubscriptionViewModel()
            {
                ClientAccountId = account.ClientAccountId,
                ClientAccountName = account.ClientAccountName,
                StationList = stationOptions
            };

            return View(model);
        }

        // POST: ClientSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientAccountId,StationAccountId, VesselName, Quantity,UnitPrice,Notes,Timestamp")] ClientSubscriptionViewModel clientSubscriptionViewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime time = clientSubscriptionViewModel.Timestamp;

                var clientSubscription = new ClientSubscription()
                {
                    UnitPrice = clientSubscriptionViewModel.UnitPrice,
                    State = ClientSubscriptionState.Created,
                    StartQuantity = clientSubscriptionViewModel.Quantity,
                    CurrentQuantity = clientSubscriptionViewModel.Quantity,
                    ClientAccountId = clientSubscriptionViewModel.ClientAccountId,
                    Notes = clientSubscriptionViewModel.Notes,
                    VesselName = clientSubscriptionViewModel.VesselName,
                    StationAccountId = int.Parse(clientSubscriptionViewModel.StationAccountId),
                    Timestamp = time
                };

                db.ClientSubscriptions.Add(clientSubscription);



                db.SaveChanges();

                var clientSubscriptionHistory = new ClientSubscriptionHistory()
                {

                    UnitPrice = clientSubscriptionViewModel.UnitPrice,
                    State = ClientSubscriptionState.Created,
                    Quantity = clientSubscriptionViewModel.Quantity,
                    Notes = clientSubscriptionViewModel.Notes,
                    VesselName = clientSubscriptionViewModel.VesselName,
                    ClientSubscriptionId = clientSubscription.ClientSubscriptionId,
                    Timestamp = time
                };

                db.ClientSubscriptionHistories.Add(clientSubscriptionHistory);
                db.SaveChanges();

                return RedirectToAction("Index", new { accountId = clientSubscriptionViewModel.ClientAccountId });
            }

            return View(clientSubscriptionViewModel);
        }

        // GET: ClientSubscriptions/Pay/5
        public ActionResult Pay(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = db.ClientSubscriptions.Find(id);

            if (clientSubscription == null)
            {
                return HttpNotFound();
            }

            var client = db.ClientAccounts.Find(clientSubscription.ClientAccountId);

            if (client == null)
            {
                return HttpNotFound();
            }

            if (client.Balance < clientSubscription.UnitPrice * clientSubscription.StartQuantity)
            {
                return View("The balance is not enough to pay the subscription.");
            }

            return View(clientSubscription);
        }

        // POST: ClientSubscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Pay")]
        [ValidateAntiForgeryToken]
        public ActionResult PayConfirmed([Bind(Include = "ClientSubscriptionId, Notes, Timestamp")] ClientSubscription subscription)
        {
            if (ModelState.IsValid)
            {
                DateTime time = subscription.Timestamp;

                ClientSubscription clientSubscription = db.ClientSubscriptions.Find(subscription.ClientSubscriptionId);
                long clientAccountId = clientSubscription.ClientAccountId;
                ClientAccount client = db.ClientAccounts.Find(clientAccountId);

                if (client.Balance >= clientSubscription.UnitPrice * clientSubscription.StartQuantity)
                {
                    clientSubscription.State = ClientSubscriptionState.Paid;
                    db.Entry(clientSubscription).State = EntityState.Modified;
                    client.Balance -= clientSubscription.UnitPrice * clientSubscription.StartQuantity;
                    clientSubscription.Timestamp = time;
                    db.Entry(client).State = EntityState.Modified;

                    var clientSubscriptionHistory = new ClientSubscriptionHistory()
                    {

                        UnitPrice = clientSubscription.UnitPrice,
                        State = ClientSubscriptionState.Paid,
                        Quantity = clientSubscription.StartQuantity,
                        VesselName = subscription.VesselName,
                        Notes = clientSubscription.Notes,
                        ClientSubscriptionId = clientSubscription.ClientSubscriptionId,
                        Timestamp = time
                    };

                    db.ClientSubscriptionHistories.Add(clientSubscriptionHistory);

                    var clientBalanceHistory = new ClientBalanceHistory()
                    {
                        Amount = clientSubscription.UnitPrice * clientSubscription.StartQuantity * -1,
                        BalanceType = BalanceChangeType.Pay,
                        ClientAccountId = clientAccountId,
                        Description = string.Format("Pay for Subscription: {0}.", subscription.ClientSubscriptionId),
                        Timestamp = time
                    };

                    db.ClientBalanceHistories.Add(clientBalanceHistory);

                    db.SaveChanges();
                    return RedirectToAction("Index", new { accountId = clientAccountId });
                }

                return View("The balance is not enough to pay the subscription.");
            }

            return View("The state is not valid.");
        }

        public ActionResult Deliver(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = db.ClientSubscriptions.Find(id);
            if (clientSubscription == null)
            {
                return HttpNotFound();
            }

            ClientSubscriptionDeliverViewModel model = new ClientSubscriptionDeliverViewModel()
            {
                ClientAccountId = clientSubscription.ClientAccountId,
                ClientSubscriptionId = clientSubscription.ClientSubscriptionId,
                ClientAccountName = clientSubscription.Client.ClientAccountName,
                VesselName = clientSubscription.VesselName,
                StationAccountName = clientSubscription.Station.StationName,
                Quantity = clientSubscription.CurrentQuantity
            };

            return View(model);
        }


        // POST: ClientSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Deliver")]
        [ValidateAntiForgeryToken]
        public ActionResult DeliverConfirmed([Bind(Include = "ClientSubscriptionId, Quantity, Notes")] ClientSubscriptionDeliverViewModel model)
        {
            if (ModelState.IsValid)
            {
                DateTime time = DateTime.Now;
                ClientSubscription subscription = db.ClientSubscriptions.Find(model.ClientSubscriptionId);
                long clientAccountId = subscription.ClientAccountId;

                float leftQuantity = subscription.CurrentQuantity - model.Quantity;

                if (leftQuantity == 0)
                {
                    subscription.State = ClientSubscriptionState.Delivered;
                }
                else
                {
                    subscription.State = ClientSubscriptionState.PartialDelivered;
                }

                subscription.CurrentQuantity = leftQuantity;
                subscription.Timestamp = time;
                subscription.Notes = model.Notes;

                db.Entry(subscription).State = EntityState.Modified;

                var clientSubscriptionHistory = new ClientSubscriptionHistory()
                {

                    UnitPrice = subscription.UnitPrice,
                    State = subscription.State,
                    Quantity = model.Quantity,
                    Notes = model.Notes,
                    VesselName = subscription.VesselName,
                    ClientSubscriptionId = subscription.ClientSubscriptionId,
                    Timestamp = time
                };

                db.ClientSubscriptionHistories.Add(clientSubscriptionHistory);

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = clientAccountId });
            }

            return View("Invalid state.");
        }

        public ActionResult Close(long? id)
        {
            return GetSnapshot(id);
        }


        // POST: ClientSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed([Bind(Include = "ClientSubscriptionId, Notes, Timestamp")] ClientSubscription clientSubscription)
        {
            return ModifyState(clientSubscription, ClientSubscriptionState.Closed);
        }


        public ActionResult Refund(long? id)
        {
            return GetSnapshot(id);
        }


        // POST: ClientSubscriptions/Deprecate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Refund")]
        [ValidateAntiForgeryToken]
        public ActionResult RefundConfirmed([Bind(Include = "ClientSubscriptionId, Notes,Timestamp")] ClientSubscription subscription)
        {
            if (ModelState.IsValid)
            {
                DateTime time = subscription.Timestamp;

                ClientSubscription clientSubscription = db.ClientSubscriptions.Find(subscription.ClientSubscriptionId);
                long clientAccountId = clientSubscription.ClientAccountId;
                ClientAccount client = db.ClientAccounts.Find(clientAccountId);

                clientSubscription.State = ClientSubscriptionState.Refunded;
                db.Entry(clientSubscription).State = EntityState.Modified;
                client.Balance += clientSubscription.UnitPrice * clientSubscription.CurrentQuantity;
                clientSubscription.Timestamp = time;
                db.Entry(client).State = EntityState.Modified;

                var clientSubscriptionHistory = new ClientSubscriptionHistory()
                {
                    UnitPrice = clientSubscription.UnitPrice,
                    State = ClientSubscriptionState.Refunded,
                    Quantity = clientSubscription.CurrentQuantity,
                    VesselName = subscription.VesselName,
                    Notes = clientSubscription.Notes,
                    ClientSubscriptionId = clientSubscription.ClientSubscriptionId,
                    Timestamp = time
                };

                db.ClientSubscriptionHistories.Add(clientSubscriptionHistory);

                var clientBalanceHistory = new ClientBalanceHistory()
                {
                    Amount = clientSubscription.UnitPrice * clientSubscription.CurrentQuantity,
                    BalanceType = BalanceChangeType.Refund,
                    ClientAccountId = clientAccountId,
                    Description = string.Format("Refund for Subscription: {0}.", subscription.ClientSubscriptionId),
                    Timestamp = time
                };

                db.ClientBalanceHistories.Add(clientBalanceHistory);

                clientSubscription.CurrentQuantity = 0;
                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = clientAccountId });
            }

            return View("The state is not valid.");
        }

        // GET: ClientSubscriptions/Delete/5
        public ActionResult Delete(long? id)
        {
            return GetSnapshot(id);
        }

        // POST: ClientSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ClientSubscription clientSubscription = db.ClientSubscriptions.Find(id);
            long clientAccountId = clientSubscription.ClientAccountId;
            db.ClientSubscriptions.Remove(clientSubscription);
            db.SaveChanges();
            return RedirectToAction("Index", new { accountId = clientAccountId });
        }

        private ActionResult GetSnapshot(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = db.ClientSubscriptions.Find(id);
            if (clientSubscription == null)
            {
                return HttpNotFound();
            }
            return View(clientSubscription);
        }

        private ActionResult ModifyState(ClientSubscription clientSubscription, ClientSubscriptionState state)
        {
            if (ModelState.IsValid)
            {
                DateTime time = clientSubscription.Timestamp;
                ClientSubscription subscription = db.ClientSubscriptions.Find(clientSubscription.ClientSubscriptionId);
                long clientAccountId = subscription.ClientAccountId;
                subscription.State = state;
                subscription.Timestamp = DateTime.Now;
                subscription.Notes = clientSubscription.Notes;

                db.Entry(subscription).State = EntityState.Modified;

                var clientSubscriptionHistory = new ClientSubscriptionHistory()
                {

                    UnitPrice = subscription.UnitPrice,
                    State = state,
                    Quantity = subscription.CurrentQuantity,
                    Notes = subscription.Notes,
                    VesselName = subscription.VesselName,
                    ClientSubscriptionId = subscription.ClientSubscriptionId,
                    Timestamp = time
                };

                db.ClientSubscriptionHistories.Add(clientSubscriptionHistory);

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = clientAccountId });
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
