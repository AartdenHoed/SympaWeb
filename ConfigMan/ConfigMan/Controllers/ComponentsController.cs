using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using System.Web.Mvc;
using ConfigMan.ViewModels;
using System.Diagnostics.Contracts;
using ConfigMan.ActionFilters;


namespace ConfigMan.Controllers
{

    [LogActionFilter]
    [HandleError]

    public class ComponentsController : Controller
    {
        private readonly DbEntities db = new DbEntities();
        private static bool ContractErrorOccurred = false;
        
        public ActionResult Menu()
        {
            SympaMessage msg = new SympaMessage();
            msg.Fill("Component - Beheer Menu", msg.Info, "Kies een optie");
           
            return View(msg);
        }

        //
        // GET: Components
        //
        public ActionResult Index(string message, String msgLevel)
        {
            ComponentIndex index = new ComponentIndex();
            index.Message.Title = "Component - Overzicht";
            
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
                        orderby j1.VendorGroup, j1.VendorName, component.ComponentNameTemplate
                        select new ComponentVM
                        {
                            ComponentID = component.ComponentID,
                            ComponentNameTemplate = component.ComponentNameTemplate,
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
                            orderby component.ComponentNameTemplate
                            select new ComponentVM
                            {
                                ComponentID = component.ComponentID,
                                ComponentNameTemplate = component.ComponentNameTemplate,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName,
                            };
                componentVM = query.Single();

                if (componentVM == null)
                {
                    componentVM.Message.Fill("Component - Bekijken", componentVM.Message.Error, "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    componentVM.Message.Fill("Component - Bekijken", componentVM.Message.Info, "Klik op BEWERK om deze component te bewerken");

                }
                
                return View(componentVM);
            }
            else
            {
                ContractErrorOccurred = false;

                string m = "Contract error bij Component Bekijken (GET)";
                string l = componentVM.Message.Error;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }
           
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
            
            return View(componentVM);
            
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComponentID,VendorID,ComponentNameTemplate,SelectedVendorIDstring")] ComponentVM componentVM)
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
                        componentVM.Message.Error, "Component " + component.ComponentNameTemplate + " bestaat al, gebruik de BEWERK functie.");
                }
                else
                {
                    string m = "Component " + component.ComponentNameTemplate.TrimEnd() + " is toegevoegd";
                    string l = componentVM.Message.Info;
                    return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });

                }
            }
            else { 
                componentVM.Message.Fill("Component - Aanmaken",
                        componentVM.Message.Error, "Model ERROR in " + componentVM.ComponentNameTemplate);
            }
            
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
                            orderby component.ComponentNameTemplate
                            select new ComponentVM
                            {
                                ComponentID = component.ComponentID,
                                ComponentNameTemplate = component.ComponentNameTemplate,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName,
                            };
                componentVM = query.Single();

                if (componentVM == null)
                {
                    componentVM.Message.Fill("Component - Bewerken", componentVM.Message.Error, "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    List<Vendor> vendordblist = db.Vendors.OrderBy(x => x.VendorName).ToList();

                    // Set first entry on current value
                    VendorVM firstentry = new VendorVM()
                    {
                        VendorID = componentVM.VendorID,
                        VendorName = componentVM.VendorName
                    };
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
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Component Bewerken (GET)";
                string l = componentVM.Message.Error;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }

        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComponentID,VendorID,ComponentNameTemplate,SelectedVendorIDstring")] ComponentVM componentVM)
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
        
                string m = "Component " + component.ComponentNameTemplate.TrimEnd() + " is aangepast";
                string l = componentVM.Message.Info;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }
            else
            {
                componentVM.Message.Fill("Component - Bewerken",
                        componentVM.Message.Error, "Model ERROR in " + componentVM.ComponentNameTemplate);
                
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
                            orderby component.ComponentNameTemplate
                            select new ComponentVM
                            {
                                ComponentID = component.ComponentID,
                                ComponentNameTemplate = component.ComponentNameTemplate,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName,
                            };
                componentVM = query.Single();
                if (componentVM == null)
                {
                    componentVM.Message.Fill("Component - Verwijderen", componentVM.Message.Error, "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    componentVM.Message.Fill("Component - Verwijderen", componentVM.Message.Info, "Klik op VERWIJDEREN om deze component te verwijderen");

                }
               
                return View(componentVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Component Verwijderen (GET)";
                string l = componentVM.Message.Error;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }
        }


        // POST: Components/Delete/5
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
                Component component = db.Components.Find(id);
                db.Components.Remove(component);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException exc)
                {
                    ComponentVM componentVM = new ComponentVM();
                    componentVM.Message.Fill("Component - Verwijderen", componentVM.Message.Warning, "Component " + component.ComponentNameTemplate + " kan niet worden verwijderd. Verwijder eerst alle Installaties, Services en Documentatie *** (" + exc.Message + ")");
                    componentVM.Fill(component);
                    return View(componentVM);
                }
                m = "Component " + component.ComponentNameTemplate.TrimEnd() + " is verwijderd.";
                l = msg.Info;
                      
            }
            else {
                ContractErrorOccurred = false;
                m = "Contract error bij Component Verwijderen (POST)";
                l = msg.Error;
                
            }
            return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
        }

        public ActionResult Report01()
        {
            ComponentIndex index = new ComponentIndex();
            index.Message.Title = "Rapport - Componenten zonder installaties";
            
            index.Message.Tekst = "Overzicht van componenten die nergens geïnstalleerd zijn";
            index.Message.Level = index.Message.Info;
            

            var query = from c in db.Components
                        where !(from i in db.Installations
                                select i.ComponentID)
                               .Contains(c.ComponentID)
                        join vendor in db.Vendors
                            on c.VendorID equals vendor.VendorID into join1
                        from j1 in join1
                        orderby j1.VendorGroup, j1.VendorName, c.ComponentNameTemplate
                        select new ComponentVM                       
                        {
                            ComponentID = c.ComponentID,
                            ComponentNameTemplate = c.ComponentNameTemplate,
                            VendorID = c.VendorID,
                            VendorName = j1.VendorName
                            
                        };

            index.ComponentLijst = query.ToList();
            
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