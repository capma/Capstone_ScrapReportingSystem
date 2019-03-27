using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Stackpole.Models;

namespace Stackpole.ViewModels
{
    public class ScrapDetailViewModel : IObjectWithState
    {
        public int id { get; set; }
        public int scrapId { get; set; }
        public int reasonId { get; set; }
        public int machineId { get; set; }
        public int quantity { get; set; }
        public Nullable<double> weight { get; set; }
        public Nullable<double> cost { get; set; }
        public Nullable<short> employeeNumber { get; set; }


        public ObjectState ObjectState { get; set; }
    }
}