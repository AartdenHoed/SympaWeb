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
using ConfigMan.Models;
using System.Runtime.Remoting.Lifetime;
using System.ComponentModel;
using Microsoft.Ajax.Utilities;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace ConfigMan.Controllers
{

    [LogActionFilter]
    [HandleError]

    public class InstallationsController : Controller
    {
        private readonly DbEntities db = new DbEntities();
        private static bool ContractErrorOccurred = false;
        
        public ActionResult Menu()
        {
            SympaMessage msg = new SympaMessage();
            msg.Fill("Installatie - Beheer Menu", msg.Info, "Kies een optie");
            return View(msg);
        }

        //
        // GET: Installations
        //
        public ActionResult Index(string message, string msgLevel)
        {
            InstallationIndex index = new InstallationIndex();
            index.Message.Title = "Installatie - Overzicht";

            if (message is null)
            {
                index.Message.Tekst = "Klik op NIEUWE INSTALLATIE om een installatie aan te maken, of klik op een actie voor een bestaande installatie";
                index.Message.Level = msgLevel;
            }
            else
            {
                index.Message.Tekst = message;
                index.Message.Level = msgLevel;
            }
            var query = from installation in db.Installations
                        join component in db.Components 
                        on installation.ComponentID equals component.ComponentID 
                        join computer in db.Computers
                        on installation.ComputerID equals computer.ComputerID
                        join vendor in db.Vendors
                        on component.VendorID equals vendor.VendorID
                        orderby computer.ComputerName,vendor.VendorGroup,vendor.VendorName,component.ComponentNameTemplate,installation.ComponentName,installation.StartDateTime,installation.EndDateTime
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
                            ComputerName = computer.ComputerName
                        };
            index.InstallationLijst = query.ToList();
            
            return View(index);

        }

        //
        // GET: Installations/Details/5
        //
        public ActionResult Details(int? computerid, int? componentid, string release, DateTime? startdatetime)
        {
            InstallationVM installationVM = new InstallationVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(computerid > 0, "Contract Failed (computerid)!");
            Contract.Requires(componentid > 0, "Contract Failed (componentid)!");
            Contract.Requires(release != null, "Contract Failed (release)!");
            Contract.Requires(startdatetime != null, "Contract Failed (startdatetime)!");

            if (!ContractErrorOccurred)
            {
                var query = from installation in db.Installations
                            where ((installation.ComputerID == computerid) 
                                    && (installation.ComponentID == componentid)
                                    && (installation.Release == release) 
                                    && (installation.StartDateTime == startdatetime))
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID 
                            join vendor in db.Vendors
                            on component.VendorID equals vendor.VendorID 
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
                                VendorName = vendor.VendorName
                            };
                installationVM = query.Single();
                if (installationVM == null)                
                {
                    installationVM.Message.Fill("Installation - Bekijken", installationVM.Message.Error, "*** ERROR *** Deze installatie staat niet in de database.");

                }
                else
                {
                    installationVM.Message.Fill("Installation - Bekijken", installationVM.Message.Info, "Klik op BEWERK om deze installatie te bewerken");

                }

                return View(installationVM);

            }
            else
            {
                ContractErrorOccurred = false;

                string m = "Contract error bij Installatie Bekijken (GET)";
                string l = installationVM.Message.Error;
                return RedirectToAction("Index", "VendorInstallations", new { Message = m, MsgLevel = l });
            }
            
        }

        //
        // GET: Installations/Create
        //
        public ActionResult Create()
        {
            InstallationVM installationVM = new InstallationVM();

            // fill selectlist with computers
            List<Computer> computerdblist = db.Computers.OrderBy(x => x.ComputerName).ToList();
            foreach (Computer c in computerdblist)
            {
                ComputerVM cVM = new ComputerVM();
                cVM.Fill(c);
                installationVM.ComputerLijst.Add(cVM);
            }

            // fill selectlist with components
            List<Component> componentdblist = db.Components.OrderBy(x => x.ComponentNameTemplate).ToList();
            foreach (Component o in componentdblist)
            {
                ComponentVM oVM = new ComponentVM();
                oVM.Fill(o);
                installationVM.ComponentLijst.Add(oVM);
            }
            installationVM.Message.Fill("Installatie - Aanmaken", installationVM.Message.Info, "Klik op AANMAKEN om deze installatie op te slaan");
            return View(installationVM);
        }

        // POST: Installations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComputerID,ComponentID,Release,Location,InstallDate,MeasuredDateTime,StartDateTime,EndDateTime,Count,SelectedComputerIDstring,SelectedComponentIDstring")] InstallationVM installationVM)
        {
            if (ModelState.IsValid)
            {
                Installation installation = new Installation();
                installation.Fill(installationVM);

                int selectedComputerID = Int32.Parse(installationVM.SelectedComputerIDstring);
                int selectedComponentID = Int32.Parse(installationVM.SelectedComponentIDstring);
                if (selectedComputerID != installationVM.ComputerID)
                {
                   installation.ComputerID = selectedComputerID;
                }
                if (selectedComponentID != installationVM.ComponentID)
                {
                    installation.ComponentID = selectedComponentID;
                }
                bool addfailed = false;
                db.Installations.Add(installation);
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
                   installationVM.Message.Fill("Installatie - Aanmaken",
                        installationVM.Message.Error, "Installatie " + installation.Release + " bestaat al, gebruik de BEWERK functie.");
                }
                else
                {
                    string m = "Installatie " + installation.Release.TrimEnd() + " is toegevoegd";
                    string l = installationVM.Message.Info;
                    return RedirectToAction("Index", "Installations", new { Message = m, MsgLevel = l });

                }
               
            }
            else
            {
                installationVM.Message.Fill("Installatie - Aanmaken",
                        installationVM.Message.Error, "Model ERROR in " + installationVM.Release.TrimEnd());
            }

            // fill selectlist with computers
            List<Computer> computerdblist = db.Computers.OrderBy(x => x.ComputerName).ToList();
            foreach (Computer c in computerdblist)
            {
                ComputerVM cVM = new ComputerVM();
                cVM.Fill(c);
                installationVM.ComputerLijst.Add(cVM);
            }

            // fill selectlist with components
            List<Component> componentdblist = db.Components.OrderBy(x => x.ComponentNameTemplate).ToList();
            foreach (Component o in componentdblist)
            {
                ComponentVM oVM = new ComponentVM();
                oVM.Fill(o);
                installationVM.ComponentLijst.Add(oVM);
            }

            return View(installationVM);

            
        }

        //
        // GET: Installations/Edit/5
        //
        public ActionResult Edit(int? computerid, int? componentid, string release, DateTime? startdatetime)
        {
            InstallationVM installationVM = new InstallationVM();
            Contract.Requires((componentid != null) && (componentid > 0) && (computerid != null) && (computerid >0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred)
            {
                var query = from installation in db.Installations
                            where ((installation.ComputerID == computerid)
                                    && (installation.ComponentID == componentid)
                                    && (installation.Release == release)
                                    && (installation.StartDateTime == startdatetime))
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID 
                            join vendor in db.Vendors
                            on component.VendorID equals vendor.VendorID 
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
                                VendorName = vendor.VendorName
                            };
                installationVM = query.Single();
                if (installationVM == null)
                {
                    installationVM.Message.Fill("Installatie - Bewerken", installationVM.Message.Error, "*** ERROR *** Deze installatie staat niet in de database.");
                }
                else
                {
                    installationVM.Message.Fill("Installatie - Bewerken", installationVM.Message.Info, "Voer wijzigingen in en klik op OPSLAAN");

                }

                return View(installationVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Component Bewerken (GET)";
                string l = installationVM.Message.Error;
                return RedirectToAction("Index", "Components", new { Message = m, MsgLevel = l });
            }

        }

        // POST: Installations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComputerID,ComponentID,Release,HiddenStartDateTime,Location,InstallDate,MeasuredDateTime,EndDateTime,Count")] InstallationVM installationVM)
        {
            if (ModelState.IsValid)
            {
                Installation installation = new Installation();
                installation.Fill(installationVM);
                db.Entry(installation).State = EntityState.Modified;
                db.SaveChanges();
                string m = "Installatie " + installationVM.Release.TrimEnd() + " is aangepast";
                string l = installationVM.Message.Info;
                return RedirectToAction("Index", "Installations", new { Message = m, MsgLevel = l });
            }
            else
            {
                installationVM.Message.Fill("Installatie - Bewerken",
                        installationVM.Message.Error, "Model ERROR in " + installationVM.Release.TrimEnd());

                
                return View(installationVM);
            }

        }

        //
        // GET: Installations/Delete/5
        //
        public ActionResult Delete(int? computerid, int? componentid, string release, DateTime startdatetime)
        {
            InstallationVM installationVM = new InstallationVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(computerid > 0, "Contract Failed (computerid)!");
            Contract.Requires(componentid > 0, "Contract Failed (componentid)!");
            Contract.Requires(release != null, "Contract Failed (release)!");
            Contract.Requires(startdatetime != null, "Contract Failed (startdatetime)!");

            if (!ContractErrorOccurred)
            {
                var query = from installation in db.Installations
                            where ((installation.ComputerID == computerid)
                                    && (installation.ComponentID == componentid)
                                    && (installation.Release == release)
                                    && (installation.StartDateTime == startdatetime))
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID 
                            join vendor in db.Vendors
                            on component.VendorID equals vendor.VendorID 
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
                                VendorName = vendor.VendorName
                            };
                installationVM = query.Single();
                if (installationVM == null)
                {
                    installationVM.Message.Fill("Installatie - Verwijderen", installationVM.Message.Error, "*** ERROR *** Deze installatie staat niet in de database.");

                }
                else
                {
                    installationVM.Message.Fill("Installatie - Verwijderen", installationVM.Message.Info, "Klik op VERWIJDEREN om deze installatie te verwijderen");
                }
                
                return View(installationVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Installatie Verwijderen (GET)";
                string l = installationVM.Message.Error;
                return RedirectToAction("Index", "Installations", new { Message = m, MsgLevel = l });
            }
        }


        // POST: Installations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? computerid, int? componentid, string release, DateTime startdatetime)
        {
            SympaMessage msg = new SympaMessage();
            string m = "";
            string l = "";
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(computerid > 0, "Contract Failed (computerid)!");
            Contract.Requires(componentid > 0, "Contract Failed (componentid)!");
            Contract.Requires(release != null, "Contract Failed (release)!");
            Contract.Requires(startdatetime != null, "Contract Failed (startdatetime)!");

            if (!ContractErrorOccurred)
            {
                
                Installation installation = db.Installations.Find(computerid,componentid,release,startdatetime);
                db.Installations.Remove(installation);
                db.SaveChanges();
                m = "Installation " + installation.Release.TrimEnd() + " is verwijderd.";
                l = msg.Info;
            }
            else
            {
                ContractErrorOccurred = false;
                m = "Contract error bij Installatie Verwijderen (POST)";
                l = msg.Error;

            }
            return RedirectToAction("Index", "Installations", new { Message = m, MsgLevel = l });
        }

        public ActionResult Report01()
        {
            InstallationIndex index = new InstallationIndex();
            index.Message.Title = "Rapport - Installaties met een Count > 1";

            index.Message.Tekst = "Installaties die meer dan één keer aanwezig zijn op een computer";
            index.Message.Level = index.Message.Info;

            var query = from installation in db.Installations
                        join component in db.Components
                        on installation.ComponentID equals component.ComponentID 
                        join computer in db.Computers
                        on installation.ComputerID equals computer.ComputerID
                        join vendor in db.Vendors
                        on component.VendorID equals vendor.VendorID
                        where installation.Count > 1
                        orderby computer.ComputerName,vendor.VendorGroup,vendor.VendorName,component.ComponentNameTemplate, installation.StartDateTime, installation.EndDateTime
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
                            ComputerName = computer.ComputerName
                        };
            index.InstallationLijst = query.ToList();

            return View(index);


        }
        public ActionResult Report02()
        {
            {
                InstallationIndex index = new InstallationIndex();
                index.Message.Title = "Rapport - Installaties met een gedeelde locatie";

                index.Message.Tekst = "Installaties die dezelfde installatielocatie hebben";
                index.Message.Level = index.Message.Info;

                var query = from installation in db.Installations
                            where installation.EndDateTime == null && installation.Location != null && installation.Location != ""
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID 
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID 
                            where (from i in db.Installations
                                   where i.ComputerID == installation.ComputerID && i.EndDateTime == null 
                                                    && i.Location != null && i.Location != ""
                                   group i by i.Location into grp
                                   where grp.Count() > 1
                                   select grp.Key)
                                   .Contains(installation.Location)
                            orderby installation.ComputerID, installation.Location
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
                            ComputerName = computer.ComputerName
                            };
                         
                index.InstallationLijst = query.ToList();
                return View(index);
            }
        }

        public ActionResult Report03()
        {
            {
                InstallationIndex index = new InstallationIndex();
                index.Message.Title = "Rapport - Installaties met recente einddatum";

                index.Message.Tekst = "Installaties die recentelijk zijn verdwenen van het systeem";
                index.Message.Level = index.Message.Info;

                DateTime dt = DateTime.Now.AddDays(-100);

                var query = from installation in db.Installations
                            where installation.EndDateTime != null && installation.EndDateTime > dt                            
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID 
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID 
                            orderby computer.ComputerName, installation.EndDateTime descending, component.ComponentNameTemplate
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
                                ComputerName = computer.ComputerName
                            }
                            
                            ;

                index.InstallationLijst = query.ToList();
                return View(index);
            }
        }

        public ActionResult Report04()
        {
            {
                InstallationIndex index = new InstallationIndex();
                index.Message.Title = "Rapport - Installaties met recente startdatum";

                index.Message.Tekst = "Installaties die recentelijk zijn verschenen op het systeem";
                index.Message.Level = index.Message.Info;

                DateTime dt = DateTime.Now.AddDays(-100);

                var query = from installation in db.Installations
                            where installation.StartDateTime > dt
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID 
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID 
                            orderby computer.ComputerName, installation.StartDateTime descending, component.ComponentNameTemplate
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
                                ComputerName = computer.ComputerName
                            }

                            ;

                index.InstallationLijst = query.ToList();
                return View(index);
            }
        }
        public ActionResult Report05()
        {
            {
                InstallationCompare index = new InstallationCompare();
                index.Message.Title = "Rapport - Installaties met verschillende releases";

                index.Message.Tekst = "Releases die op één of meer systemen verschillen van elkaar";
                index.Message.Level = index.Message.Info;

                InstallationData idata = new InstallationData();

                // Determine unique computers
                var query1 = from i in db.Installations
                             group i by new { i.ComputerID } into grp

                             join computer in db.Computers
                             on grp.Key.ComputerID equals computer.ComputerID
                             select new ComputerVM
                             {
                                 ComputerID = grp.Key.ComputerID,
                                 ComputerName = computer.ComputerName
                             };
                index.ComputerLijst = query1.ToList();

                // Determine unique installed components (OVERALL)
                var query2 = from i in db.Installations
                             where i.EndDateTime == null
                             group i by new { i.ComponentID, i.ComponentName, i.Release } into grp

                             join component in db.Components
                             on grp.Key.ComponentID equals component.ComponentID
                             select new
                             {
                                 grp.Key.ComponentID,
                                 component.ComponentNameTemplate,
                                 grp.Key.ComponentName,
                                 grp.Key.Release,
                                 component.VendorID

                             } into join1
                             join vendor in db.Vendors
                             on join1.VendorID equals vendor.VendorID
                             orderby vendor.VendorGroup, vendor.VendorName, join1.ComponentNameTemplate
                             select new InstallationRelease
                             {
                                 ComponentID = join1.ComponentID,
                                 ComponentNameTemplate = join1.ComponentNameTemplate,
                                 ComponentName = join1.ComponentName,
                                 Release = join1.Release,
                                 VendorName = vendor.VendorName,
                                 Matched = false

                             };
                idata.ComponentLijst = query2.ToList();

                // Get active component/releaselist for each computer
                foreach (ComputerVM comp in index.ComputerLijst)
                {
                    int currentcomp = comp.ComputerID;
                    var query3 = from i in db.Installations

                                 where i.ComputerID == currentcomp && i.EndDateTime == null
                                 group i by new { i.ComponentID, i.ComponentName,i.Release } into grp

                                 select new InstallationRelease
                                 {
                                     ComponentID = grp.Key.ComponentID,
                                     ComponentName = grp.Key.ComponentName,
                                     Release = grp.Key.Release

                                 }
                                 ;
                    List<InstallationRelease> thislist = new List<InstallationRelease>();
                    thislist = query3.ToList();
                    idata.InstallationReleaseOverview.Add(thislist);
                }

                // first exclude all release that are the same on all computers
                int aantalcomputers = idata.InstallationReleaseOverview.Count;               

                foreach (InstallationRelease installationrelease in idata.ComponentLijst)
                {
                    int matchcounter = 0;
                    foreach (List<InstallationRelease> installationReleaseLijst in idata.InstallationReleaseOverview)
                    {
                        var findrelease = installationReleaseLijst.Find(x => x.ComponentID == installationrelease.ComponentID &&
                                                                                x.ComponentName == installationrelease.ComponentName &&
                                                                                x.Release == installationrelease.Release);
                        if (findrelease != null) {
                            matchcounter ++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (matchcounter == aantalcomputers)
                    {
                        installationrelease.Matched = true;                  // all computers have this release installed
                    }
                }

                // now handle all releases that are NOT present on all systems 

                int currentid = 0;
                string currentrelease = "###";
                bool addheader = false;
                bool compgroup = false;
                foreach (InstallationRelease installationrelease in idata.ComponentLijst)
                {
                   
                    if (installationrelease.Matched)
                    {
                        continue;               // Skip already matched releases
                    }
                    
                    if (installationrelease.ComponentID == currentid)
                    {
                        if (installationrelease.ComponentName == installationrelease.ComponentNameTemplate)
                        {
                            continue;
                        }
                        if (installationrelease.Release == currentrelease)
                        {
                            continue;
                        }
                        else
                        {
                            currentrelease = installationrelease.Release;
                        }
                    }
                    else // new component starts
                    {
                        currentid = installationrelease.ComponentID;
                        currentrelease = installationrelease.Release;
                        if (installationrelease.ComponentName != installationrelease.ComponentNameTemplate)
                        {
                            addheader = true;
                            compgroup = true;                            
                        }
                        else
                        {
                            compgroup = false; 
                        }
                    }
                    
                    InstallationReportLine irl = new InstallationReportLine();
                    InstallationReportLine irlH = new InstallationReportLine();
                    if (addheader)
                    {
                        irlH.ComponentNameTemplate = installationrelease.ComponentNameTemplate;
                        irlH.ComponentName = installationrelease.ComponentName;
                        irlH.VendorName = installationrelease.VendorName;
                    }
                    int aantalreleases = 0;                    
                    bool firstrelease = true;
                    bool mismatchfound = false;
                    string comparerelease = null;
                    int compnumber = 0;
                    foreach (List<InstallationRelease> installationReleaseLijst in idata.InstallationReleaseOverview)
                    {
                        if (addheader)
                        {
                            irlH.Indicator.Add("========");                            
                        }
                        var findcomponent = installationReleaseLijst.Find(x => x.ComponentID == installationrelease.ComponentID);
                        var findcomponent2 = installationReleaseLijst.Find(x => x.ComponentName == installationrelease.ComponentName);

                        if (compgroup)
                        {
                            if (findcomponent != null)
                            {
                                compnumber++; 
                            }
                            if (findcomponent2 == null)
                            {
                                irl.Indicator.Add("n/a");
                                mismatchfound = true;
                            }
                            else
                            {
                                aantalreleases++;
                                if (firstrelease)
                                {
                                    comparerelease = findcomponent2.Release;
                                    firstrelease = false;
                                }
                                if (comparerelease != findcomponent2.Release)
                                {
                                    mismatchfound = true;
                                }
                                irl.Indicator.Add(findcomponent2.Release);
                            }                          

                        }
                        else
                        {
                            if (findcomponent == null)
                            {
                                irl.Indicator.Add("n/a");
                            }
                            else
                            {
                                aantalreleases++;
                                compnumber++;
                                if (firstrelease)
                                {
                                    comparerelease = findcomponent.Release;
                                    firstrelease = false;
                                }
                                if (comparerelease != findcomponent.Release)
                                {
                                    mismatchfound = true;
                                }
                                irl.Indicator.Add(findcomponent.Release);
                            }
                        }
                    }
                    
                    if (((aantalreleases > 1) || (compgroup)) && mismatchfound && (compnumber > 1))
                    {
                        if (addheader)
                        {
                            index.InstallationReport.Add(irlH);
                            
                        }
                        if (compgroup)
                        {
                            irl.ComponentNameTemplate = "......... " + installationrelease.ComponentName;
                        } 
                        else
                        {
                            irl.ComponentNameTemplate = installationrelease.ComponentNameTemplate;
                        }
                        irl.ComponentName = installationrelease.ComponentName;
                        irl.VendorName = installationrelease.VendorName;
                        index.InstallationReport.Add(irl);
                    }
                    addheader = false;


                }
                                    
                return View(index);
            }
        }

        public ActionResult Report06()
        {
            {
                InstallationCompare index = new InstallationCompare();
                index.Message.Title = "Rapport - Componenten die ontbreken";

                index.Message.Tekst = "Componenten die op één of meer systemen niet aanwezig zijn";
                index.Message.Level = index.Message.Info;

                InstallationData idata = new InstallationData();    

                // Determine unique computers
                var query1 = from i in db.Installations
                            group i by new { i.ComputerID } into grp

                            join computer in db.Computers
                            on grp.Key.ComputerID equals computer.ComputerID
                            select new ComputerVM
                            {
                                ComputerID = grp.Key.ComputerID,
                                ComputerName = computer.ComputerName
                            }; 
                index.ComputerLijst = query1.ToList();

                // Determine unique installed components
                var query2 = from i in db.Installations
                             where i.EndDateTime == null
                             group i by new { i.ComponentID, i.Release} into grp

                             join component in db.Components
                             on grp.Key.ComponentID equals component.ComponentID
                             select new 
                             {
                                 grp.Key.ComponentID,
                                 component.ComponentNameTemplate,
                                 grp.Key.Release,
                                 component.VendorID

                             } into join1
                             join vendor in db.Vendors
                             on join1.VendorID equals vendor.VendorID
                             orderby vendor.VendorGroup, vendor.VendorName, join1.ComponentNameTemplate
                             select new InstallationRelease
                             {
                                 ComponentID = join1.ComponentID,
                                 ComponentNameTemplate = join1.ComponentNameTemplate,
                                 Release = join1.Release,
                                 VendorName = vendor.VendorName

                             };
                idata.ComponentLijst = query2.ToList();

                // Get component/releaselist for each computer
                foreach (ComputerVM comp in index.ComputerLijst)
                {
                    int currentcomp = comp.ComputerID;
                    var query3 = from i in db.Installations
                                 where i.EndDateTime == null
                                 where i.ComputerID == currentcomp
                                 group i by new { i.ComponentID, i.Release } into grp

                                 select new InstallationRelease
                                 {
                                     ComponentID = grp.Key.ComponentID,
                                     Release = grp.Key.Release

                                 } 
                                 ;
                    List<InstallationRelease> thislist = new List<InstallationRelease>();
                    thislist = query3.ToList();
                    idata.InstallationReleaseOverview.Add(thislist);
                }

                int currentid = 0;
                foreach (InstallationRelease installationrelease in idata.ComponentLijst)
                {
                    if (currentid == installationrelease.ComponentID)
                    {
                        continue;
                    }
                    else
                    {
                        currentid = installationrelease.ComponentID;
                    }
                    InstallationReportLine irl = new InstallationReportLine();
                    Boolean nullfound = false;
                    
                    foreach (List<InstallationRelease> installationReleaseLijst in idata.InstallationReleaseOverview)
                    {
                        
                        var findcomponent = installationReleaseLijst.Find(x => x.ComponentID == installationrelease.ComponentID);

                        if (findcomponent == null)
                        {
                            nullfound = true;
                            irl.Indicator.Add("n/a");
                        }
                        else
                        {
                            irl.Indicator.Add(findcomponent.Release); 
                        }
                    }
                    if (nullfound)
                    {
                        irl.ComponentNameTemplate = installationrelease.ComponentNameTemplate;
                        irl.VendorName = installationrelease.VendorName;
                        index.InstallationReport.Add( irl );
                    }
                                     

                }

                return View(index);
            }
        }

        public ActionResult Report07()
        {
            {
                InstallationIndex index = new InstallationIndex();
                index.Message.Title = "Rapport - Inactieve installaties";

                index.Message.Tekst = "Installaties die inmiddels zijn verdwenen van het betreffende systeem";
                index.Message.Level = index.Message.Info;

                var query = from installation in db.Installations
                            where installation.EndDateTime != null
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID
                            orderby computer.ComputerName, installation.StartDateTime descending, component.ComponentNameTemplate
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
                                ComputerName = computer.ComputerName
                            }

                            ;

                index.InstallationLijst = query.ToList();
                return View(index);
            }
        }

        public ActionResult Report08()
        {
            {
                InstallationIndex index = new InstallationIndex();
                index.Message.Title = "Rapport - Actieve installaties";

                index.Message.Tekst = "Installaties die nu geïnstalleerd zijn op het betreffende systeem";
                index.Message.Level = index.Message.Info;

                var query = from installation in db.Installations
                            where installation.EndDateTime == null
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID
                            orderby computer.ComputerName, installation.StartDateTime descending, component.ComponentNameTemplate
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
                                ComputerName = computer.ComputerName
                            }

                            ;

                index.InstallationLijst = query.ToList();
                return View(index);
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