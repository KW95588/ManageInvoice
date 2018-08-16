using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InvoiceManager.DAL;
using InvoiceManager.Models;
using DevDefined.OAuth.Consumer;
using XeroApi.OAuth;
using XeroApi;
using System.Security.Cryptography.X509Certificates;
using InvoiceManager.DAL;

namespace InvoiceManager.Controllers
{
    public class InvoiceController : Controller
    {
        private InvoiceManagerContext db = new InvoiceManagerContext();

        // GET: Invoice
        public ActionResult Index()
        {
            return View(db.Invoice.ToList());
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoice.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoice/Create
        public ActionResult Create()
        {
            return View();
        }


        /*
        API Limits
            https://developer.xero.com/documentation/auth-and-limits/xero-api-limits


             There are limits to the number of API calls that your application can make against a particular Xero organisation.

                Minute Limit: 60 calls in a rolling 60 second window
                Daily Limit: 5000 calls in a rolling 24 hour window

            If you exceed either rate limit you will receive a HTTP 503 (Service Unavailable) response

            Private application limits

            You can create a maximum of 2 private applications against a Xero organisation regardless of the user that created the application.

            System limits
            Invoicing

            Xero is designed for volumes of up to 1,000 Sales invoices (Accounts Receivables) and 1,000 Purchases bills (Accounts Payables) per month, dependent also on the frequency of invoicing during the month, variability of amounts and the frequency of sales tax reporting requirements.
            Bank Transactions – Spend & Receive Money

            Xero is designed for volumes of up to around 2,000 bank transactions per month, also dependent on the frequency of transactions during the month and variability of transaction values. 
         */

        // POST: Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Contact,Date,Type,DueDate,Status")] Invoice invoice)
        {

            if (invoice.Date < DateTime.Now.AddDays(-1))
            {
                ModelState.AddModelError("Date", "Date must be today or a future time");
            }
            if (invoice.DueDate < invoice.Date)
            {
                ModelState.AddModelError("DueDate", "DueDate must be greater than the Start Date");
            }


            //in real app we also need to check the existence of the contact 

            if (ModelState.IsValid)
            {

                var XeroSession = new XeroApi.OAuth.XeroApiPrivateSession("MyApiTestSoftware", "5INHEEY7VVKTPZPBMTF9BRCDN33KGM", new X509Certificate2("C:\\OpenSSL-Win32\\bin\\public_privatekey.pfx"));
                var repository = new XeroApi.Repository(XeroSession);

                var contacts = repository.Contacts.Where(c => c.Name == "Marine Systems");

                //new comment

                var uInvoice = new XeroApi.Model.Invoice();
                var uContact = new XeroApi.Model.Contact();

                DateTime today = DateTime.Now;
                DateTime duedate = today.AddDays(30);

                uContact.Name = "Marine Systems";

                uInvoice.Contact = uContact;
                //uInvoice.Date = DateTime.Now;
                uInvoice.Date = invoice.Date;
                uInvoice.Type = "ACCREC";
                //uInvoice.DueDate = duedate;
                uInvoice.DueDate = invoice.DueDate;
                uInvoice.Status = "AUTHORISED";
                uInvoice.FullyPaidOnDate = today;
                uInvoice.AmountPaid = 230;

              

                uInvoice.LineItems = new XeroApi.Model.LineItems();
                var uLineItem1 = new XeroApi.Model.LineItem();
                uLineItem1.Quantity = 1;
                uLineItem1.Description = "Product 1";
                uLineItem1.AccountCode = "200";
                uLineItem1.UnitAmount = 50;
                uInvoice.LineItems.Add(uLineItem1);

                var uLineItem2 = new XeroApi.Model.LineItem();
                uLineItem2.Quantity = 3;
                uLineItem2.Description = "Product 2";
                uLineItem2.AccountCode = "200";
                uLineItem2.UnitAmount = 50;
                uInvoice.LineItems.Add(uLineItem2);

                var sResults = repository.Create((XeroApi.Model.Invoice)uInvoice);

                var theAccount = repository.Accounts.FirstOrDefault();

                //the following section is to create a payment for the invoice
                var invoiceNumber = sResults.InvoiceNumber;
                var invoiceID = sResults.InvoiceID;
                var NewInvoice = repository.Invoices.Where(i => i.InvoiceID == invoiceID).FirstOrDefault();

                //var account = repository.Accounts.Where(a => a.Name == "MyAccount").FirstOrDefault();
                var account = new XeroApi.Model.Account();

                //account.ReportingCode = "ABC";

                var Payment = new XeroApi.Model.Payment();
                //Payment.Account = (XeroApi.Model.Account)account;
                Payment.Account = theAccount;
                Payment.Invoice = (XeroApi.Model.Invoice)NewInvoice;
                Payment.Invoice.InvoiceNumber = invoiceNumber.ToString();
                Payment.Invoice.InvoiceID = invoiceID;
                Payment.Status = "AUTHORISED";
                Payment.Date = DateTime.Now;
                Payment.Amount = 200;

                var paymentResults = repository.Create((XeroApi.Model.Payment)Payment);

                db.Invoice.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoice.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Contact,Date,Type,DueDate,Status")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoice);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoice.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoice.Find(id);
            db.Invoice.Remove(invoice);
            db.SaveChanges();
            return RedirectToAction("Index");
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
