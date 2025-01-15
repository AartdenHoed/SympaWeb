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

        [DisplayName("Toon alle Services")]
        public bool ShowAll { get { return (this.Subsetstr == "A"); } }

        [DisplayName("Toon services zonder installaties")]
        public bool ShowGhosts { get { return (this.Subsetstr == "G"); } }

        public string SysteemFilter { get; set; }

        public string ServiceNaamFilter { get; set; }

        public string ChangeStateFilter { get; set; }

        public string DirectoryFilter { get; set; }
        public string TemplateFilter { get; set; }

        public string ComponentFilter { get; set; }

        public string ProgramFilter { get; set; }




        public void Fill(string filterstr, string subsetstr, string systeemfilter, string servicenaamfilter, string changestatefilter,
                                           string directoryfilter, string templatefilter, string componentfilter, string programfilter)
        {
            this.Filterstr = filterstr;
            this.Subsetstr = subsetstr;
            this.SysteemFilter = systeemfilter;   
            this.ServiceNaamFilter = servicenaamfilter;
            this.ChangeStateFilter = changestatefilter;
            this.DirectoryFilter = directoryfilter;
            this.TemplateFilter = templatefilter;
            this.ComponentFilter = componentfilter;
            this.ProgramFilter = programfilter;
        }

    }
}