using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class ServiceFilter
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

        private string P_Subsetstr { get; set; }
        public string Subsetstr
        {
            get { return this.P_Subsetstr; }
            set
            {
                if (string.IsNullOrEmpty(value)) { this.P_Subsetstr = "A"; }
                else { this.P_Subsetstr = value; }
            }
        }
        public void Fill()
        {
            int i = 1;
        }

    }
}