using JobBoardv3.DATA.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobBoardv3.UI.MVC.Models
{
    public class ApplicationsViewModel
    {
        public IEnumerable<Application> Applications { get; set; }

        public IEnumerable<int> ApplicationIds { get; set; }


    }
}