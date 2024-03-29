﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class VendorVM
    {
        public SympaMessage Message = new SympaMessage();
        public int VendorID { get; set; }
        public string VendorIDstring { get { return VendorID.ToString();}}

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
            this.VendorName = vendor.VendorName.TrimEnd();
            if (vendor.VendorGroup == null)
            {
                this.VendorGroup = vendor.VendorGroup;
            }
            else { 
                this.VendorGroup = vendor.VendorGroup.TrimEnd();
            }
            this.VendorID = vendor.VendorID;

        }
    }
}