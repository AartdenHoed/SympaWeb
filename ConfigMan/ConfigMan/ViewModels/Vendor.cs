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
            this.VendorName = vendorVM.VendorName.TrimEnd();
            if (vendorVM.VendorName == null)
            {
                this.VendorGroup = vendorVM.VendorGroup;
            }
            else
            {
                this.VendorGroup = vendorVM.VendorGroup.TrimEnd();
            }
            this.VendorID = vendorVM.VendorID;
        }
    }
}