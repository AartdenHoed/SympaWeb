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
        public string Filterstr { get; set; }
        public bool Filter { get { return (!((this.Filterstr == "N") || (this.Filterstr is null))); } }
                   
        public string ComponentFilter { get; set; }
        public string VendorFilter { get; set; }    
        public string AuthFilter { get; set; }  

        public SympaMessage Message = new SympaMessage();

        public List<ComponentVM> ComponentLijst = new List<ComponentVM>();
    }
        
       
}