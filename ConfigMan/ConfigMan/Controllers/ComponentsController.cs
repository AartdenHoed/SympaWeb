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
using System.Runtime.Remoting.Lifetime;
using System.Text.RegularExpressions;
using ConfigMan.Models;
using Newtonsoft.Json.Linq;


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
        public ActionResult Index(string messageP, string msgLevelP, 
            string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            ComponentIndex index = new ComponentIndex();

            index.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

            string t = "Component - Overzicht";
            string m = messageP;
            string l = msgLevelP;
            if (m is null) { 
                if (!index.FilterData.Filter) {
                    m = "Klik op NIEUWE COMPONENT om een component aan te maken, of klik op een actie voor een bestaande component"; 
                    l = index.Message.Info;
                }
                else {
                    m = "Klik op NIEUWE COMPONENT om een component aan te maken, of klik op een actie voor een bestaande component (LIJST IS GEFILTERD)";
                    l = index.Message.Info;
                }
            }  
            index.Message.Fill(t,l,m);

            switch (index.FilterData.Subsetstr) 
            {
                case "A":
                    var queryA = from component in db.Components
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
                    index.ComponentLijst = queryA.ToList();
                    break;
                case "Y":
                    var queryY = from c in db.Components
                                 where !(from i in db.Installations
                                         where i.EndDateTime != null
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

                    index.ComponentLijst = queryY.ToList();
                    break;
                case "N":
                    var queryN = from c in db.Components
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

                    index.ComponentLijst = queryN.ToList();
                    break;
                case "E":
                    var queryE = from c in db.Components
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

                    index.ComponentLijst = queryE.ToList();
                    break;
                default:
                    m = "Subset waarde '" + index.FilterData.Subsetstr + "' is ongeldig";
                    l = index.Message.Error;
                    index.Message.Fill(t, l, m);
                    return View(index);
            }
              
            if (!index.FilterData.Filter)
            {
                return View(index);
            }
            else
            {
                if (!string.IsNullOrEmpty(index.FilterData.ComponentFilter))
                {
                    var query2 = from cm in index.ComponentLijst
                                 where cm.ComponentNameTemplate.ToUpper().Contains(index.FilterData.ComponentFilter.ToUpper())
                                 select cm;
                    index.ComponentLijst = query2.ToList();
                }
                if ((!string.IsNullOrEmpty(index.FilterData.VendorFilter)) && (index.ComponentLijst.Count > 0))
                {
                    var query3 = from cm in index.ComponentLijst
                                 where cm.VendorName.ToUpper().Contains(index.FilterData.VendorFilter.ToUpper())
                                 select cm;
                    index.ComponentLijst = query3.ToList();
                }
                if ((!string.IsNullOrEmpty(index.FilterData.AuthFilter)) && (index.ComponentLijst.Count > 0))
                {
                    var query4 = from cm in index.ComponentLijst
                                 where cm.Authorized.ToUpper().Contains(index.FilterData.AuthFilter.ToUpper())
                                 select cm;
                    index.ComponentLijst = query4.ToList();
                }
                

            }
            if (index.ComponentLijst.Count == 0)
            {
                m = "Geen enkele component voldoet aan de ingegeven filterwaarden ";
                l = index.Message.Warning;
                index.Message.Fill(t,l,m);
            }
            return View(index);
        }     
        //
        // GET: Components/Details/5
        //
        public ActionResult Details(int? id, string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            ComponentVM componentVM = new ComponentVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Component ID op");
            string t = "Component - Bekijken";
            string l = "?";
            string m = "?";
            
            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Component Bekijken (GET)";
                l = componentVM.Message.Error;
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
                                
                return RedirectToAction("Index", "Components", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = componentVM.FilterData.Filterstr,
                    subsetstrP = componentVM.FilterData.Subsetstr,
                    componentFilterP = componentVM.FilterData.ComponentFilter,
                    authFilterP = componentVM.FilterData.AuthFilter,
                    vendorFilterP = componentVM.FilterData.VendorFilter
                });
            }
            else 
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
                    l = componentVM.Message.Error;
                    m = "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.";                    
                }
                else
                {
                    l = componentVM.Message.Info;
                    m = "Klik op BEWERK om deze component te bewerken";
                }
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
                return View(componentVM);
            }           
        }
        
        //
        // GET: Components/Create
        //
        public ActionResult Create(string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
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
            componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

            
            return View(componentVM);
            
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComponentVM componentVM, string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            List<Vendor> vendordblist = new List<Vendor>();
            componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
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
                        messageP = m,
                        msgLevelP = l,
                        filterstrP = componentVM.FilterData.Filterstr,
                        subsetstrP = componentVM.FilterData.Subsetstr,
                        componentFilterP = componentVM.FilterData.ComponentFilter,
                        authFilterP = componentVM.FilterData.AuthFilter,
                        vendorFilterP = componentVM.FilterData.VendorFilter
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
        public ActionResult Edit(int id, string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            ComponentVM componentVM = new ComponentVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Component ID op");
            string t = "Component - Bewerken";
            string l = "?";
            string m = "?";

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Component Bewerken (GET)";
                l = componentVM.Message.Error;
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

                return RedirectToAction("Index", "Components", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = componentVM.FilterData.Filterstr,
                    subsetstrP = componentVM.FilterData.Subsetstr,
                    componentFilterP = componentVM.FilterData.ComponentFilter,
                    authFilterP = componentVM.FilterData.AuthFilter,
                    vendorFilterP = componentVM.FilterData.VendorFilter
                });
            }
            else 
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
                    l = componentVM.Message.Error;
                    m = "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.";
                }
                else
                {
                    l = componentVM.Message.Info;
                    m = "Voer wijzigingen in en klik op OPSLAAN";
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
                }
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
   
                return View(componentVM);
            }
 
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComponentVM componentVM, string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            string t = "Component - Bewerken";
            string l = "?";
            string m = "?";
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
        
                m = "Component " + component.ComponentNameTemplate.TrimEnd() + " is aangepast";
                l = componentVM.Message.Info;
                componentVM.Message.Fill(t,l,m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
                return RedirectToAction("Index", "Components", new
                {
                    messageP = componentVM.Message.Tekst,
                    msgLevelP = componentVM.Message.Level,
                    filterstrP = componentVM.FilterData.Filterstr,
                    subsetstrP = componentVM.FilterData.Subsetstr,
                    componentFilterP = componentVM.FilterData.ComponentFilter,
                    authFilterP = componentVM.FilterData.AuthFilter,
                    vendorFilterP = componentVM.FilterData.VendorFilter
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

                l = componentVM.Message.Error;
                m = "Model ERROR in " + componentVM.ComponentNameTemplate;
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

                return View(componentVM);
            }

        }

        //
        // GET: Components/Delete/5
        //
        public ActionResult Delete(int id, string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            ComponentVM componentVM = new ComponentVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Component ID op");
            string t = "Component - Bekijken";
            string l = "?";
            string m = "?";

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Component Verwijderen (GET)";
                l = componentVM.Message.Error;
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

                return RedirectToAction("Index", "Components", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = componentVM.FilterData.Filterstr,
                    subsetstrP = componentVM.FilterData.Subsetstr,
                    componentFilterP = componentVM.FilterData.ComponentFilter,
                    authFilterP = componentVM.FilterData.AuthFilter,
                    vendorFilterP = componentVM.FilterData.VendorFilter
                });
            }

            else
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
                    l = componentVM.Message.Error;
                    m = "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.";
                }
                else
                {
                    l = componentVM.Message.Info;
                    m = "Klik op VERWIJDEREN om deze component te verwijderen";
                }
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
                return View(componentVM);
            }
            
        }


        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)

        {
            ComponentVM componentVM = new ComponentVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Component ID op");
            string t = "Component - Verwijderen";
            string l = "?";
            string m = "?";
            componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;
                m = "Contract error bij Component Verwijderen (POST)";
                l = componentVM.Message.Error;
                componentVM.Message.Fill(t, l, m);
            }
            else
            {
                Component component = db.Components.Find(id);
                db.Components.Remove(component);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException exc)
                {
                    l = componentVM.Message.Warning;
                    m = "Component " + component.ComponentNameTemplate + " kan niet worden verwijderd. Verwijder eerst alle Installaties, Services en Documentatie *** (" + exc.Message + ")";
                    componentVM.Message.Fill(t,l,m);
                    componentVM.Fill(component);
                    componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

                    return View(componentVM);
                }
                m = "Component " + component.ComponentNameTemplate.TrimEnd() + " is verwijderd.";
                l = componentVM.Message.Info;
                componentVM.Message.Fill(t,l,m);
            }

            return RedirectToAction("Index", "Components", new
            {
                messageP = m,
                msgLevelP = l,
                filterstrP = componentVM.FilterData.Filterstr,
                subsetstrP = componentVM.FilterData.Subsetstr,
                componentFilterP = componentVM.FilterData.ComponentFilter,
                authFilterP = componentVM.FilterData.AuthFilter,
                vendorFilterP = componentVM.FilterData.VendorFilter
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
        public ActionResult Match(int id, string messageP, string msglevelP, 
            string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP)
        {
            ComponentVM componentVM = new ComponentVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Component ID op");
            string t = "Component - Matchen";
            string m = messageP;
            string l = msglevelP;
            componentVM.Message.Fill(t, l, m);
            componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;
                m = "Contract error bij Component Matchen (GET)";
                l = componentVM.Message.Error;
                componentVM.Message.Fill(t, l, m);
            }            
            else
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
                                VendorGroup = j1.VendorGroup
                            };
                componentVM = query.Single();

                if (componentVM == null)
                {
                    l = componentVM.Message.Error;
                    m = "*** ERROR *** ComponentID " + id.ToString() + " staat niet in de database.";
                }
                else
                {
                    l = componentVM.Message.Info;
                    m = "In onderstaande lijst kunnen overeenkomende installaties aan deze component worden gekoppeld";

                    // Get matching installations. 
                    // First installation belogiong to same Vendor Group
                    var query2 = from installation in db.Installations
                                 join component in db.Components
                                 on installation.ComponentID equals component.ComponentID
                                 join vendor in db.Vendors
                                 on component.VendorID equals vendor.VendorID
                                 where (vendor.VendorGroup == componentVM.VendorGroup)
                                 join computer in db.Computers
                                 on installation.ComputerID equals computer.ComputerID
                                 select new InstallationVM
                                 {
                                     ComputerID = installation.ComputerID,
                                     ComponentID = installation.ComponentID,
                                     ComponentName = installation.ComponentName,
                                     Release = installation.Release.TrimEnd(),
                                     Location = installation.Location.TrimEnd(),
                                     InstallDate = installation.InstallDate,
                                     MeasuredDateTime = installation.MeasuredDateTime,
                                     StartDateTime = installation.StartDateTime,
                                     EndDateTime = installation.EndDateTime,
                                     Count = installation.Count,
                                     ComponentNameTemplate = component.ComponentNameTemplate,
                                     ComputerName = computer.ComputerName,
                                     VendorID = vendor.VendorID,
                                     VendorName = vendor.VendorName,
                                     VendorGroup = vendor.VendorGroup
                                 };
                    List<InstallationVM> templist = new List<InstallationVM>();
                    templist = query2.ToList();
                    if (templist.Count == 0)
                    {
                        l = componentVM.Message.Warning;
                        m = "Geen installaties gevonden van leverancier " + componentVM.VendorGroup;
                    }
                    else
                    {
                        // Check if Installation Name matches ComponentNameTemplate
                        Regex rg = new Regex(componentVM.ComponentNameTemplate.Trim());
                        foreach (InstallationVM ivm in templist)
                        {
                            if ((rg.IsMatch(ivm.ComponentName.Trim())) || (ivm.ComponentName.Trim() == componentVM.ComponentNameTemplate.Trim()))
                            {
                                componentVM.InstallationLijst.Add(ivm);
                            }
                        }
                        if (componentVM.InstallationLijst.Count == 0)
                        {
                            l = componentVM.Message.Warning;
                            m = "Geen overeenkomstige installaties gevonden van leverancier " + componentVM.VendorGroup;

                        }
                    }

                }
                componentVM.Message.Fill(t, l, m);
                componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);
               
            }
            return View(componentVM);


        }

        public ActionResult Koppel(string filterstrP, string subsetstrP, string componentFilterP, string authFilterP, string vendorFilterP,
                                    int computerid, int componentid, string release, DateTime startdatetime, int newcomponentid)
        {
            ComponentVM componentVM = new ComponentVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(computerid > 0, "Contract Failed (computerid)!");
            Contract.Requires(componentid > 0, "Contract Failed (componentid)!");
            Contract.Requires(release != null, "Contract Failed (release)!");
            Contract.Requires(componentid != newcomponentid,"Component is al gekoppeld");
            Contract.Requires(startdatetime != null, "Contract Failed (startdatetime)!");
            Contract.Requires(newcomponentid > 0, "Contract Failed (componentid)!");

            componentVM.FilterData.Fill(filterstrP, subsetstrP, componentFilterP, authFilterP, vendorFilterP);

            string t = "Component - Koppelen Installatie";
            string m = "?";
            string l = "?";
            
            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Installatie Koppelen (GET)";
                l = componentVM.Message.Error;
                componentVM.Message.Fill(t, l, m);

                return RedirectToAction("Index", "Components", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = componentVM.FilterData.Filterstr,
                    subsetstrP = componentVM.FilterData.Subsetstr,
                    componentFilterP = componentVM.FilterData.ComponentFilter,
                    authFilterP = componentVM.FilterData.AuthFilter,
                    vendorFilterP = componentVM.FilterData.VendorFilter
                });
            }
            else
            {
                var query = from installation in db.Installations
                            where ((installation.ComputerID == computerid)
                                    && (installation.ComponentID == componentid)
                                    && (installation.Release == release)
                                    && (installation.StartDateTime == startdatetime))
                            select installation;
                           
                Installation OldInst = query.Single();
                bool addfailed = false;
                if (OldInst != null)
                {
                    Installation NewInst = new Installation()
                    {
                        ComponentID = newcomponentid,
                        ComputerID = OldInst.ComputerID,
                        ComponentName = OldInst.ComponentName,
                        Release = OldInst.Release,
                        Location = OldInst.Location,
                        InstallDate = OldInst.InstallDate,
                        MeasuredDateTime = OldInst.MeasuredDateTime,
                        StartDateTime = OldInst.StartDateTime,
                        EndDateTime = OldInst.EndDateTime,
                        Count = OldInst.Count
                    };
                       
                    db.Installations.Add(NewInst);
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
                        l = componentVM.Message.Error;
                        m = "Installatie " + OldInst.ComponentName.TrimEnd() + " is al gekoppeld.";
                        componentVM.Message.Fill(t, l, m);
                    }
                    else 
                    {                       
                        db.Installations.Remove(OldInst);
                        db.SaveChanges();
                        l = componentVM.Message.Info;
                        m = "Installatie " + OldInst.ComponentName + "is gekoppeld";
                        componentVM.Message.Fill(t, l, m);
                    }                   

                }
                else {
                    l = componentVM.Message.Error;
                    m = "*** ERROR *** Deze installatie staat niet in de database.";
                    componentVM.Message.Fill(t, l, m);                    
                }            
                        
                return RedirectToAction("Match", "Components", new
                {
                    id = newcomponentid,
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = componentVM.FilterData.Filterstr,
                    subsetstrP = componentVM.FilterData.Subsetstr,
                    componentFilterP = componentVM.FilterData.ComponentFilter,
                    authFilterP = componentVM.FilterData.AuthFilter,
                    vendorFilterP = componentVM.FilterData.VendorFilter
                });
            }
           
           

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