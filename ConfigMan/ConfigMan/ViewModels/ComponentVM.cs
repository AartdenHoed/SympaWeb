using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class ComponentVM
    {
        public int ComponentID { get; set; }
        public int VendorID { get; set; }

        [Required(ErrorMessage = "Componentnaam is een verplicht veld")]
        [DisplayName("Unieke Component Naam")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]
        public string ComponentName { get; set; }

       
        public void Fill(Component component)
        {
            this.ComponentName = component.ComponentName.Trim();
            this.ComponentID = component.ComponentID;
            this.VendorID = component.VendorID;

        }
    }
}