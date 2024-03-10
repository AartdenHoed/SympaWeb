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

namespace ConfigMan.Controllers { 

    [LogActionFilter]
    [HandleError]
   
    public class ComputersController : Controller
    {
        private readonly DbEntities db = new DbEntities();
        private static bool ContractErrorOccurred = false;
        
        public ActionResult Menu()
        {
            SympaMessage msg = new SympaMessage();
            msg.Fill("Computer - Beheer Menu", msg.Info, "Kies een optie");
            return View(msg);
        }

        //
        // GET: Computers
        //
        public ActionResult Index(string message, String msgLevel)
        {
            ComputerIndex index = new ComputerIndex();
            index.Message.Title = "Computer - Overzicht";
            
            if (message is null)
            {
                index.Message.Tekst = "Klik op NIEUWE COMPUTER om een computer aan te maken, of klik op een actie voor een bestaande computer";
                index.Message.Level = msgLevel;
            }
            else
            {
                index.Message.Tekst = message;
                index.Message.Level = msgLevel;
            }
            List<Computer> complist = db.Computers.OrderBy(x => x.ComputerName).ToList();
            
            foreach (Computer c in complist) {
                ComputerVM VM = new ComputerVM();
                VM.Fill(c);
                index.ComputerLijst.Add(VM); 
            }
            return View(index);

        }

        //
        // GET: Computers/Details/5
        //
        public ActionResult Details(int? id)
        {
            ComputerVM computerVM = new ComputerVM();
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Contract Failed!");

            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                if (computer == null)
                {
                    computerVM.Message.Fill("Computer - Bekijken", computerVM.Message.Error, "*** ERROR *** ComputerID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    computerVM.Message.Fill("Component - Bekijken", computerVM.Message.Info, "Klik op BEWERK om deze computer te bewerken");

                }
                
                computerVM.Fill(computer);
                return View(computerVM);
            }
            else
            {               
                ContractErrorOccurred = false;

                string m = "Contract error bij Computer Bekijken (GET)";
                string l = computerVM.Message.Error;
                return RedirectToAction("Index", "Computers", new { Message = m, MsgLevel = l });
                
            }
        }

       
        //
        // GET: Computers/Create
        //
        public ActionResult Create()
        {
            ComputerVM computerVM = new ComputerVM();
            computerVM.Message.Fill("Computer - Aanmaken", computerVM.Message.Info, "Klik op AANMAKEN om deze computer op te slaan");
            
            return View(computerVM);
        }

        // POST: Computers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComputerID,ComputerName,ComputerPurchaseDate,OS")] ComputerVM computerVM)
        {
            if (ModelState.IsValid)
            {
                Computer computer = new Computer();
                computer.Fill(computerVM);
                bool addfailed = false;
                db.Computers.Add(computer);
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
                    computerVM.Message.Fill("Computer - Aanmaken",
                        computerVM.Message.Error, "Computer " + computer.ComputerName + " bestaat al, gebruik de BEWERK functie.");
                }
                else
                {
                    string m = "Computer " + computer.ComputerName + " is toegevoegd";
                    string l = computerVM.Message.Info;
                    return RedirectToAction("Index", "Computers", new { Message = m, MsgLevel = l });

                }
            }
            else
            {
                computerVM.Message.Fill("Computer - Aanmaken",
                       computerVM.Message.Error, "Model ERROR in " + computerVM.ComputerName);
                
            }
            
            return View(computerVM);
        }

        //
        // GET: Computers/Edit/5
        //
        public ActionResult Edit(int? id)
        {
            ComputerVM computerVM = new ComputerVM();
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                if (computer == null)
                {
                    computerVM.Message.Fill("Computer - Bewerken", computerVM.Message.Error, "*** ERROR *** ComputerID " + id.ToString() + " staat niet in de database.");
                }
                else {                    
                    computerVM.Fill(computer);
                    computerVM.Message.Fill("Computer - Bewerken", computerVM.Message.Info, "Voer wijzigingen in en klik op OPSLAAN");
                    
                }
                
                return View(computerVM);
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Computer Bewerken (GET)";
                string l = computerVM.Message.Error;
                return RedirectToAction("Index", "Computers", new { Message = m, MsgLevel = l });
            }

        }

        // POST: Computers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComputerID,ComputerName,ComputerPurchaseDate,OS")] ComputerVM computerVM)
        {
            if (ModelState.IsValid)
            {
                Computer computer = new Computer();
                computer.Fill(computerVM);
                db.Entry(computer).State = EntityState.Modified;
                db.SaveChanges();
                
                string m = "Computer " + computer.ComputerName + " is aangepast";
                string l = computerVM.Message.Info;
                return RedirectToAction("Index", "Computers", new { Message = m, MsgLevel = l });
            }
            else
            {
                computerVM.Message.Fill("Computer - Bewerken",
                        computerVM.Message.Error, "Model ERROR in " + computerVM.ComputerName);
                
                return View(computerVM);
            }

        }

        //
        // GET: Computers/Delete/5
        //
        public ActionResult Delete(int? id)
        {
            ComputerVM computerVM = new ComputerVM() ;
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                if (computer == null)
                {
                    computerVM.Message.Fill("Computer - Verwijderen", computerVM.Message.Error, "*** ERROR *** ComputerID " + id.ToString() + " staat niet in de database.");

                }
                else
                {
                    computerVM.Message.Fill("Computer - Verwijderen", computerVM.Message.Info, "Klik op VERWIJDEREN om deze computer te verwijderen");
                }                             
                computerVM.Fill(computer);
                return View(computerVM);                
            }
            else
            {
                ContractErrorOccurred = false;
                string m = "Contract error bij Ccomputer Verwijderen (GET)";
                string l = computerVM.Message.Error;
                return RedirectToAction("Index", "Computers", new { Message = m, MsgLevel = l });
            }

        }
        

        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SympaMessage msg = new SympaMessage();
            string m = "";
            string l = "";
            Contract.Requires(id > 0);
            Contract.ContractFailed += (Contract_ContractFailed);
            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                db.Computers.Remove(computer);
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException exc)
                {
                    ComputerVM computerVM = new ComputerVM();
                    computerVM.Message.Fill("Computer - Verwijderen", computerVM.Message.Warning, "Computer " + computer.ComputerName + " kan niet worden verwijderd. Verwijder eerst alle Installaties, Licenties en Services *** (" + exc.Message + ")");
                    computerVM.Fill(computer);
                    return View(computerVM);
                }

                m = "Computer " + computer.ComputerName + " is verwijderd.";
                l = msg.Info;
                
            }
            else
            {
                ContractErrorOccurred = false;
                m = "Contract error bij Computer Verwijderen (POST)";
                l = msg.Error;
                
            }
            return RedirectToAction("Index", "Computers", new { Message = m, MsgLevel = l });

        }

        public ActionResult Report01()
        {
            ComputerIndex index = new ComputerIndex();
            index.Message.Title = "Rapport - Computers zonder installaties";
            
            index.Message.Tekst = "Overzicht van computers waar niets op geïnstalleerd staat";
            index.Message.Level = index.Message.Info;

            var query = from c in db.Computers
                        where !(from i in db.Installations
                                select i.ComputerID)
                               .Contains(c.ComputerID)
                        orderby c.ComputerName
                        select new ComputerVM
                        {
                            ComputerID = c.ComputerID,  
                            ComputerName = c.ComputerName,
                            ComputerPurchaseDate = c.ComputerPurchaseDate,
                            OS = c.OS
                        };

            index.ComputerLijst = query.ToList();
            
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
