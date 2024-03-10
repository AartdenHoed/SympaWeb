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
      

        public ActionResult Menu()
        {
            SympaMessage msg = new SympaMessage();
            msg.Fill("Vendor - Beheer Menu", msg.Info, "Kies een optie");
            return View(msg);
        }

        //
        // GET: Vendors
        //
        public ActionResult Index(string message, String msgLevel)
        {
            VendorIndex index = new VendorIndex();
            index.Message.Title = "Vendor - Overzicht";

            if (message is null)
            {
                index.Message.Tekst = "Klik op NIEUWE VENDOR om een vendor aan te maken, of klik op een actie voor een bestaande vendor";
                index.Message.Level = msgLevel;
            }
            else
            {
                index.Message.Tekst = message;
                index.Message.Level = msgLevel;
            }
            List<Vendor> vendlist = db.Vendors.OrderBy(x => x.VendorName).ToList();

            foreach (Vendor v in vendlist)
            {
                VendorVM VM = new VendorVM();
                VM.Fill(v);
                index.VendorLijst.Add(VM);
            }
            return View(index);

        }

        //
        // GET: Vendors/Details/5
        //
        public ActionResult Details(int? id)
        {

            VendorVM vendorVM = new VendorVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Contract Failed!");

            if (!ContractErrorOccurred)
            {
                Vendor vendor = db.Vendors.Find(id);
                if (vendor == null)
                {
                    vendorVM.Message.Fill("Vendor - Bekijken", vendorVM.Message.Error, "*** ERROR *** VendorID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    vendorVM.Message.Fill("Vendor - Bekijken", vendorVM.Message.Info, "Klik op BEWERK om deze vendor te bewerken");

                }
                
                vendorVM.Fill(vendor);
                return View(vendorVM);
            }
            else
            {
                ContractErrorOccurred = false;

                string m = "Contract error bij Vendor Bekijken (GET)";
                string l = vendorVM.Message.Error;
                return RedirectToAction("Index", "Vendors", new { Message = m, MsgLevel = l });

            }
        }

           

        //
        // GET: Vendors/Create
        //
        public ActionResult Create()
        {
            VendorVM vendorVM = new VendorVM();
            vendorVM.Message.Fill("Vendor - Aanmaken", vendorVM.Message.Info, "Klik op AANMAKEN om deze vendor op te slaan");

            return View(vendorVM);
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
                    vendorVM.Message.Fill("Vendor - Aanmaken",
                        vendorVM.Message.Error, "Vendor " + vendor.VendorName.TrimEnd() + " bestaat al, gebruik de BEWERK functie.");
                }
                else
                {
                    string m = "Vendor " + vendor.VendorName.TrimEnd() + " is toegevoegd";
                    string l = vendorVM.Message.Info;
                    return RedirectToAction("Index", "Vendors", new { Message = m, MsgLevel = l });

                }
            }
            else
            {
                vendorVM.Message.Fill("Vendor - Aanmaken",
                       vendorVM.Message.Error, "Model ERROR in " + vendorVM.VendorName);

            }

            return View(vendorVM);
        }

        //
        // GET: Vendors/Edit/5
        //
        public ActionResult Edit(int? id)
        {
            VendorVM vendorVM = new VendorVM();
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred)
            {
                Vendor vendor = db.Vendors.Find(id);
                if (vendor == null)
                {
                    vendorVM.Message.Fill("Vendor - Bewerken", vendorVM.Message.Error, "*** ERROR *** VendorID " + id.ToString() + " staat niet in de database.");
                }
                else
                {
                    vendorVM.Fill(vendor);
                    vendorVM.Message.Fill("Vendor - Bewerken", vendorVM.Message.Info, "Voer wijzigingen in en klik op OPSLAAN");

                }
                
                return View(vendorVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Vendor Bewerken (GET)";
                string l = vendorVM.Message.Error;
                return RedirectToAction("Index", "Vendors", new { Message = m, MsgLevel = l });
            }

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

                string m = "Vendor " + vendor.VendorName + " is aangepast";
                string l = vendorVM.Message.Info;
                return RedirectToAction("Index", "Vendors", new { Message = m, MsgLevel = l });
            }
            else
            {
                vendorVM.Message.Fill("Vendor - Bewerken",
                        vendorVM.Message.Error, "Model ERROR in " + vendorVM.VendorName);
                
                return View(vendorVM);
            }

        }

        //
        // GET: Vendors/Delete/5
        //
        public ActionResult Delete(int? id)
        {
            VendorVM vendorVM = new VendorVM();
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred)
            {
                Vendor vendor = db.Vendors.Find(id);
                if (vendor == null)
                {
                    vendorVM.Message.Fill("Vendor - Verwijderen", vendorVM.Message.Error, "*** ERROR *** VendorID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    vendorVM.Message.Fill("Vendor - Verwijderen", vendorVM.Message.Info, "Klik op VERWIJDEREN om deze vendor te verwijderen");
                }
                vendorVM.Fill(vendor);
                return View(vendorVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Vendor Verwijderen (GET)";
                string l = vendorVM.Message.Error;
                return RedirectToAction("Index", "Vendors", new { Message = m, MsgLevel = l });
            }

        }


        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SympaMessage msg = new SympaMessage();
            string m = "";
            string l = "";
            Contract.Requires(id > 0);
            Contract.ContractFailed += (Contract_ContractFailed);
            if (!ContractErrorOccurred)
            {
                Vendor vendor = db.Vendors.Find(id);
                db.Vendors.Remove(vendor);
                try {
                db.SaveChanges();
                }
                catch (DbUpdateException exc)
                {
                    VendorVM vendorVM = new VendorVM();
                    vendorVM.Message.Fill("Vendor - Verwijderen", vendorVM.Message.Warning, "Vendor " + vendor.VendorName + " kan niet worden verwijderd. Verwijder eerst alle Componenten *** (" + exc.Message + ")");
                    vendorVM.Fill(vendor);
                    return View(vendorVM);
                }
                m = "Vendor " + vendor.VendorName.TrimEnd() + " is verwijderd.";
                l = msg.Info;
                
            }
            else
            {
                ContractErrorOccurred = false;
                m = "Contract error bij Vendor Verwijderen (POST)";
                l = msg.Error;
                
            }
            return RedirectToAction("Index", "Vendors", new { Message = m, MsgLevel = l });

        }

        public ActionResult Report01()
        {
            VendorIndex index = new VendorIndex();
            index.Message.Title = "Rapport - Vendors zonder Installaties";

            index.Message.Tekst = "Overzicht van vendors waarvan geen componenten zijn geïnstalleerd";
            index.Message.Level = index.Message.Info;
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

            index.VendorLijst = query.ToList();

            return View(index);

        }

        public ActionResult Report02()
        {
            VendorIndex index = new VendorIndex();
            index.Message.Title = "Rapport - Vendors zonder Componenten";

            index.Message.Tekst = "Overzicht van vendors waaraan geen componenten zijn gekoppeld";
            index.Message.Level = index.Message.Info;
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

            index.VendorLijst = query.ToList();

            return View(index);

        }

        private void Contract_ContractFailed(object sender, ContractFailedEventArgs e)
        {

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
