using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;
using Microsoft.Ajax.Utilities;


namespace ConfigMan.ViewModels
{
    public class ComponentIndex
    {
        public bool Filter { get; set; }
        public string ComponentFilter { get; set; }
        public string VendorFilter { get; set; }    
        public string AuthFilter { get; set; }  

        public SympaMessage Message = new SympaMessage();

        public List<ComponentVM> ComponentLijst = new List<ComponentVM>();
    }
        
       
}