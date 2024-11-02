using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using System.Web.Mvc;
using ConfigMan.ViewModels;
using System.Diagnostics.Contracts;
using ConfigMan.ActionFilters;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;
using System.Reflection;
using System.Collections;


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
        public ActionResult Index(string message, string msgLevel, string filterstr, string componentFilter, string authFilter, string vendorFilter)
        {
            ComponentIndex index = new ComponentIndex();

            if ((string.IsNullOrEmpty(filterstr)) || (filterstr == "N"))
            {
                index.Filterstr = "N";
            }
            else
            {
                index.Filterstr = "Y";
            }

            if (message is null)
                if (!index.Filter) {
                    index.Message.Tekst = "Klik op NIEUWE COMPONENT om een component aan te maken, of klik op een actie voor een bestaande component"; 
                    index.Message.Level = index.Message.Info;

                }
                else {
                    index.Message.Tekst = "Klik op NIEUWE COMPONENT om een component aan te maken, of klik op een actie voor een bestaande component (LIJST IS GEFILTERD)";
                    index.Message.Level = index.Message.Info;
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
                            Authorized = component.Authorized,
                            VendorID = j1.VendorID,
                            VendorName = j1.VendorName                            
                        };
            index.ComponentLijst = query.ToList();

            if (!index.Filter)
            {
                return View(index);
            }
            else
            {
                if (!string.IsNullOrEmpty(componentFilter))
                {
                    index.ComponentFilter = componentFilter;
                    var query2 = from cm in index.ComponentLijst
                                 where cm.ComponentNameTemplate.ToUpper().Contains(componentFilter.ToUpper())
                                 select cm;
                    index.ComponentLijst = query2.ToList();
                }
                if ((!string.IsNullOrEmpty(vendorFilter)) && (index.ComponentLijst.Count > 0))
                {
                    index.VendorFilter = vendorFilter;
                    var query3 = from cm in index.ComponentLijst
                                 where cm.VendorName.ToUpper().Contains(vendorFilter.ToUpper())
                                 select cm;
                    index.ComponentLijst = query3.ToList();
                }
                if ((!string.IsNullOrEmpty(authFilter)) && (index.ComponentLijst.Count > 0))
                {
                    index.AuthFilter = authFilter;
                    var query4 = from cm in index.ComponentLijst
                                 where cm.Authorized.ToUpper().Contains(authFilter.ToUpper())
                                 select cm;
                    index.ComponentLijst = query4.ToList();
                }
                

            }
            if (index.ComponentLijst.Count == 0)
            {
                index.Message.Tekst = "Geen enkele component voldoet aan de ingegeven filterwaarden ";
                index.Message.Level = index.Message.Warning;
            }
            return View(index);
        }      
            
 

        //
        // GET: Components/Details/5
        //
        public ActionResult Details(int? id, string filterstr, string componentFilter, string authFilter, string vendorFilter)
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
                                Authorized = component.Authorized,
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
                if ((string.IsNullOrEmpty(filterstr)) || (filterstr == "N"))
                {
                    componentVM.Filterstr = "N";
                }
                else
                {
                    componentVM.Filterstr = "Y";
                }
                if (componentVM.Filter)
                {
                    if (string.IsNullOrEmpty(componentFilter))
                    {
                        componentVM.ComponentFilter = null;
                    }
                    else
                    {
                        componentVM.ComponentFilter = componentFilter;
                    }
                    if (string.IsNullOrEmpty(vendorFilter))
                    {
                        componentVM.VendorFilter = null;
                    }
                    else
                    {
                        componentVM.VendorFilter = vendorFilter;
                    }
                    if (string.IsNullOrEmpty(authFilter))
                    {
                        componentVM.AuthFilter = null;
                    }
                    else
                    {
                        componentVM.AuthFilter = authFilter;
                    }
                }

                return View(componentVM);
            }
            else
            {
                ContractErrorOccurred = false;

                string m = "Contract error bij Component Bekijken (GET)";
                string l = componentVM.Message.Error;
               
                return RedirectToAction("Index", "Components", new {Message = m, MsgLevel = l });
            }
           
        }
        
        //
        // GET: Components/Create
        //
        public ActionResult Create(string filterstr, string componentFilter, string authFilter, string vendorFilter)
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

            if ((string.IsNullOrEmpty(filterstr)) || (filterstr == "N"))
            {
                componentVM.Filterstr = "N";
            }
            else
            {
                componentVM.Filterstr = "Y";
            }
            if (componentVM.Filter)
            {
                if (string.IsNullOrEmpty(componentFilter))
                {
                    componentVM.ComponentFilter = null;
                }
                else
                {
                    componentVM.ComponentFilter = componentFilter;
                }
                if (string.IsNullOrEmpty(vendorFilter))
                {
                    componentVM.VendorFilter = null;
                }
                else
                {
                    componentVM.VendorFilter = vendorFilter;
                }
                if (string.IsNullOrEmpty(authFilter))
                {
                    componentVM.AuthFilter = null;
                }
                else
                {
                    componentVM.AuthFilter = authFilter;
                }
            }

            return View(componentVM);
            
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComponentID,VendorID,VendorName,ComponentNameTemplate,Authorized,SelectedVendorIDstring," +
            "Filterstr,ComponentFilter,VendorFilter,AuthFilter")] ComponentVM componentVM)
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
                    return RedirectToAction("Index", "Components", new
                    {
                        Message = m,
                        MsgLevel = l,
                        filterstr = componentVM.Filterstr,
                        componentFilter = componentVM.ComponentFilter,
                        authFilter = componentVM.AuthFilter,
                        vendorFilter = componentVM.VendorFilter
                    });

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
        public ActionResult Edit(int? id, string filterstr, string componentFilter, string authFilter, string vendorFilter)
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
                                Authorized = component.Authorized,
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
                if ((string.IsNullOrEmpty(filterstr)) || (filterstr == "N"))
                {
                    componentVM.Filterstr = "N";
                }
                else
                {
                    componentVM.Filterstr = "Y";
                }
                if (componentVM.Filter)
                {
                    if (string.IsNullOrEmpty(componentFilter))
                    {
                        componentVM.ComponentFilter = null;
                    }
                    else
                    {
                        componentVM.ComponentFilter = componentFilter;
                    }
                    if (string.IsNullOrEmpty(vendorFilter))
                    {
                        componentVM.VendorFilter = null;
                    }
                    else
                    {
                        componentVM.VendorFilter = vendorFilter;
                    }
                    if (string.IsNullOrEmpty(authFilter))
                    {
                        componentVM.AuthFilter = null;
                    }
                    else
                    {
                        componentVM.AuthFilter = authFilter;
                    }
                }

                return View(componentVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Component Bewerken (GET)";
                string l = componentVM.Message.Error;
                return RedirectToAction("Index", "Components", new {Message = m, MsgLevel = l,
                        filterstr, componentFilter, authFilter, vendorFilter});
            }

        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComponentID,VendorID,VendorName,ComponentNameTemplate,Authorized,SelectedVendorIDstring," +
            "Filterstr,ComponentFilter,VendorFilter,AuthFilter")] ComponentVM componentVM)
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
                return RedirectToAction("Index", "Components", new
                {
                    Message = m,
                    MsgLevel = l,
                    filterstr = componentVM.Filterstr,
                    componentFilter = componentVM.ComponentFilter,
                    authFilter = componentVM.AuthFilter,
                    vendorFilter = componentVM.VendorFilter
                });
            }
            else
            {
                List<Vendor> vendordblist = db.Vendors.OrderBy(x => x.VendorName).ToList();

                // Set first entry on current value
                VendorVM firstentry = new VendorVM()
                {
                    VendorID = componentVM.VendorID,
                    VendorName = db.Vendors.Find(componentVM.VendorID).VendorName
                };
                componentVM.VendorLijst.Add(firstentry);

                // add entries form Vendor db
                foreach (Vendor v in vendordblist)
                {
                    VendorVM VM = new VendorVM();
                    VM.Fill(v);
                    componentVM.VendorLijst.Add(VM);
                }

                componentVM.Message.Fill("Component - Bewerken",
                        componentVM.Message.Error, "Model ERROR in " + componentVM.ComponentNameTemplate);
                
                return View(componentVM);
            }

        }

        //
        // GET: Components/Delete/5
        //
        public ActionResult Delete(int? id, string filterstr, string componentFilter, string authFilter, string vendorFilter)
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
                                Authorized = component.Authorized, 
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
                return RedirectToAction("Index", "Components", new {Message = m, MsgLevel = l });
            }
        }


        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string filterstr, string componentFilter, string authFilter, string vendorFilter)

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
                    if ((string.IsNullOrEmpty(filterstr)) || (filterstr == "N"))
                    {
                        componentVM.Filterstr = "N";
                    }
                    else
                    {
                        componentVM.Filterstr = "Y";
                    }
                    if (componentVM.Filter)
                    {
                        if (string.IsNullOrEmpty(componentFilter))
                        {
                            componentVM.ComponentFilter = null;
                        }
                        else
                        {
                            componentVM.ComponentFilter = componentFilter;
                        }
                        if (string.IsNullOrEmpty(vendorFilter))
                        {
                            componentVM.VendorFilter = null;
                        }
                        else
                        {
                            componentVM.VendorFilter = vendorFilter;
                        }
                        if (string.IsNullOrEmpty(authFilter))
                        {
                            componentVM.AuthFilter = null;
                        }
                        else
                        {
                            componentVM.AuthFilter = authFilter;
                        }
                    }
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
            
            return RedirectToAction("Index", "Components", new
            {
                Message = m,
                MsgLevel = l,
                filterstr,
                componentFilter,
                authFilter,
                vendorFilter
            });
        }

        public ActionResult Report01()
        {
            ComponentIndex index = new ComponentIndex();
            index.Message.Title = "Rapport - Componenten zonder installaties";
            
            index.Message.Tekst = "Overzicht van componenten zonder installaties";
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
                            Authorized = c.Authorized,
                            VendorID = c.VendorID,
                            VendorName = j1.VendorName
                            
                        };

            index.ComponentLijst = query.ToList();
            
            return View(index);

        }

        public ActionResult Report02()
        {
            ComponentIndex index = new ComponentIndex();
            index.Message.Title = "Rapport - Componenten zonder actieve installaties";

            index.Message.Tekst = "Overzicht van componenten zonder actieve installaties";
            index.Message.Level = index.Message.Info;


            var query = from c in db.Components
                        where !(from i in db.Installations
                                where i.EndDateTime == null
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
                            Authorized = c.Authorized,
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