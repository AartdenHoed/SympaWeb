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
        [DisplayName("Aantal componenten")]
        public int Aantal { get { return this.ComponentLijst.Count; } }
        public SympaFilter FilterData = new SympaFilter();

        public SympaMessage Message = new SympaMessage();

        public List<ComponentVM> ComponentLijst = new List<ComponentVM>();
    }
        
       
}