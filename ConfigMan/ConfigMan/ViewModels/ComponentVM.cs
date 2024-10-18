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

        private string _ComponentNameTemplate = "";

        [Required(ErrorMessage = "Componentnaam is een verplicht veld")]
        [DisplayName("Unieke Component Naam")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]       
        public string ComponentNameTemplate { get { return _ComponentNameTemplate.TrimEnd(); }  set { _ComponentNameTemplate = value.TrimEnd(); } }

        public string ComponentNameTemplateV 
        { get { return ComponentNameTemplate.Replace("\\.", ".").Replace("\\d+", "#").Replace("\\(", "(").Replace("\\)", ")"); }}

        public string Authorized {  get; set; } 
       
        public void Fill(Component component)
        {
            this.ComponentNameTemplate = component.ComponentNameTemplate.TrimEnd();
            this.ComponentID = component.ComponentID;
            this.VendorID = component.VendorID;
            this.Authorized = component.Authorized;

        }
    }
}