using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.ServiceModel.Channels;


namespace ConfigMan.ViewModels
{
    public class ComputerIndex
    {
        public SympaMessage Message = new SympaMessage();

        public List<ComputerVM> ComputerLijst = new List<ComputerVM>();
    }
        
       
}