using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class ServiceIndex
    {
        [DisplayName("Aantal Services")]
        public int Aantal { get { return this.ServiceLijst.Count; } }
        
        public SympaMessage Message = new SympaMessage();

        public ServiceFilter FilterData = new ServiceFilter();

        public List<ServiceVM> ServiceLijst = new List<ServiceVM>();
    }
}
