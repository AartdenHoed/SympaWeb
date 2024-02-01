using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMan.ViewModels;

namespace ConfigMan
{
    public partial class Computer
    {
        public void Fill(ComputerVM computerVM)
        {
            this.ComputerName = computerVM.ComputerName.Trim();
            this.ComputerPurchaseDate = computerVM.ComputerPurchaseDate;
            this.OS = computerVM.OS.Trim();
            this.ComputerID = computerVM.ComputerID;
        }
    }
}