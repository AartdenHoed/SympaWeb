using System;
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
            this.ComponentName = installationVM.ComponentName;
            if (installationVM.Release == null)
            {
                this.Release = installationVM.Release;
            }
            else
            {
                this.Release = installationVM.Release.TrimEnd();
            }
            if (installationVM.Location == null)
            {
                this.Location = installationVM.Location;
            }
            else
            {
                this.Location = installationVM.Location.TrimEnd();
            }
            this.InstallDate = installationVM.InstallDate;
            this.MeasuredDateTime = installationVM.MeasuredDateTime;
            this.StartDateTime = installationVM.StartDateTime;
            this.EndDateTime = installationVM.EndDateTime;
            this.Count = installationVM.Count;
         
        }
    }
}