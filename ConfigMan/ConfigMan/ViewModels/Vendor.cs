using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMan.ViewModels;

namespace ConfigMan
{
    public partial class Vendor
    {
        public void Fill(VendorVM vendorVM)
        {
            this.VendorName = vendorVM.VendorName.Trim();
            this.VendorGroup = vendorVM.VendorGroup.Trim();
            
            this.VendorID = vendorVM.VendorID;
        }
    }
}