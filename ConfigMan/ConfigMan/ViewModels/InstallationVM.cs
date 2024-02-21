using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class InstallationVM
    {
        public List<ComputerVM> ComputerLijst = new List<ComputerVM>();
        public string SelectedComputerIDstring { get; set; }
        public List<ComponentVM> ComponentLijst = new List<ComponentVM>();
        public string SelectedComponentIDstring { get; set; }
        public int ComputerID { get; set; }
        public int ComponentID { get; set; }

        public int VendorID { get; set; }
        public string VendorName {  get; set; } 

        [Required(ErrorMessage = "Release is een verplicht veld")]
        [DisplayName("Release aanduiding")]
        [MaxLength(44, ErrorMessage = "Maximaal 44 characters")]
        public string Release { get; set; }

        [DisplayName("Installatie directory")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]
        public string Location { get; set; }

        [DisplayName("Installatiedatum op het systeem")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> InstallDate { get; set; }

        [Required(ErrorMessage = "Datum/tijd van meting is een verplicht veld")]
        [DisplayName("Datum/tijd van meting")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]

        public System.DateTime MeasuredDateTime { get; set; }

        [Required(ErrorMessage = "Datum/tijd van start van deze installatie is een verplicht veld")]
        [DisplayName("Installatie Start")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]

        public System.DateTime StartDateTime { get; set; }

        [DisplayName("Installatie Einde")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public Nullable<System.DateTime> EndDateTime { get; set; }

        [DisplayName("Aantal installatie instances")]
        [Required(ErrorMessage = "Installatie count moet minimaal 1 zijn en maximaal 999")]
        [Range(1, 999, ErrorMessage = "Minimaal 1, maximaal 999")]
        public int Count { get; set; }

        public string ComponentName { get; set;}
        public string ComputerName { get; set;}

        public void Fill(Installation installation)
        {
            this.ComputerID = installation.ComputerID;
            this.ComponentID = installation.ComponentID;
            if (installation.Release == null) {
                this.Release = installation.Release;
            }
            else { 
                this.Release = installation.Release.TrimEnd();
            }
            if (installation.Location == null) {
                this.Location = installation.Location;
            }
            else { 
                this.Location = installation.Location.TrimEnd();
            }
            this.InstallDate = installation.InstallDate;    
            this.MeasuredDateTime = installation.MeasuredDateTime;
            this.StartDateTime = installation.StartDateTime;
            this.EndDateTime = installation.EndDateTime;
            this.Count = installation.Count;
        }
    }
}