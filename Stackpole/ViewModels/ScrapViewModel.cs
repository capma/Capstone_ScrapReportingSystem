using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Stackpole.Models;

namespace Stackpole.ViewModels
{
    public class ScrapViewModel : IObjectWithState
    {
        public ScrapViewModel()
        {
            this.ScrapDetails = new List<ScrapDetailViewModel>();
            this.ScrapDetailsToDelete = new List<int>();
            this.machineId = 0;
            this.unitCost = 0.0;
            this.unitWeight = 0.0;
        }

        public int id { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string partId { get; set; }
        public string departmentId { get; set; }
        public string plantId { get; set; }
        public Nullable<int> machineId { get; set; }
        public string operationId { get; set; }
        public Nullable<double> unitCost { get; set; }
        public Nullable<double> unitWeight { get; set; }
        public string cancelled { get; set; }

        public ObjectState ObjectState { get; set; }
        public List<ScrapDetailViewModel> ScrapDetails { get; set; }
        public string MessageToClient { get; set; }
        public List<int> ScrapDetailsToDelete { get; set; }
    }
}