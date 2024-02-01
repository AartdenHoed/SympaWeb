using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMan.ViewModels

{
    public class ConfigMSG
    {
        public string Error = "E";
        public string Info = "I";
        public string Warning = "W";
        public string Message { get; set; }
        public string ErrorLevel { get; set; }
        public string Title { get; set; }

        public void SetMessage(string title, string lvl, string msg)
        {
            this.Message = msg;
            this.ErrorLevel = lvl;
            this.Title = title;

        }

    }


}