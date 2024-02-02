using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class VendorVM
    {
        public int VendorID { get; set; }

        [Required(ErrorMessage = "Vendornaam is een verplicht veld")]
        [DisplayName("Unieke Vendor Naam")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "Vendorgroup is een verplicht veld")]
        [DisplayName("Vendor Group")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]
        public string VendorGroup { get; set; }


        public void Fill(Vendor vendor)
        {
            this.VendorName = vendor.VendorName.Trim();
            this.VendorGroup = vendor.VendorGroup.Trim();
            this.VendorID = vendor.VendorID;

        }
    }
}