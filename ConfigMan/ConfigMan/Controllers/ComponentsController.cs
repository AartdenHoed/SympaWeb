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
using System.Reflection;
using System.Collections;
using System.ComponentModel;

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
            var query = from component in db.Components
                        join vendor in db.Vendors
                        on component.VendorID equals vendor.VendorID into join1
                        from j1 in join1
                        orderby component.ComponentName
                        select new ComponentVM
                        {
                            ComponentID = component.ComponentID,
                            ComponentName = component.ComponentName,
                            VendorID = j1.VendorID,
                            VendorName = j1.VendorName,
                        };
            List<ComponentVM> complist = query.ToList();

            return View(complist);

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
                var query = from component in db.Components
                            where component.ComponentID == id
                            join vendor in db.Vendors
                            on component.VendorID equals vendor.VendorID into join1
                            from j1 in join1
                            orderby component.ComponentName
                            select new ComponentVM
                            {
                                ComponentID = component.ComponentID,
                                ComponentName = component.ComponentName,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName,
                            };
                ComponentVM componentVM = query.Single();
                if (componentVM == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else
                {
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
            // Create vendor drop down list
            ComponentVM componentVM = new ComponentVM();
            List<Vendor> vendordblist = db.Vendors.OrderBy(x => x.VendorName).ToList();
            foreach (Vendor v in vendordblist)
            {
                VendorVM VM = new VendorVM();
                VM.Fill(v);
                componentVM.VendorLijst.Add(VM);
            }
            return View(componentVM);
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComponentID,VendorID,ComponentName,SelectedVendorIDstring")] ComponentVM componentVM)
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
                var query = from component in db.Components
                            where component.ComponentID == id
                            join vendor in db.Vendors
                            on component.VendorID equals vendor.VendorID into join1
                            from j1 in join1
                            orderby component.ComponentName
                            select new ComponentVM
                            {
                                ComponentID = component.ComponentID,
                                ComponentName = component.ComponentName,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName,
                            };
                ComponentVM componentVM = query.Single();
                List<Vendor> vendordblist = db.Vendors.OrderBy(x => x.VendorName).ToList();
                
                // Set first entry on current value
                VendorVM firstentry = new VendorVM();
                firstentry.VendorID = componentVM.VendorID;
                firstentry.VendorName = componentVM.VendorName;
                componentVM.VendorLijst.Add(firstentry);

                // add entries form Vendor db
                foreach (Vendor v in vendordblist)
                {
                    VendorVM VM = new VendorVM();
                    VM.Fill(v);
                    componentVM.VendorLijst.Add(VM);
                }
                if (componentVM == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else
                {
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
        public ActionResult Edit([Bind(Include = "ComponentID,VendorID,ComponentName,SelectedVendorIDstring")] ComponentVM componentVM)
        {
            if (ModelState.IsValid)
            {
                int selectedVendorID = Int32.Parse(componentVM.SelectedVendorIDstring);
                Component component = new Component();
                component.Fill(componentVM);
                if (selectedVendorID == componentVM.VendorID) {  
                    // Vendor did not change                    
                    db.Entry(component).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SympaMsg"] = "Component " + component.ComponentName + " succesfully updated.";
                }
                else
                {
                    // Vendor ID changed. delete old record en insert new one
                    Component c = db.Components.Find(component.ComponentID);
                    db.Components.Remove(c);                    
                    
                    component.VendorID = selectedVendorID;
                    db.Components.Add(component);
                    
                    try
                    {
                        db.SaveChanges();
                        TempData["SympaMsg"] = "Component " + component.ComponentName + " succesfully added.";
                    }
                    catch (DbUpdateException)
                    {
                        
                        ViewBag.SympaMsg = "Component " + component.ComponentName + " already exists, use update function.";
                    }

                }
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
                var query = from component in db.Components
                            where component.ComponentID == id
                            join vendor in db.Vendors
                            on component.VendorID equals vendor.VendorID into join1
                            from j1 in join1
                            orderby component.ComponentName
                            select new ComponentVM
                            {
                                ComponentID = component.ComponentID,
                                ComponentName = component.ComponentName,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName,
                            };
                ComponentVM componentVM = query.Single();
                if (componentVM == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else
                {
                    
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
                        join vendor in db.Vendors
                            on c.VendorID equals vendor.VendorID into join1
                        orderby c.ComponentName
                        from j1 in join1
                        select new ComponentVM
                        {
                            ComponentID = c.ComponentID,
                            ComponentName = c.ComponentName,
                            VendorID = c.VendorID,
                            VendorName = j1.VendorName
                            
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