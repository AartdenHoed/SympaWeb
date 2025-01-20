using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace ConfigMan.ViewModels
{
    
    public class SympaFilter
    {
        public string Filterstr { get; set; }
        public bool Filter { get { return (!((this.Filterstr == "N") || (this.Filterstr is null))); } }

        [DisplayName("Toon alle componenten")]
        public bool ShowAll { get { return (this.Subsetstr == "A"); } }

        [DisplayName("Toon componenten met actieve installaties")]
        public bool ShowActive { get { return (this.Subsetstr == "Y"); } }

        [DisplayName("Toon componenten zonder actieve installaties")]
        public bool ShowInactive { get { return (this.Subsetstr == "N"); } }

        [DisplayName("Toon componenten zonder installaties")]
        public bool ShowEmpty { get { return (this.Subsetstr == "E"); } }

        private string P_Subsetstr { get; set;  }
        public string Subsetstr
        {
            get { return this.P_Subsetstr; }
            set
            {
                if (string.IsNullOrEmpty(value)) { this.P_Subsetstr = "A"; }
                else { this.P_Subsetstr = value; }
            }
        }

        public string ComponentFilter { get; set; }
        public string VendorFilter { get; set; }
        public string AuthFilter { get; set; }
        public string OriginFilter { get; set; }

        public void Fill(string filterstr, string subsetstr, string componentFilter, string authFilter, string originFilter, string vendorFilter)
        {
            this.Filterstr = filterstr;
            this.Subsetstr = subsetstr;
            this.ComponentFilter = componentFilter;
            this.AuthFilter = authFilter;
            this.OriginFilter = originFilter;
            this.VendorFilter = vendorFilter;
        }
    }

}
