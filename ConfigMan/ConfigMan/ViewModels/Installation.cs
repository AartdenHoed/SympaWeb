﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMan.ViewModels;

namespace ConfigMan
{
    public partial class Installation
    {
        public void Fill(InstallationVM installationVM)
        {
            this.ComputerID = installationVM.ComputerID;
            this.ComponentID = installationVM.ComponentID;
            this.Release = installationVM.Release.Trim();
            this.Location = installationVM.Location.Trim();
            this.InstallDate = installationVM.InstallDate;
            this.MeasuredDateTime = installationVM.MeasuredDateTime;
            this.StartDateTime = installationVM.StartDateTime;
            this.EndDateTime = installationVM.EndDateTime;
            this.Count = installationVM.Count;
         
        }
    }
}