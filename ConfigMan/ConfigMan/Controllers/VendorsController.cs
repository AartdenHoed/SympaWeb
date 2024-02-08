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
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using Microsoft.Ajax.Utilities;

namespace ConfigMan.Controllers { 

    [LogActionFilter]
    [HandleError]
   
    public class VendorsController : Controller
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
        // GET: Vendors
        //
        public ActionResult Index()
        {
            if (ContractErrorOccurred) {
                ContractErrorOccurred = false;
                ViewBag.SympaMsg = ErrorMessage;
                ErrorMessage = " ";
            }
            else {
                ViewBag.SympaMsg = TempData["SympaMsg"];
            }
            List<Vendor> complist = db.Vendors.OrderBy(x => x.VendorGroup).ToList();
            List<VendorVM> vmlist = new List<VendorVM>();
            foreach (Vendor c in complist) {
                VendorVM VM = new VendorVM();
                VM.Fill(c);
                vmlist.Add(VM); 
            }
            return View(vmlist);

        }

        //
        // GET: Vendors/Details/5
        //
        public ActionResult Details(int? id)
        {
           
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Contract Failed!");

            if (!ContractErrorOccurred) {
                Vendor vendor = db.Vendors.Find(id);
                if (vendor == null) {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else {
                    VendorVM vendorVM = new VendorVM();
                    vendorVM.Fill(vendor);
                    return View(vendorVM);
                }
            }
            return RedirectToAction("Index");
        }

        private void Contract_ContractFailed(object sender, ContractFailedEventArgs e) {
            ErrorMessage = "*** ERROR *** Selected vendor id is invalid.";
            ContractErrorOccurred = true;
            e.SetHandled();
            return;
        }

        //
        // GET: Vendors/Create
        //
        public ActionResult Create()
        {    
            return View();
        }

        // POST: Vendors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VendorID,VendorName,VendorGroup")] VendorVM vendorVM)
        {
            if (ModelState.IsValid)            
            {
                Vendor vendor = new Vendor();
                vendor.Fill(vendorVM);
                bool addfailed = false;
                db.Vendors.Add(vendor);
                try {
                    db.SaveChanges();
                    TempData["SympaMsg"] = "Vendor " + vendor.VendorName + " succesfully added."; 
                }
                catch (DbUpdateException) {
                    addfailed = true;
                    ViewBag.SympaMsg = "Vendor " + vendor.VendorName + " already exists, use update function.";
                }
                if (addfailed) { return View(); }
                else { return RedirectToAction("Index"); }
            }

            return View(vendorVM);
        }

        //
        // GET: Vendors/Edit/5
        //
        public ActionResult Edit(int? id)
        {
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred) {
                Vendor vendor = db.Vendors.Find(id);
                if (vendor == null) {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else {
                    VendorVM vendorVM = new VendorVM();
                    vendorVM.Fill(vendor);
                    return View(vendorVM);
                }
            }
            return RedirectToAction("Index");

        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VendorID,VendorName,VendorGroup")] VendorVM vendorVM)
        {
            if (ModelState.IsValid)
            {
                Vendor vendor = new Vendor();
                vendor.Fill(vendorVM);
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SympaMsg"] = "Vendor " + vendor.VendorName + " succesfully updated.";
                return RedirectToAction("Index");
            }
            else {
                TempData["SympaMsg"] = "Update failed for vendor " + vendorVM.VendorName ;
                return View(vendorVM);
            }
            
        }

        //
        // GET: Vendors/Delete/5
        //
        public ActionResult Delete(int? id)
        {
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred) {
                Vendor vendor = db.Vendors.Find(id);
                if (vendor == null) {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else {
                    VendorVM vendorVM = new VendorVM();
                    vendorVM.Fill(vendor);
                    return View(vendorVM);
                }
            }
            return RedirectToAction("Index");

        }
        

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contract.Requires(id > 0);
            Contract.ContractFailed += (Contract_ContractFailed);
            if (!ContractErrorOccurred) {
                Vendor vendor = db.Vendors.Find(id);
                db.Vendors.Remove(vendor);
                db.SaveChanges();
                TempData["SympaMsg"] = "Vendor "+ vendor.VendorName + " succesfully deleted.";
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
            var query = from ven in db.Vendors
                        select new VendorVM
                        {
                            VendorID = ven.VendorID,
                            VendorName = ven.VendorName,
                            VendorGroup = ven.VendorGroup
                        } into v1
                        where
                        !(from v in db.Vendors
                          join c in db.Components on v.VendorID equals c.VendorID into join1
                          from j1 in join1
                          join inst in db.Installations on j1.ComponentID equals inst.ComponentID
                          select v.VendorID).Contains(v1.VendorID)
                        select v1;
             
            List < VendorVM > venlist = query.ToList();

            
            return View(venlist);

        }

        public ActionResult Report02()
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
            var query = from v in db.Vendors
                        where !(from c in db.Components
                                select c.VendorID)
                               .Contains(v.VendorID)
                        orderby v.VendorGroup
                        select new VendorVM
                        {
                            VendorID = v.VendorID,
                            VendorName = v.VendorName,
                            VendorGroup = v.VendorGroup
                        };

            List<VendorVM> venlist = query.ToList();

            return View(venlist);

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
