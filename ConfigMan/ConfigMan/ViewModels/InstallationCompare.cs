using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;


namespace ConfigMan.ViewModels
{
    public class InstallationCompare
    {
        public SympaMessage Message = new SympaMessage();

        public List <ComputerVM> ComputerLijst = new List<ComputerVM>();
        

        public List <InstallationReportLine> InstallationReport = new List<InstallationReportLine>();

       
    }

    public class InstallationReportLine
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public string ComponentNameTemplate { get; set; }

        [DisplayName("Component Naam (Template)")]
        public string ComponentNameTemplateV
        { get { return ComponentNameTemplate.Replace("\\d+", "#").Replace("\\", ""); } }

        public string VendorName { get; set; }

        public List<string> Indicator = new List<string>(); // As many entries as computers

    }

    public class InstallationData
    {

        public List<InstallationRelease> ComponentLijst = new List<InstallationRelease>();

        public List<List<InstallationRelease>> InstallationReleaseOverview = new List<List<InstallationRelease>>(); // for each computer 1 list

    }

    public class InstallationRelease
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public string ComponentNameTemplate { get; set; }        
        public string Release { get; set; }
        public string VendorName { get; set; }  
        public Boolean Matched { get; set; }
    }

    
   

   

   
       
}