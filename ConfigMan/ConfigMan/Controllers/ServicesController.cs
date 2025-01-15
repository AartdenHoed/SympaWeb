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
using System.Reflection;

namespace ConfigMan.Controllers
{
    public class ServicesController : Controller
    {
        private readonly DbEntities db = new DbEntities();
        private static bool ContractErrorOccurred = false;

        // GET: Services
        public ActionResult Index(string messageP, string msgLevelP, string filterstrP, string subsetstrP,
            string systeemfilterP, string servicenaamfilterP, string changestatefilterP, string directoryfilterP, string templatefilterP,
            string componentfilterP, string programfilterP)
        {
            ServiceIndex index = new ServiceIndex();

            index.FilterData.Fill(filterstrP, subsetstrP,
                            systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

            string t = "Services - Overzicht";
            string m = messageP;
            string l = msgLevelP;
            if (m is null)
            {
                if (!index.FilterData.Filter)
                {
                    m = "Klik op een actie voor een bestaande service";
                    l = index.Message.Info;
                }
                else
                {
                    m = "Klik op een actie voor een bestaande service (LIJST IS GEFILTERD)";
                    l = index.Message.Info;
                }
            }
            index.Message.Fill(t, l, m);

            try
            {
                switch (index.FilterData.Subsetstr)
                {
                    case "A":

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
                                         DirectoryTemplate = service.DirectoryTemplate,
                                         ProgramName = service.ProgramName,
                                         Parameter = service.Parameter,
                                         ComputerID = service.ComputerID
                                     };
                        index.ServiceLijst = queryA.ToList();
                        break;

                    case "G":
                        var queryG = from service in db.Services
                                     where !(from i in db.Installations
                                             where ((i.EndDateTime == null) && (i.ComputerID == service.ComputerID))
                                             select i.ComponentID)
                                        .Contains(service.ComponentID)
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
                                         DirectoryTemplate = service.DirectoryTemplate,
                                         ProgramName = service.ProgramName,
                                         Parameter = service.Parameter,
                                         ComputerID = service.ComputerID
                                     };
                        index.ServiceLijst = queryG.ToList();
                        break;
                    default:
                        m = "Subset waarde '" + index.FilterData.Subsetstr + "' is ongeldig";
                        l = index.Message.Error;
                        index.Message.Fill(t, l, m);
                        return View(index);
                }

                if (index.ServiceLijst.Count == 0)
                {
                    m = "Geen service gevonden";
                    l = index.Message.Warning;
                }
            }
            catch (Exception e)
            {
                m = e.Message;
            }
            if (!index.FilterData.Filter)
            {
                return View(index);
            }
            else
            {
                try
                {
                    if ((!string.IsNullOrEmpty(index.FilterData.SysteemFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query2 = from sv in index.ServiceLijst
                                     where sv.SystemName.ToUpper().Contains(index.FilterData.SysteemFilter.ToUpper())
                                     select sv;
                        index.ServiceLijst = query2.ToList();
                    }
                    if ((!string.IsNullOrEmpty(index.FilterData.ServiceNaamFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query3 = from sv in index.ServiceLijst
                                     where sv.Name.ToUpper().Contains(index.FilterData.ServiceNaamFilter.ToUpper())
                                     select sv;
                        index.ServiceLijst = query3.ToList();
                    }
                    if ((!string.IsNullOrEmpty(index.FilterData.ChangeStateFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query4 = from sv in index.ServiceLijst
                                     where sv.ChangeState.ToUpper().Contains(index.FilterData.ChangeStateFilter.ToUpper())
                                     select sv;
                        index.ServiceLijst = query4.ToList();
                    }
                    if ((!string.IsNullOrEmpty(index.FilterData.ComponentFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query5 = from sv in index.ServiceLijst
                                     where sv.ComponentName.ToUpper().Contains(index.FilterData.ComponentFilter.ToUpper())
                                     select sv;
                        index.ServiceLijst = query5.ToList();
                    }
                    if ((!string.IsNullOrEmpty(index.FilterData.DirectoryFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query6 = from sv in index.ServiceLijst
                                     where sv.DirName.ToUpper().Contains(index.FilterData.DirectoryFilter.ToUpper())
                                     select sv;
                        index.ServiceLijst = query6.ToList();
                    }
                    if ((!string.IsNullOrEmpty(index.FilterData.TemplateFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query7A = from sva in index.ServiceLijst
                                      where (sva.DirectoryTemplate != null)
                                      select sva;
                        index.ServiceLijst = query7A.ToList();
                        if (index.ServiceLijst.Count > 0)
                        {
                            var query7B = from svb in index.ServiceLijst
                                          where svb.DirectoryTemplate.ToUpper().Contains(index.FilterData.TemplateFilter.ToUpper())
                                          select svb;
                            index.ServiceLijst = query7B.ToList();
                        }
                    }
                    if ((!string.IsNullOrEmpty(index.FilterData.ProgramFilter)) && (index.ServiceLijst.Count > 0))
                    {
                        var query8 = from sv in index.ServiceLijst
                                     where sv.ProgramName.ToUpper().Contains(index.FilterData.ProgramFilter.ToUpper())
                                     select sv;
                        index.ServiceLijst = query8.ToList();
                    }
                }
                catch (Exception e)
                {
                    m = e.Message;
                    l = index.Message.Error;
                    index.Message.Fill(t, l, m);
                    return View(index);
                }
            }

            if (index.ServiceLijst.Count == 0)
            {
                m = "Geen enkele service voldoet aan de ingegeven filterwaarden ";
                l = index.Message.Warning;
            }
            index.Message.Fill(t, l, m);
            return View(index);
        }

        // GET: Services/Details/5
        public ActionResult Details(int id, string name,
            string filterstrP, string subsetstrP,
            string systeemfilterP, string servicenaamfilterP, string changestatefilterP, string directoryfilterP, string templatefilterP, string componentfilterP, string programfilterP)
        {
            ServiceVM serviceVM = new ServiceVM(); 
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Computer ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldige Service Naam op");
            string t = "Service - Bekijken";
            string l = "?";
            string m = "?";

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Service Bekijken (GET)";
                l = serviceVM.Message.Error;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                            systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, 
                            componentfilterP, programfilterP);

                return RedirectToAction("Index", "Services", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = serviceVM.FilterData.Filterstr,
                    subsetstrP = serviceVM.FilterData.Subsetstr,
                    systeemfilterP = serviceVM.FilterData.SysteemFilter,
                    servicenaamfilterP = serviceVM.FilterData.ServiceNaamFilter,
                    changestatefilterP = serviceVM.FilterData.ChangeStateFilter,
                    directoryfilterP = serviceVM.FilterData.DirectoryFilter,
                    templatefilterP = serviceVM.FilterData.TemplateFilter,
                    componentfilterP = serviceVM.FilterData.ComponentFilter,
                    programfilterP = serviceVM.FilterData.ProgramFilter
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
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                           systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

                return View(serviceVM);
            }
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int id, string name,
            string filterstrP, string subsetstrP,
            string systeemfilterP, string servicenaamfilterP, string changestatefilterP, string directoryfilterP, string templatefilterP, 
            string componentfilterP, string programfilterP)
        {
            ServiceVM serviceVM = new ServiceVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Computer ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldige Service Naam op");
            string t = "Service - Bewerken";
            string l = "?";
            string m = "?";

            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Service Bewerken (GET)";
                l = serviceVM.Message.Error;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                           systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

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
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                           systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

                return View(serviceVM);
            }
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceVM serviceVM,
            string filterstrP, string subsetstrP,
            string systeemfilterP, string servicenaamfilterP, string changestatefilterP, string directoryfilterP, string templatefilterP,
            string componentfilterP, string programfilterP)
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
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                           systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);
                return RedirectToAction("Index", "Services", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = serviceVM.FilterData.Filterstr,
                    subsetstrP = serviceVM.FilterData.Subsetstr,
                    systeemfilterP = serviceVM.FilterData.SysteemFilter,
                    servicenaamfilterP = serviceVM.FilterData.ServiceNaamFilter,
                    changestatefilterP = serviceVM.FilterData.ChangeStateFilter,
                    directoryfilterP = serviceVM.FilterData.DirectoryFilter,
                    templatefilterP = serviceVM.FilterData.TemplateFilter,
                    componentfilterP = serviceVM.FilterData.ComponentFilter,
                    programfilterP = serviceVM.FilterData.ProgramFilter
                });
            }
            else
            {
                List<Component> clist = db.Components.OrderBy(x => x.ComponentNameTemplate).ToList();

                // Set first entry on current value
                ComponentVM firstentry = new ComponentVM()
                {
                    ComponentID = serviceVM.ComponentID,
                    ComponentNameTemplate = db.Components.Find(serviceVM.ComponentID).ComponentNameTemplate
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
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                            systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);
                return View(serviceVM);
            }

        }

        // GET: Services/Delete/5
        public ActionResult Delete(int id, string name,
            string filterstrP, string subsetstrP,
            string systeemfilterP, string servicenaamfilterP, string changestatefilterP, string directoryfilterP, string templatefilterP,
            string componentfilterP, string programfilterP)
        {
            ServiceVM serviceVM = new ServiceVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Computer ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldige Service Naam op");
            string t = "Service - Verwijderen";
            string l = "?";
            string m = "?";
            
            if (ContractErrorOccurred)
            {
                ContractErrorOccurred = false;

                m = "Contract error bij Service Verwijderen (GET)";
                l = serviceVM.Message.Error;
                serviceVM.Message.Fill(t, l, m);
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                             systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

                return RedirectToAction("Index", "Services", new
                {
                    messageP = m,
                    msgLevelP = l,
                    filterstrP = serviceVM.FilterData.Filterstr,
                    subsetstrP = serviceVM.FilterData.Subsetstr,
                    systeemfilterP = serviceVM.FilterData.SysteemFilter,
                    servicenaamfilterP = serviceVM.FilterData.ServiceNaamFilter,
                    changestatefilterP = serviceVM.FilterData.ChangeStateFilter,
                    directoryfilterP = serviceVM.FilterData.DirectoryFilter,
                    templatefilterP = serviceVM.FilterData.TemplateFilter,
                    componentfilterP = serviceVM.FilterData.ComponentFilter,
                    programfilterP = serviceVM.FilterData.ProgramFilter
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
                serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                            systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

                return View(serviceVM);
            }
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string name,
            string filterstrP, string subsetstrP,
            string systeemfilterP, string servicenaamfilterP, string changestatefilterP, string directoryfilterP, string templatefilterP,
            string componentfilterP, string programfilterP)
        {
            ServiceVM serviceVM = new ServiceVM();
            SympaMessage msg = new SympaMessage();
            string m = "";
            string l = "";
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Geef een geldig Comuter ID op");
            Contract.Requires(!string.IsNullOrEmpty(name), "Geef een geldig Service Naam op");
            serviceVM.FilterData.Fill(filterstrP, subsetstrP,
                            systeemfilterP, servicenaamfilterP, changestatefilterP, directoryfilterP, templatefilterP, componentfilterP, programfilterP);

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
            return RedirectToAction("Index", "Services", new
            {
                messageP = m,
                msgLevelP = l,
                filterstrP = serviceVM.FilterData.Filterstr,
                subsetstrP = serviceVM.FilterData.Subsetstr,
                systeemfilterP = serviceVM.FilterData.SysteemFilter,
                servicenaamfilterP = serviceVM.FilterData.ServiceNaamFilter,
                changestatefilterP = serviceVM.FilterData.ChangeStateFilter,
                directoryfilterP = serviceVM.FilterData.DirectoryFilter,
                templatefilterP = serviceVM.FilterData.TemplateFilter, 
                componentfilterP = serviceVM.FilterData.ComponentFilter,
                programfilterP = serviceVM.FilterData.ProgramFilter
            });
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
