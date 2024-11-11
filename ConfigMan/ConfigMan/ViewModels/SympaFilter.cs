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

        private bool _subset { get { return (this.ShowAll || this.ShowActive || this.ShowInactive || this.ShowEmpty); } }

        private string _Subsetstr { get; set;  }
        public string Subsetstr
        {
            get { return this._Subsetstr; }
            set
            {
                if (string.IsNullOrEmpty(value)) { this._Subsetstr = "A"; }
                else { this._Subsetstr = value; }
            }
        }

        public string ComponentFilter { get; set; }
        public string VendorFilter { get; set; }
        public string AuthFilter { get; set; }

        public void Fill(string filterstr, string subsetstr, string componentFilter, string authFilter, string vendorFilter)
        {
            this.Filterstr = filterstr;
            this.Subsetstr = subsetstr;
            this.ComponentFilter = componentFilter;
            this.AuthFilter = authFilter;
            this.VendorFilter = vendorFilter;
        }
    }

}
