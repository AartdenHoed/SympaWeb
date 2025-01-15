using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class ServiceVM
    {
        public SympaMessage Message = new SympaMessage();
        public ServiceFilter FilterData = new ServiceFilter();
        public List<ComponentVM> ComponentLijst = new List<ComponentVM>();
        public string SelectedComponentIDstring { get; set; }

        [Required(ErrorMessage = "PSComputerName is een verplicht veld")]
        [DisplayName("PSComputerName")]
        [MaxLength(16, ErrorMessage = "Maximale lengte = 16")] 
        public string PSComputerName { get; set; }

        [Required(ErrorMessage = "SystemName is een verplicht veld")]
        [DisplayName("SystemName")]
        [MaxLength(16, ErrorMessage = "Maximale lengte = 16")]
        public string SystemName { get; set; }

        private string _Name = "";

        [Required(ErrorMessage = "Service Name is een verplicht veld")]
        [DisplayName("Service Name")]
        [MaxLength(64, ErrorMessage = "Maximale lengte = 64")]
        public string Name { get { return _Name.TrimEnd(); } set { _Name = value.TrimEnd(); } }
        public int ComputerID { get; set; }
        public int ComponentID { get; set; }

        public string ComponentName { get; set; }

        [DisplayName("Service Name Suffix")]
        [MaxLength(8, ErrorMessage = "Maximale lengte = 8")]
        public string Suffix { get; set; }

        [Required(ErrorMessage = "Service Caption is een verplicht veld")]
        [DisplayName("Service Caption")]
        [MaxLength(128, ErrorMessage = "Maximale lengte = 128")]
        public string Caption { get; set; }

        [Required(ErrorMessage = "Service DisplayName is een verplicht veld")]
        [DisplayName("Service DisplayName")]
        [MaxLength(128, ErrorMessage = "Maximale lengte = 128")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Service PathName is een verplicht veld")]
        [DisplayName("Service PathName")]
        [MaxLength(2048, ErrorMessage = "Maximale lengte = 2048")]
        public string PathName { get; set; }

        [Required(ErrorMessage = "Service Type is een verplicht veld")]
        [DisplayName("Service Type")]
        [MaxLength(16, ErrorMessage = "Maximale lengte = 16")]
        public string ServiceType { get; set; }

        [Required(ErrorMessage = "Service StartMode is een verplicht veld")]
        [DisplayName("Service StartMode")]
        [MaxLength(8, ErrorMessage = "Maximale lengte = 8")]
        public string StartMode { get; set; }

        [Required(ErrorMessage = "Service Gestart? is een verplicht veld")]
        [DisplayName("Service Gestart?")]
        [MaxLength(5, ErrorMessage = "Maximale lengte = 5")]
        public string Started { get; set; }

        [Required(ErrorMessage = "Service Start Status is een verplicht veld")]
        [DisplayName("Service Start Status")]
        [MaxLength(8, ErrorMessage = "Maximale lengte = 8")]
        public string State { get; set; }

        [Required(ErrorMessage = "Service Health Status is een verplicht veld")]
        [DisplayName("Service Health Status")]
        [MaxLength(8, ErrorMessage = "Maximale lengte = 8")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Service Exit Code is een verplicht veld")]
        [DisplayName("Service Exit Code")]
        public long ExitCode { get; set; }

        [Required(ErrorMessage = "Service Description is een verplicht veld")]
        [DisplayName("Service Description")]
        [MaxLength(2048, ErrorMessage = "Maximale lengte = 2048")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Service Software Name is een verplicht veld")]
        [DisplayName("Service Software Name")]
        [MaxLength(48, ErrorMessage = "Maximale lengte = 48")]
        public string Software { get; set; }

        [DisplayName("Directory Template (RegEx)")]
        [MaxLength(256, ErrorMessage = "Maximale lengte = 256")]
        public string DirectoryTemplate { get; set; }

        [Required(ErrorMessage = "Service Directory is een verplicht veld")]
        [DisplayName("Service Directory")]
        [MaxLength(256, ErrorMessage = "Maximale lengte = 256")]
        public string DirName { get; set; }

        [Required(ErrorMessage = "Service Executable is een verplicht veld")]
        [DisplayName("Service Executable")]
        [MaxLength(64, ErrorMessage = "Maximale lengte = 64")]
        public string ProgramName { get; set; }

        [DisplayName("Service Parameter")]
        [MaxLength(2048, ErrorMessage = "Maximale lengte = 2048")]
        public string Parameter { get; set; }

        [Required(ErrorMessage = "Service Change Status is een verplicht veld")]
        [DisplayName("Service Change Status")]
        [MaxLength(8, ErrorMessage = "Maximale lengte = 8")]
        public string ChangeState { get; set; }

        [Required(ErrorMessage = "Datum/Tijd eerste waarneming is een verplicht veld")]
        [DisplayName("Datum/tijd eerste waarneming")]
        public System.DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Datum/Tijd laatste waarneming is een verplicht veld")]
        [DisplayName("Datum/tijd laatste waarneming")]
        public System.DateTime CheckDate { get; set; }

        [DisplayName("Vorige Service Change Status")]
        [MaxLength(8, ErrorMessage = "Maximale lengte = 8")]
        public string OldChangeState { get; set; }

        [DisplayName("Vorige Service Directory")]
        [MaxLength(256, ErrorMessage = "Maximale lengte = 256")]
        public string OldDirName { get; set; }

        [DisplayName("Vorige Service Executable")]
        [MaxLength(64, ErrorMessage = "Maximale lengte = 64")]
        public string OldProgramName { get; set; }

        [DisplayName("Vorige Service Parameter")]
        [MaxLength(2048, ErrorMessage = "Maximale lengte = 2048")]
        public string OldParameter { get; set; }

    }
}