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
        public ActionResult Index(string message, String msgLevel)
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
                        on installation.ComponentID equals component.ComponentID into join1
                        from j1 in join1
                        join computer in db.Computers
                        on installation.ComputerID equals computer.ComputerID into join2
                        from j2 in join2
                        orderby j2.ComputerName,j1.ComponentName,installation.StartDateTime,installation.EndDateTime
                        select new InstallationVM
                        {
                            ComputerID = installation.ComputerID,
                            ComponentID = installation.ComponentID,
                            Release = installation.Release.TrimEnd(),
                            Location = installation.Location.TrimEnd(),
                            InstallDate = installation.InstallDate,
                            MeasuredDateTime = installation.MeasuredDateTime,
                            StartDateTime = installation.StartDateTime,
                            EndDateTime = installation.EndDateTime,
                            Count = installation.Count,
                            ComponentName = j1.ComponentName,
                            ComputerName = j2.ComputerName
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
                            on installation.ComponentID equals component.ComponentID into join0                            
                            from j0 in join0
                            join vendor in db.Vendors
                            on j0.VendorID equals vendor.VendorID into join1
                            from j1 in join1
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID into join2
                            from j2 in join2
                            select new InstallationVM
                            {
                                ComputerID = installation.ComputerID,
                                ComponentID = installation.ComponentID,
                                Release = installation.Release.TrimEnd(),
                                Location = installation.Location.TrimEnd(),
                                InstallDate = installation.InstallDate,
                                MeasuredDateTime = installation.MeasuredDateTime,
                                StartDateTime = installation.StartDateTime,
                                EndDateTime = installation.EndDateTime,
                                Count = installation.Count,
                                ComponentName = j0.ComponentName,
                                ComputerName = j2.ComputerName,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName
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
            List<Component> componentdblist = db.Components.OrderBy(x => x.ComponentName).ToList();
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
            List<Component> componentdblist = db.Components.OrderBy(x => x.ComponentName).ToList();
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
                            on installation.ComponentID equals component.ComponentID into join0
                            from j0 in join0
                            join vendor in db.Vendors
                            on j0.VendorID equals vendor.VendorID into join1
                            from j1 in join1
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID into join2
                            from j2 in join2
                            select new InstallationVM
                            {
                                ComputerID = installation.ComputerID,
                                ComponentID = installation.ComponentID,
                                Release = installation.Release.TrimEnd(),
                                Location = installation.Location.TrimEnd(),
                                InstallDate = installation.InstallDate,
                                MeasuredDateTime = installation.MeasuredDateTime,
                                StartDateTime = installation.StartDateTime,
                                EndDateTime = installation.EndDateTime,
                                Count = installation.Count,
                                ComponentName = j0.ComponentName,
                                ComputerName = j2.ComputerName,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName
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
                            on installation.ComponentID equals component.ComponentID into join0
                            from j0 in join0
                            join vendor in db.Vendors
                            on j0.VendorID equals vendor.VendorID into join1
                            from j1 in join1
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID into join2
                            from j2 in join2
                            select new InstallationVM
                            {
                                ComputerID = installation.ComputerID,
                                ComponentID = installation.ComponentID,
                                Release = installation.Release.TrimEnd(),
                                Location = installation.Location.TrimEnd(),
                                InstallDate = installation.InstallDate,
                                MeasuredDateTime = installation.MeasuredDateTime,
                                StartDateTime = installation.StartDateTime,
                                EndDateTime = installation.EndDateTime,
                                Count = installation.Count,
                                ComponentName = j0.ComponentName,
                                ComputerName = j2.ComputerName,
                                VendorID = j1.VendorID,
                                VendorName = j1.VendorName
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
                        on installation.ComponentID equals component.ComponentID into join1
                        from j1 in join1
                        join computer in db.Computers
                        on installation.ComputerID equals computer.ComputerID into join2
                        from j2 in join2
                        where installation.Count > 1
                        orderby j2.ComputerName, j1.ComponentName, installation.StartDateTime, installation.EndDateTime
                        select new InstallationVM                       
                        {
                            ComputerID = installation.ComputerID,
                            ComponentID = installation.ComponentID,
                            Release = installation.Release.TrimEnd(),
                            Location = installation.Location.TrimEnd(),
                            InstallDate = installation.InstallDate,
                            MeasuredDateTime = installation.MeasuredDateTime,
                            StartDateTime = installation.StartDateTime,
                            EndDateTime = installation.EndDateTime,
                            Count = installation.Count,
                            ComponentName = j1.ComponentName,
                            ComputerName = j2.ComputerName
                        };
            index.InstallationLijst = query.ToList();

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