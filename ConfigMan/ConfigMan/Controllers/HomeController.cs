using ConfigMan.ActionFilters;
using ConfigMan.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConfigMan.Controllers {

    [LogActionFilter]
    [HandleError]

    public class HomeController : Controller {
        public ActionResult Index() {
            SympaMessage msg = new SympaMessage();
            msg.Fill("Home", msg.Info, "SYMPA");
            return View(msg);
        }

        public ActionResult About() {            
            SympaMessage msg = new SympaMessage();
            msg.Fill("Home - Over SYMPA", msg.Info, "Welkom bij SYMPA - gratis HOME computer configuratie management");
            return View(msg);            
        }

        public ActionResult Contact() {            
            SympaMessage msg = new SympaMessage();
            msg.Fill("Home - Contact", msg.Info, "Dit programma wordt u aangeboden door:");
            return View(msg);
        }
    }
}