using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;


namespace ConfigMan.ViewModels
{
    public class ComponentVM
    {
        public SympaMessage Message = new SympaMessage();
        
        public List<VendorVM> VendorLijst = new List<VendorVM>();
        public List<InstallationVM> InstallationLijst = new List<InstallationVM>();
        [DisplayName("Aantal installaties")]
        public int Aantal { get { return this.InstallationLijst.Count; } }

        public string SelectedVendorIDstring { get; set; }

        public int ComponentID { get; set; }
        public string ComponentIDstring { get { return ComponentID.ToString(); } }
        public int VendorID { get; set; }
        
        [DisplayName("Unieke Vendor Naam")]
        public string VendorName { get; set; }

        [DisplayName("Vendor Group")]
        public string VendorGroup { get; set; }

        private string _ComponentNameTemplate = "";

        [Required(ErrorMessage = "Componentnaam is een verplicht veld")]
        [DisplayName("Unieke Component Naam")]
        [MaxLength(120, ErrorMessage = "Maximaal 120 characters")]       
        public string ComponentNameTemplate { get { return _ComponentNameTemplate.TrimEnd(); }  set { _ComponentNameTemplate = value.TrimEnd(); } }

        public string ComponentNameTemplateV 
        { get { return ComponentNameTemplate.Replace("\\.", ".").Replace("\\d+", "#").Replace("\\(", "(").Replace("\\)", ")"); }}

        [Required(ErrorMessage = "Autorisatie (Y/N) is een verplicht veld")]
        [DisplayName("Component geautoriseerd (Y/N)")]
        [MaxLength(1, ErrorMessage = "Lengte = 1 (Y of N)")]
        [StringRange(AllowableValues = new[] { "Y", "N"}, ErrorMessage = "Specificeer Y (geautoriseerd) of N (niet geautoriseerd)")]
        public string Authorized {  get; set; }

        public SympaFilter FilterData = new SympaFilter();

        public void Fill(Component component)
        {
            this.ComponentNameTemplate = component.ComponentNameTemplate.TrimEnd();
            this.ComponentID = component.ComponentID;
            this.VendorID = component.VendorID;
            this.Authorized = component.Authorized;

        }

        public class StringRangeAttribute : ValidationAttribute
        {
            public string[] AllowableValues { get; set; }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {

                if (AllowableValues?.Contains(value?.ToString()) == true)
                {
                    return ValidationResult.Success;
                }

                var msg = $"Enter een geldige waarde: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
                return new ValidationResult(msg);
            }
        }
    }
}