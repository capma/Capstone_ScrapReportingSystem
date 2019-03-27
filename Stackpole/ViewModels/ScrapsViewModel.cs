using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stackpole.Models;

namespace Stackpole.ViewModels
{
    public class ScrapsViewModel
    {
        public Scrap scrap { get; set; }
        public List<ScrapDetail> scrapDetails { get; set; }

        public int operationIdInt { get; set; }

        // constructor
        public ScrapsViewModel()
        {
            scrapDetails = new List<ScrapDetail>();
        }
    }
}