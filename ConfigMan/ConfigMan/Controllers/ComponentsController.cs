﻿using System;
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
using Microsoft.Ajax.Utilities;

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
            
            return View();
        }

        //
        // GET: Components
        //
        public ActionResult Index(string message, String msgLevel)
        {
            ComponentIndex index = new ComponentIndex();
            index.Message.Title = "Component - Overzicht";
            ViewData["Title"] = "Component - Overzicht";
            if (message is null)
            {
                index.Message.Tekst = "Klik op NIEUWE COMPONENT om een component aan te maken, of klik op een actie voor een bestaande component"; 
                index.Message.Level = msgLevel;
            }
            else
            {
                index.Message.Tekst = message;
                index.Message.Level = msgLevel;
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
            index.ComponentLijst = query.ToList();

            return View(index);

        }

        //
        // GET: Components/Details/5
        //
        public ActionResult Details(int? id)
        {
            ComponentVM componentVM = new ComponentVM();
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
                componentVM = query.Single();

                if (componentVM == null)
                {
                    componentVM.Message.Fill("Component - Bekijken", componentVM.Message.Error, "*** ERROR *** ComponentID " + id.ToString() + " not found in database.");

                }
                else
                {                    
                    componentVM.Message.Fill("Component - Bekijken", componentVM.Message.Info, "Klik op BEWERK om deze component te bewerken");
                    
                }
                ViewData["Title"] = "Component - Bekijken";
                return View(componentVM);
            }
            string m = "Contract error bij Component Bekijken (GET)";
            string l = componentVM.Message.Error;
            return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
           
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
            componentVM.Message.Fill("Component - Aanmaken", componentVM.Message.Info, "Klik op AANMAKEN om deze component op te slaan");
            ViewData["Title"] = "Component - Aanmaken";
            return View(componentVM);
            
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComponentID,VendorID,ComponentName,SelectedVendorIDstring")] ComponentVM componentVM)
        {
            List<Vendor> vendordblist = new List<Vendor>();
            if (ModelState.IsValid)
            {
                Component component = new Component();
                component.Fill(componentVM);
                bool addfailed = false;
                db.Components.Add(component);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    addfailed = true;

                }
                if (addfailed)
                {
                    componentVM.Message.Fill("Component - Aanmaken",
                        componentVM.Message.Error, "Component " + component.ComponentName + " already exists, use update function.");
                }
                else
                {
                    string m = "Component " + component.ComponentName + " is toegevoegd";
                    string l = componentVM.Message.Info;
                    return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });

                }
            }
            else { 
                componentVM.Message.Fill("Component - Aanmaken",
                        componentVM.Message.Error, "Model ERROR for " + componentVM.ComponentName);
            }
            ViewData["Title"] = "Component - Aanmaken";
            vendordblist = db.Vendors.OrderBy(x => x.VendorName).ToList();
            foreach (Vendor v in vendordblist)
                {
                    VendorVM VM = new VendorVM();
                    VM.Fill(v);
                    componentVM.VendorLijst.Add(VM);
                }
            return View(componentVM);
        }

        //
        // GET: Components/Edit/5
        //
        public ActionResult Edit(int? id)
        {
            ComponentVM componentVM = new ComponentVM();
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
                componentVM = query.Single();

                if (componentVM == null)
                {
                    componentVM.Message.Fill("Component - Bewerken", componentVM.Message.Error, "*** ERROR *** ComponentID " + id.ToString() + " not found in database.");

                }
                else { 
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
                    componentVM.Message.Fill("Component - Bewerken", componentVM.Message.Info, "Voer wijzigingen in en klik op OPSLAAN");
                }
                return View(componentVM);
            }
            string m = "Contract error bij Component Bewerken (GET)";
            string l = componentVM.Message.Error;
            return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });

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
                if (selectedVendorID != componentVM.VendorID)
                {
                    component.VendorID = selectedVendorID;
                }  
                db.Entry(component).State = EntityState.Modified;
                db.SaveChanges();
        
                string m = "Component " + component.ComponentName + " is aangepast";
                string l = componentVM.Message.Info;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }
            else
            {
                componentVM.Message.Fill("Component - Bewerken",
                        componentVM.Message.Error, "Model ERROR for " + componentVM.ComponentName);
                
                return View(componentVM);
            }

        }

        //
        // GET: Components/Delete/5
        //
        public ActionResult Delete(int? id)
        {
            ComponentVM componentVM = new ComponentVM();
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
                componentVM = query.Single();
                if (componentVM == null)
                {
                    componentVM.Message.Fill("Component - Verwijderen", componentVM.Message.Error, "*** ERROR *** ComponentID " + id.ToString() + " not found in database.");
                    
                }
                else
                {
                    componentVM.Message.Fill("Component - Verwijderen", componentVM.Message.Info, "Klik op VERWIJDEREN om deze component te verwijderen");
                    
                }
                ViewData["Title"] = "Component - Verwijderen";
                return View(componentVM);
                }
            string m = "Contract error bij Component Verwijderen (GET)";
            string l = componentVM.Message.Error;
            return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
        }


        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SympaMessage msg = new SympaMessage();
            Contract.Requires(id > 0);
            Contract.ContractFailed += (Contract_ContractFailed);
            if (!ContractErrorOccurred)
            {
                Component component = db.Components.Find(id);
                db.Components.Remove(component);
                db.SaveChanges();
                string m = "Component " + component.ComponentName + " is verwijderd.";
                string l = msg.Info;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });                
            }
            else { 
                string m = "Contract error bij Component Verwijderen (GET)";
                string l = msg.Error;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }
        }

        public ActionResult Report01()
        {
            ComponentIndex index = new ComponentIndex();
            index.Message.Title = "Rapport - Componenten zonder installaties";
            ViewData["Title"] = "Rapport - Componenten zonder installaties";
            index.Message.Tekst = "Overzicht van componenten die nergens geïnstalleerd zijn";
            index.Message.Level = index.Message.Info;
            

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

            index.ComponentLijst = query.ToList();

            return View(index);

        }
        private void Contract_ContractFailed(object sender, ContractFailedEventArgs e)
        {
            ErrorMessage = "*** ERROR *** Contract Failed.";
            ContractErrorOccurred = true;
            e.SetHandled();
            return;
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