using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevDefined.OAuth.Consumer;
using XeroApi.OAuth;
using XeroApi;
using System.Security.Cryptography.X509Certificates;
using InvoiceManager.DAL;

namespace InvoiceManager.Controllers
{
    public class HomeController : Controller
    {

        private InvoiceManagerContext db = new InvoiceManagerContext();

        public ActionResult Index()
        {
            return View();
        }


       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}