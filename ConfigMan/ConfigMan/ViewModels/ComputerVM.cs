﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels
{
    public class ComputerVM
    {
        public SympaMessage Message = new SympaMessage();
        public int ComputerID { get; set; }
        public string ComputerIDstring { get { return ComputerID.ToString(); } }

        [Required(ErrorMessage = "Computernaam is een verplicht veld")]
        [DisplayName("Unieke Computer Naam")]
        [MaxLength(32, ErrorMessage = "Maximaal 32 characters")]
        public string ComputerName { get; set; }

        [DisplayName("Aanschafdatum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> ComputerPurchaseDate { get; set; }

        [DisplayName("Operating Systeem")]
        [MaxLength(24, ErrorMessage = "Maximaal 24 characters")]
        public string OS { get; set; }


        public void Fill(Computer computer)
        {
            this.ComputerName = computer.ComputerName.TrimEnd();
            this.ComputerPurchaseDate = computer.ComputerPurchaseDate;
            if (computer.OS == null)
            {
                this.OS = computer.OS;
            }
            else
            {
                this.OS = computer.OS.TrimEnd();
            }
            this.ComputerID = computer.ComputerID;

        }
    }
}