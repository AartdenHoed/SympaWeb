using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;


namespace ConfigMan.ViewModels
{
    public class ComponentVM
    {
        public SympaMessage Message = new SympaMessage();
        
        public List<VendorVM> VendorLijst = new List<VendorVM>();
        
        public string SelectedVendorIDstring { get; set; }

        public int ComponentID { get; set; }
        public string ComponentIDstring { get { return ComponentID.ToString(); } }
        public int VendorID { get; set; }
        
        [DisplayName("Unieke Vendor Naam")]
        public string VendorName { get; set; }

        private string _ComponentName = "";

        [Required(ErrorMessage = "Componentnaam is een verplicht veld")]
        [DisplayName("Unieke Component Naam")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]       
        public string ComponentName { get { return _ComponentName.TrimEnd(); }  set { _ComponentName = value.TrimEnd(); } }

       
        public void Fill(Component component)
        {
            this.ComponentName = component.ComponentName.TrimEnd();
            this.ComponentID = component.ComponentID;
            this.VendorID = component.VendorID;

        }
    }
}