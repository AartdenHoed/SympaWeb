using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;


namespace ConfigMan.ViewModels
{
    public class VendorIndex
    {
        [DisplayName("Aantal vendors")]
        public int Aantal { get { return this.VendorLijst.Count; } }
        public SympaMessage Message = new SympaMessage();

        public List<VendorVM> VendorLijst = new List<VendorVM>();
    }
        
       
}