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
        public string ComponentNameTemplateV 
        { get { return ComponentNameTemplate.Replace("\\.", ".").Replace("\\d+", "#").Replace("\\(", "(").Replace("\\)", ")"); } }
        public void Fill(ComponentVM componentVM)
        {
            this.ComponentNameTemplate = componentVM.ComponentNameTemplate.TrimEnd();
            this.Authorized = componentVM.Authorized;
            
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