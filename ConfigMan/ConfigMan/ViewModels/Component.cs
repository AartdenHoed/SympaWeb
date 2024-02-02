using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMan.ViewModels;

namespace ConfigMan
{
    public partial class Component
    {
        public void Fill(ComponentVM componentVM)
        {
            this.ComponentName = componentVM.ComponentName.Trim();
            this.ComponentID = componentVM.ComponentID;
            
            this.VendorID = componentVM.VendorID;
        }
    }
}