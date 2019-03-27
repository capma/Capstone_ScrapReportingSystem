using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stackpole.Models;

namespace Stackpole.ViewModels
{
    public class PlantViewModel : IObjectWithState
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<int> area { get; set; }

        public string MessageToClient { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}