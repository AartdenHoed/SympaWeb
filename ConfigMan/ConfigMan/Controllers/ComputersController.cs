﻿using System;
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

namespace ConfigMan.Controllers { 

    [LogActionFilter]
    [HandleError]
   
    public class ComputersController : Controller
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
        // GET: Computers
        //
        public ActionResult Index()
        {
            if (ContractErrorOccurred) {
                ContractErrorOccurred = false;
                ViewBag.SympaMsg = ErrorMessage;
                ErrorMessage = " ";
            }
            else {
                ViewBag.SympaMsg = TempData["SympaMsg"];
            }
            List<Computer> complist = db.Computers.OrderBy(x => x.ComputerName).ToList();
            List<ComputerVM> vmlist = new List<ComputerVM>();
            foreach (Computer c in complist) {
                ComputerVM VM = new ComputerVM();
                VM.Fill(c);
                vmlist.Add(VM); 
            }
            return View(vmlist);

        }

        //
        // GET: Computers/Details/5
        //
        public ActionResult Details(int? id)
        {
           
            Contract.ContractFailed += (Contract_ContractFailed);
            Contract.Requires(id > 0, "Contract Failed!");

            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                if (computer == null) {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else {
                    ComputerVM computerVM = new ComputerVM();
                    computerVM.Fill(computer);
                    return View(computerVM);
                }
            }
            return RedirectToAction("Index");
        }

        private void Contract_ContractFailed(object sender, ContractFailedEventArgs e) {
            ErrorMessage = "*** ERROR *** Selected computer id is invalid.";
            ContractErrorOccurred = true;
            e.SetHandled();
            return;
        }

        //
        // GET: Computers/Create
        //
        public ActionResult Create()
        {    
            return View();
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
                try {
                    db.SaveChanges();
                    TempData["SympaMsg"] = "Computer " + computer.ComputerName + " succesfully added."; 
                }
                catch (DbUpdateException) {
                    addfailed = true;
                    ViewBag.SympaMsg = "Computer " + computer.ComputerName + " already exists, use update function.";
                }
                if (addfailed) { return View(); }
                else { return RedirectToAction("Index"); }
            }

            return View(computerVM);
        }

        //
        // GET: Computers/Edit/5
        //
        public ActionResult Edit(int? id)
        {
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                if (computer == null) {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else {
                    ComputerVM computerVM = new ComputerVM();
                    computerVM.Fill(computer);
                    return View(computerVM);
                }
            }
            return RedirectToAction("Index");

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
                TempData["SympaMsg"] = "Computer " + computer.ComputerName + " succesfully updated.";
                return RedirectToAction("Index");
            }
            else {
                TempData["SympaMsg"] = "Update filed for computer " + computerVM.ComputerName ;
                return View(computerVM);
            }
            
        }

        //
        // GET: Computers/Delete/5
        //
        public ActionResult Delete(int? id)
        {
            Contract.Requires((id != null) && (id > 0));
            Contract.ContractFailed += (Contract_ContractFailed);

            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                if (computer == null) {
                    TempData["SympaMsg"] = "*** ERROR *** ID " + id.ToString() + " not found in database.";
                }
                else {
                    ComputerVM computerVM = new ComputerVM();
                    computerVM.Fill(computer);
                    return View(computerVM);
                }
            }
            return RedirectToAction("Index");

        }
        

        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contract.Requires(id > 0);
            Contract.ContractFailed += (Contract_ContractFailed);
            if (!ContractErrorOccurred) {
                Computer computer = db.Computers.Find(id);
                db.Computers.Remove(computer);
                db.SaveChanges();
                TempData["SympaMsg"] = "Computer "+ computer.ComputerName + " succesfully deleted.";
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

            List<ComputerVM> complist = query.ToList();
           
            return View(complist);

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
