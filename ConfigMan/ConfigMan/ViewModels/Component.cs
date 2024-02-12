using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMan.ViewModels;
using Microsoft.Ajax.Utilities;

namespace ConfigMan
{
    public partial class Component
    {
        public void Fill(ComponentVM componentVM)
        {
            this.ComponentName = componentVM.ComponentName.Trim();
            this.ComponentID = componentVM.ComponentID;
            
            if ((componentVM.SelectedVendorIDstring != null) && (componentVM.SelectedVendorIDstring != "")) {
                this.VendorID = Int32.Parse(componentVM.SelectedVendorIDstring);
            }
            else
            {
                this.VendorID = componentVM.VendorID;
            }
        }
    }
}