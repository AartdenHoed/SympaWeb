using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMan.ViewModels;
using Microsoft.Ajax.Utilities;

namespace ConfigMan
{
    public partial class Service
    {
        public void Fill(ServiceVM serviceVM)
        {
            this.PSComputerName = serviceVM.PSComputerName;  
            this.SystemName = serviceVM.SystemName;
            this.Name = serviceVM.Name;
            this.ComputerID = serviceVM.ComputerID;   
            
            this.Suffix = serviceVM.Suffix;
            this.Caption = serviceVM.Caption;
            this.DisplayName = serviceVM.DisplayName;
            this.PathName = serviceVM.PathName;
            this.ServiceType = serviceVM.ServiceType;
            this.StartMode = serviceVM.StartMode;
            this.Started = serviceVM.Started;
            this.State = serviceVM.State;
            this.Status = serviceVM.Status;
            this.ExitCode = serviceVM.ExitCode;  
            this.Description = serviceVM.Description;
            this.Software = serviceVM.Software;
            this.DirName = serviceVM.DirName;
            this.DirectoryTemplate = serviceVM.DirectoryTemplate;
            this.ProgramName = serviceVM.ProgramName;
            this.Parameter = serviceVM.Parameter;   
            this.ChangeState = serviceVM.ChangeState;
            this.StartDate = serviceVM.StartDate;
            this.CheckDate = serviceVM.CheckDate;
            this.OldChangeState = serviceVM.OldChangeState;
            this.OldDirName = serviceVM.OldDirName;
            this.OldProgramName = serviceVM.OldProgramName;
            this.OldParameter = serviceVM.OldParameter;

            if (string.IsNullOrEmpty(serviceVM.SelectedComponentIDstring))
            {
                this.ComponentID = serviceVM.ComponentID;
            }
            else
            {
                this.ComponentID = Int32.Parse(serviceVM.SelectedComponentIDstring);
            }
           
            
        }
    }
}