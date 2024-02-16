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
            this.ComputerName = computerVM.ComputerName.TrimEnd();
            this.ComputerPurchaseDate = computerVM.ComputerPurchaseDate;
            if (computerVM.OS == null)
            {
                this.OS = computerVM.OS;
            }
            else
            {
                this.OS = computerVM.OS.TrimEnd();
            }
            this.ComputerID = computerVM.ComputerID;
        }
    }
}