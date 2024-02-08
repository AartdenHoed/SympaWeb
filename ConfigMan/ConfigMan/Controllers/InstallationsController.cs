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

namespace ConfigMan.Controllers
{

    [LogActionFilter]
    [HandleError]

    public class InstallationsController : Controller
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
        // GET: Installations
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
                            Release = installation.Release.Trim(),
                            Location = installation.Location.Trim(),
                            InstallDate = installation.InstallDate,
                            MeasuredDateTime = installation.MeasuredDateTime,
                            StartDateTime = installation.StartDateTime,
                            EndDateTime = installation.EndDateTime,
                            Count = installation.Count,
                            ComponentName = j1.ComponentName,
                            ComputerName = j2.ComputerName
                        };
            List <InstallationVM> vmlist = query.ToList();
            
            return View(vmlist);

        }

        //
        // GET: Installations/Details/5
        //
        public ActionResult Details(int? computerid, int? componentid, string release, DateTime? startdatetime)
        {

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
                            on installation.ComponentID equals component.ComponentID into join1
                            from j1 in join1
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID into join2
                            from j2 in join2
                            select new InstallationVM
                            {
                                ComputerID = installation.ComputerID,
                                ComponentID = installation.ComponentID,
                                Release = installation.Release.Trim(),
                                Location = installation.Location.Trim(),
                                InstallDate = installation.InstallDate,
                                MeasuredDateTime = installation.MeasuredDateTime,
                                StartDateTime = installation.StartDateTime,
                                EndDateTime = installation.EndDateTime,
                                Count = installation.Count,
                                ComponentName = j1.ComponentName,
                                ComputerName = j2.ComputerName
                            };
                InstallationVM installationVM = query.Single();
                if (installationVM == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** Record not found in database.";
                }
                else
                {
                    return View(installationVM);
                }
            }
            return RedirectToAction("Index");
        }

        private void Contract_ContractFailed(object sender, ContractFailedEventArgs e)
        {
            ErrorMessage = "*** ERROR *** Selected installation id is invalid.";
            ContractErrorOccurred = true;
            e.SetHandled();
            return;
        }

        //
        // GET: Installations/Create
        //
        public ActionResult Create()
        {
            return View();
        }

        // POST: Installations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComputerID,ComponentID,Release,Location,InstallDate,MeasuredDateTime,StartDateTime,EndDateTime,Count")] InstallationVM installationVM)
        {
            if (ModelState.IsValid)
            {
                Installation installation = new Installation();
                installation.Fill(installationVM);
                bool addfailed = false;
                db.Installations.Add(installation);
                try
                {
                    db.SaveChanges();
                    TempData["SympaMsg"] = "Installation " + installation.Release + " succesfully added.";
                }
                catch (DbUpdateException)
                {
                    addfailed = true;
                    ViewBag.SympaMsg = "Installation " + installation.Release + " already exists, use update function.";
                }
                if (addfailed) { return View(); }
                else { return RedirectToAction("Index"); }
            }

            return View(installationVM);
        }

        //
        // GET: Installations/Edit/5
        //
        public ActionResult Edit(int? computerid, int? componentid)
        {
            Contract.Requires((componentid != null) && (componentid > 0) && (computerid != null) && (computerid >0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred)
            {
                var query = from installation in db.Installations
                            where ((installation.ComputerID == computerid) && (installation.ComponentID == componentid))
                            join component in db.Components
                            on installation.ComponentID equals component.ComponentID into join1
                            from j1 in join1
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID into join2
                            from j2 in join2
                            select new InstallationVM
                            {
                                ComputerID = installation.ComputerID,
                                ComponentID = installation.ComponentID,
                                Release = installation.Release.Trim(),
                                Location = installation.Location.Trim(),
                                InstallDate = installation.InstallDate,
                                MeasuredDateTime = installation.MeasuredDateTime,
                                StartDateTime = installation.StartDateTime,
                                EndDateTime = installation.EndDateTime,
                                Count = installation.Count,
                                ComponentName = j1.ComponentName,
                                ComputerName = j2.ComputerName
                            };
                InstallationVM installationVM = query.Single();
                if (installationVM == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** Record not found in database.";
                }
                else
                {
                    return View(installationVM);
                }
            }
            return RedirectToAction("Index");

        }

        // POST: Installations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComputerID,ComponentID,Release,Location,InstallDate,MeasuredDateTime,StartDateTime,EndDateTime,Count")] InstallationVM installationVM)
        {
            if (ModelState.IsValid)
            {
                Installation installation = new Installation();
                installation.Fill(installationVM);
                db.Entry(installation).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SympaMsg"] = "Installation " + installation.Release + " succesfully updated.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["SympaMsg"] = "Update failed for installation " + installationVM.Release;
                return View(installationVM);
            }

        }

        //
        // GET: Installations/Delete/5
        //
        public ActionResult Delete(int? computerid, int? componentid, string release, DateTime startdatetime)
        {
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
                            on installation.ComponentID equals component.ComponentID into join1
                            from j1 in join1
                            join computer in db.Computers
                            on installation.ComputerID equals computer.ComputerID into join2
                            from j2 in join2
                            select new InstallationVM
                            {
                                ComputerID = installation.ComputerID,
                                ComponentID = installation.ComponentID,
                                Release = installation.Release.Trim(),
                                Location = installation.Location.Trim(),
                                InstallDate = installation.InstallDate,
                                MeasuredDateTime = installation.MeasuredDateTime,
                                StartDateTime = installation.StartDateTime,
                                EndDateTime = installation.EndDateTime,
                                Count = installation.Count,
                                ComponentName = j1.ComponentName,
                                ComputerName = j2.ComputerName
                            };
                InstallationVM installationVM = query.Single();
                if (installationVM == null)
                {
                    TempData["SympaMsg"] = "*** ERROR *** Record not found in database.";
                }
                else
                {
                    return View(installationVM);
                }
            }
            return RedirectToAction("Index");

        }


        // POST: Installations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? computerid, int? componentid, string release, DateTime startdatetime)
        {
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
                TempData["SympaMsg"] = "Installation " + installation.Release + " succesfully deleted.";
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
            var query = from c in db.Installations
                        select new Installation();

            List < Installation > instlist = query.ToList();

            return View(instlist);

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