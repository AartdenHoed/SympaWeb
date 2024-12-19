using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using System.Web.UI.WebControls;
using ConfigMan;
using ConfigMan.ViewModels;

namespace ConfigMan.Controllers
{
    public class ServicesController : Controller
    {
        private readonly DbEntities db = new DbEntities();
        private static bool ContractErrorOccurred = false;

        // GET: Services
        public ActionResult Index(string messageP, string msgLevelP)
        {
            ServiceIndex index = new ServiceIndex();

            string t = "Services - Overzicht";
            string m = messageP;
            string l = msgLevelP;
            if (m is null)
            {
               m = "Klik op een actie voor een bestaande service";
               l = index.Message.Info;
            }
            index.Message.Fill(t, l, m);

            var queryA = from service in db.Services
                         join component in db.Components
                         on service.ComponentID equals component.ComponentID
                         join computer in db.Computers
                         on service.ComputerID equals computer.ComputerID
                         orderby computer.ComputerName, service.Name
                         select new ServiceVM
                         {
                             SystemName = service.SystemName,
                             Name = service.Name,
                             DisplayName = service.DisplayName,
                             ChangeState = service.ChangeState,
                             ComponentName = component.ComponentNameTemplate,
                             DirName = service.DirName,
                             ProgramName = service.ProgramName,
                             Parameter = service.Parameter,
                             ComputerID = service.ComputerID
                         };
                             
                         
                           
            index.ServiceLijst = queryA.ToList();
                   
           
            if (index.ServiceLijst.Count == 0)
            {
                m = "Geen service gevonden";
                l = index.Message.Warning;
                
            }
            index.Message.Fill(t, l, m);
            return View(index);
        }

