using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Stackpole.ViewModels
{
    public class ScrapModal {
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
    }

    public class ScrapDetailModal {
        public int id { get; set; }

        public int scrapId { get; set; }
        [DisplayName("Reason")]
        public int reasonId { get; set; }
        [DisplayName("Machine")]
        public int machineId { get; set; }

        [Required]
        [DisplayName("Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int quantity { get; set; }

        [Required]
        [DisplayName("Weight")]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public Nullable<double> weight { get; set; }

        [DisplayName("Value")]
        public Nullable<double> cost { get; set; }

        [DisplayName("Employee")]
        public Nullable<short> employeeNumber { get; set; }
    }

    public class ScrapBasicInfoModal
    {
        public ScrapModal scrapModal { get; set; }
        public ScrapDetailModal scrapDetailModal { get; set; }
        public int operationIdInt { get; set; }

        public IList<SelectListItem> Machines { get; set; }
        public IList<SelectListItem> Reasons { get; set; }
    }
}