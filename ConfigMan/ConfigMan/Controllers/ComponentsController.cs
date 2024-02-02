using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ConfigMan.ViewModels;
using System.Diagnostics.Contracts;
using ConfigMan.ActionFilters;
using System.Diagnostics;

namespace ConfigMan.Controllers
{

    [LogActionFilter]
    [HandleError]

    public class ComponentsController : Controller
    {
        private readonly DbEntities db = new DbEntities();
        private static bool ContractErrorOccurred = false;
        private static string ErrorMessage = " ";

        public ActionResult Menu()
        {
            ViewBag.SympaMsg = TempData["SympaMsg"];
            return View();
        }

        //
        // GET: Components
        //
        public ActionResult Index()
        {
            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;
                ViewBag.SympaMsg = ErrorMessage;
                ErrorMessage = " ";
            }
            else
            {
                ViewBag.SympaMsg = TempData["SympaMsg"];
            }
            List<Component> complist = db.Components.OrderBy(x => x.ComponentName).ToList();
            List<ComponentVM> vmlist = new List<ComponentVM>();
            foreach (Component c in complist)
            {
                ComponentVM VM = new ComponentVM();
                VM.Fill(c);
                vmlist.Add(VM);
            }
            return View(vmlist);

        }

        //
        // GET: Components/Details/5
        //
        public ActionResult Details(int? id)
        {

            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Contract Failed!");

            if (!ContractErrorOccurred)
            {
                Component component = db.Components.Find(id);
                if (component == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else
                {
                    ComponentVM componentVM = new ComponentVM();
                    componentVM.Fill(component);
                    return View(componentVM);
                }
            }
            return RedirectToAction("Index");
        }

        private void Contract_ContractFailed(object sender, ContractFailedEventArgs e)
        {
            ErrorMessage = "*** ERROR *** Selected component id is invalid.";
            ContractErrorOccurred = true;
            e.SetHandled();
            return;
        }

        //
        // GET: Components/Create
        //
        public ActionResult Create()
        {
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComponentID,VendorID,ComponentName")] ComponentVM componentVM)
        {
            if (ModelState.IsValid)
            {
                Component component = new Component();
                component.Fill(componentVM);
                bool addfailed = false;
                db.Components.Add(component);
                try
                {
                    db.SaveChanges();
                    TempData["SympaMsg"] = "Component " + component.ComponentName + " succesfully added.";
                }
                catch (DbUpdateException)
                {
                    addfailed = true;
                    ViewBag.SympaMsg = "Component " + component.ComponentName + " already exists, use update function.";
                }
                if (addfailed) { return View(); }
                else { return RedirectToAction("Index"); }
            }

            return View(componentVM);
        }

        //
        // GET: Components/Edit/5
        //
        public ActionResult Edit(int? id)
        {
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred)
            {
                Component component = db.Components.Find(id);
                if (component == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else
                {
                    ComponentVM componentVM = new ComponentVM();
                    componentVM.Fill(component);
                    return View(componentVM);
                }
            }
            return RedirectToAction("Index");

        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComponentID,VendorID,ComponentName")] ComponentVM componentVM)
        {
            if (ModelState.IsValid)
            {
                Component component = new Component();
                component.Fill(componentVM);
                db.Entry(component).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SympaMsg"] = "Component " + component.ComponentName + " succesfully updated.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["SympaMsg"] = "Update failed for component " + componentVM.ComponentName;
                return View(componentVM);
            }

        }

        //
        // GET: Components/Delete/5
        //
        public ActionResult Delete(int? id)
        {
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred)
            {
                Component component = db.Components.Find(id);
                if (component == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else
                {
                    ComponentVM componentVM = new ComponentVM();
                    componentVM.Fill(component);
                    return View(componentVM);
                }
            }
            return RedirectToAction("Index");

        }


        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contract.Requires(id > 0);
            Contract.ContractFailed += (Contract_ContractFailed);
            if (!ContractErrorOccurred)
            {
                Component component = db.Components.Find(id);
                db.Components.Remove(component);
                db.SaveChanges();
                TempData["SympaMsg"] = "Component " + component.ComponentName + " succesfully deleted.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Report01()
        {
            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;
                ViewBag.SympaMsg = ErrorMessage;
                ErrorMessage = " ";
            }
            else
            {
                ViewBag.SympaMsg = TempData["SympaMsg"];
            }
            var query = from c in db.Components
                        where !(from i in db.Installations
                                select i.ComponentID)
                               .Contains(c.ComponentID)
                        orderby c.ComponentName
                        select new ComponentVM
                        {
                            ComponentID = c.ComponentID,
                            ComponentName = c.ComponentName,
                            VendorID = c.VendorID
                            
                        };

            List<ComponentVM> complist = query.ToList();

            return View(complist);

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