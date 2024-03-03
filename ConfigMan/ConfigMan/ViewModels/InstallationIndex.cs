using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;


namespace ConfigMan.ViewModels
{
    public class InstallationIndex
    {
        public SympaMessage Message = new SympaMessage();

        public List<InstallationVM> InstallationLijst = new List<InstallationVM>();
    }
        
       
}