        // GET: Services/Details/5
        public ActionResult Details(int? id, string name)
        {
            ServiceVM serviceVM = new ServiceVM(); 
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Comuter ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldig Service Naam op");
            string t = "Service - Bekijken";
            string l = "?";
            string m = "?";

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Service Bekijken (GET)";
                l = serviceVM.Message.Error;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return RedirectToAction("Index", "Services", new
                {
                    messageP = m,
                    msgLevelP = l

                });
            }
            else
            {
                var query = from service in db.Services
                            where ((service.ComputerID == id) && (service.Name == name))
                            join component in db.Components
                            on service.ComponentID equals component.ComponentID 
                            join computer in db.Computers
                            on service.ComputerID equals computer.ComputerID
                            orderby computer.ComputerName, service.Name
                            select new ServiceVM
                            {
                                PSComputerName = service.PSComputerName     ,
                                SystemName = service.SystemName     ,
                                Name = service.Name     ,
                                ComputerID = service.ComputerID     ,
                                ComponentID = service.ComponentID      ,
                                ComponentName = component.ComponentNameTemplate,
                                Suffix =  service.Suffix    ,
                                Caption = service.Caption     ,
                                DisplayName = service.DisplayName     ,
                                PathName =  service.PathName    ,
                                ServiceType = service.ServiceType     ,
                                StartMode = service.StartMode     ,
                                Started = service.Started      ,
                                State = service.State     ,
                                Status =  service.Status    ,
                                ExitCode = service.ExitCode     ,
                                Description = service.Description     ,
                                Software = service.Software     ,
                                DirectoryTemplate =  service.DirectoryTemplate,   
                                DirName =  service.DirName   ,
                                ProgramName = service.ProgramName     ,
                                Parameter =  service.Parameter    ,
                                ChangeState =  service.ChangeState    ,
                                StartDate =  service.StartDate    ,
                                CheckDate =  service.CheckDate    ,
                                OldChangeState = service.OldChangeState     ,
                                OldDirName =  service.OldDirName    ,
                                OldProgramName = service.OldProgramName      ,
                                OldParameter = service.OldParameter};
                serviceVM = query.Single();

                if (serviceVM == null)
                {
                    l = serviceVM.Message.Error;
                    m = "*** ERROR *** Service " + name + " op computer met ID "  + id.ToString() + " staat niet in de database.";
                }
                else
                {
                    l = serviceVM.Message.Info;
                    m = "Klik op BEWERK om deze service te bewerken";
                }
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return View(serviceVM);
            }
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int? id, string name)
        {
            ServiceVM serviceVM = new ServiceVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Computer ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldig Service Naam op");
            string t = "Service - Bewerken";
            string l = "?";
            string m = "?";

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Service Bewerken (GET)";
                l = serviceVM.Message.Error;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return RedirectToAction("Index", "Services", new
                {
                    messageP = m,
                    msgLevelP = l

                });
            }
            else
            {
                var query = from service in db.Services
                            where ((service.ComputerID == id) && (service.Name == name))
                            join component in db.Components
                            on service.ComponentID equals component.ComponentID
                            join computer in db.Computers
                            on service.ComputerID equals computer.ComputerID
                            orderby computer.ComputerName, service.Name
                            select new ServiceVM
                            {
                                PSComputerName = service.PSComputerName,
                                SystemName = service.SystemName,
                                Name = service.Name,
                                ComputerID = service.ComputerID,
                                ComponentID = service.ComponentID,
                                ComponentName = component.ComponentNameTemplate,
                                Suffix = service.Suffix,
                                Caption = service.Caption,
                                DisplayName = service.DisplayName,
                                PathName = service.PathName,
                                ServiceType = service.ServiceType,
                                StartMode = service.StartMode,
                                Started = service.Started,
                                State = service.State,
                                Status = service.Status,
                                ExitCode = service.ExitCode,
                                Description = service.Description,
                                Software = service.Software,
                                DirectoryTemplate = service.DirectoryTemplate,
                                DirName = service.DirName,
                                ProgramName = service.ProgramName,
                                Parameter = service.Parameter,
                                ChangeState = service.ChangeState,
                                StartDate = service.StartDate,
                                CheckDate = service.CheckDate,
                                OldChangeState = service.OldChangeState,
                                OldDirName = service.OldDirName,
                                OldProgramName = service.OldProgramName,
                                OldParameter = service.OldParameter
                            };
                serviceVM = query.Single();

                if (serviceVM == null)
                {
                    l = serviceVM.Message.Error;
                    m = "*** ERROR *** Service " + name + " op computer met ID " + id.ToString() + " staat niet in de database.";
                }
                else
                {
                    l = serviceVM.Message.Info;
                    m = "Voer wijzigingen in en klik op OPSLAAN";
                    List<Component> clist = db.Components.OrderBy(x => x.ComponentNameTemplate).ToList();

                    // Set first entry on current value
                    ComponentVM firstentry = new ComponentVM()
                    {
                        ComponentID = serviceVM.ComponentID,
                        ComponentNameTemplate = serviceVM.ComponentName,
                    };
                    serviceVM.ComponentLijst.Add(firstentry);
                    // add entries form Vendor db
                    foreach (Component c in clist)
                    {
                        ComponentVM CM = new ComponentVM();
                        CM.Fill(c);
                        serviceVM.ComponentLijst.Add(CM);
                    }
                }
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return View(serviceVM);
            }
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceVM serviceVM)
        {
            string t = "Service - Bewerken";
            string l = "?";
            string m = "?";
            if (ModelState.IsValid)
            {
                int selectedComponentID = Int32.Parse(serviceVM.SelectedComponentIDstring);
                Service service = new Service();
                service.Fill(serviceVM);
                if (selectedComponentID != serviceVM.ComponentID)
                {
                    service.ComponentID = selectedComponentID;
                }
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();

                m = "Service " + service.Name.TrimEnd() + " is aangepast";
                l = serviceVM.Message.Info;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();
                return RedirectToAction("Index", "Services", new
                {
                    messageP = serviceVM.Message.Tekst,
                    msgLevelP = serviceVM.Message.Level
                });
            }
            else
            {
                List<Component> clist = db.Components.OrderBy(x => x.ComponentNameTemplate).ToList();

                // Set first entry on current value
                ComponentVM firstentry = new ComponentVM()
                {
                    ComponentID = serviceVM.ComponentID,
                    ComponentNameTemplate = serviceVM.ComponentName,
                };
                serviceVM.ComponentLijst.Add(firstentry);
                // add entries form Vendor db
                foreach (Component c in clist)
                {
                    ComponentVM CM = new ComponentVM();
                    CM.Fill(c);
                    serviceVM.ComponentLijst.Add(CM);
                }
                
                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }
                string errorcount = modelErrors.Count.ToString();               

                l = serviceVM.Message.Error;
                m = errorcount + " Model ERRORS in " + serviceVM.Name + "  ==> First message = '" + modelErrors[0] + "'" ;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return View(serviceVM);
            }

        }

        // GET: Services/Delete/5
        public ActionResult Delete(int? id, string name)
        {
            ServiceVM serviceVM = new ServiceVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Comuter ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldig Service Naam op");
            string t = "Service - Verwijderen";
            string l = "?";
            string m = "?";
            
            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Service Verwijderen (GET)";
                l = serviceVM.Message.Error;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return RedirectToAction("Index", "Services", new
                {
                    messageP = m,
                    msgLevelP = l

                });
            }
            else
            {
                var query = from service in db.Services
                            where ((service.ComputerID == id) && (service.Name == name))
                            join component in db.Components
                            on service.ComponentID equals component.ComponentID
                            join computer in db.Computers
                            on service.ComputerID equals computer.ComputerID
                            orderby computer.ComputerName, service.Name
                            select new ServiceVM
                            {
                                PSComputerName = service.PSComputerName,
                                SystemName = service.SystemName,
                                Name = service.Name,
                                ComputerID = service.ComputerID,
                                ComponentID = service.ComponentID,
                                ComponentName = component.ComponentNameTemplate,
                                Suffix = service.Suffix,
                                Caption = service.Caption,
                                DisplayName = service.DisplayName,
                                PathName = service.PathName,
                                ServiceType = service.ServiceType,
                                StartMode = service.StartMode,
                                Started = service.Started,
                                State = service.State,
                                Status = service.Status,
                                ExitCode = service.ExitCode,
                                Description = service.Description,
                                Software = service.Software,
                                DirectoryTemplate = service.DirectoryTemplate,
                                DirName = service.DirName,
                                ProgramName = service.ProgramName,
                                Parameter = service.Parameter,
                                ChangeState = service.ChangeState,
                                StartDate = service.StartDate,
                                CheckDate = service.CheckDate,
                                OldChangeState = service.OldChangeState,
                                OldDirName = service.OldDirName,
                                OldProgramName = service.OldProgramName,
                                OldParameter = service.OldParameter
                            };
                serviceVM = query.Single();

                if (serviceVM == null)
                {
                    l = serviceVM.Message.Error;
                    m = "*** ERROR *** Service " + name + " op computer met ID " + id.ToString() + " staat niet in de database.";
                }
                else
                {
                    l = serviceVM.Message.Info;
                    m = "Klik op VERWJDER om deze service te verwijderen";
                }
                serviceVM.Message.Fill(t, l, m);
                serviceVM.ServiceFilter.Fill();

                return View(serviceVM);
            }
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string name)
        {
            ServiceVM serviceVM = new ServiceVM();
            SympaMessage msg = new SympaMessage();
            string m = "";
            string l = "";
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Comuter ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldig Service Naam op");                   

            if (!ContractErrorOccurred)
            {
                Service service = db.Services.Find(name, id);
                db.Services.Remove(service);
                db.SaveChanges();
                m = "Service " + service.Name.TrimEnd() + " is verwijderd.";
                l = msg.Info;
            }
            else
            {
                ContractErrorOccurred = false;
                m = "Contract error bij Service Verwijderen (POST)";
                l = msg.Error;

            }
            return RedirectToAction("Index", "Services", new { messageP = m, msgLevelP = l });
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
