using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Stackpole.Models;
using Stackpole.ViewModels;

namespace Stackpole.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScrapsController : Controller
    {
        private StackpoleEntities _stackpoleContext;

        public ScrapsController()
        {
            _stackpoleContext = new StackpoleEntities();
        }

        public ActionResult Index()
        {
            return View(_stackpoleContext.Scraps.OrderByDescending(s => s.date).Take(50).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scrap scrap = _stackpoleContext.Scraps.Find(id);
            if (scrap == null)
            {
                return HttpNotFound();
            }

            ScrapViewModel scrapViewModel = ViewModels.Helpers.CreateScrapViewModelFromScrap(scrap);
            scrapViewModel.MessageToClient = "I originated from the viewmodel, rather than the model.";

            return View(scrapViewModel);
        }

        public ActionResult Create()
        {
            ScrapViewModel scrapViewModel = new ScrapViewModel();
            scrapViewModel.date = System.DateTime.Now;
            scrapViewModel.ObjectState = ObjectState.Added;

            getListPlants();

            return View(scrapViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scrap scrap = _stackpoleContext.Scraps.Find(id);
            if (scrap == null)
            {
                return HttpNotFound();
            }

            // Get data for dropdownlists
            getListPlants(false);
            getListDepartments(scrap.plantId);
            getListPartsByPlantName(scrap.departmentId);
            getListOperations(scrap.partId, scrap.departmentId);
            getLisMachinesForEdit(scrap.partId, scrap.departmentId, scrap.operationId);

            ScrapViewModel scrapViewModel = ViewModels.Helpers.CreateScrapViewModelFromScrap(scrap);
            scrapViewModel.MessageToClient = string.Format("The original id of Scrap is {0}.", scrapViewModel.id);
            scrapViewModel.date = System.DateTime.Now;

            if (string.IsNullOrEmpty(scrapViewModel.cancelled))
                scrapViewModel.cancelled = "";

            return View(scrapViewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scrap scrap = _stackpoleContext.Scraps.Find(id);
            if (scrap == null)
            {
                return HttpNotFound();
            }

            ScrapViewModel scrapViewModel = ViewModels.Helpers.CreateScrapViewModelFromScrap(scrap);
            scrapViewModel.MessageToClient = "You are about to permanently delete this scrap.";
            scrapViewModel.ObjectState = ObjectState.Deleted;

            return View(scrapViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stackpoleContext.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult Save(ScrapViewModel scrapViewModel)
        {
            Scrap scrap = ViewModels.Helpers.CreateScrapFromScrapViewModel(scrapViewModel);
            scrap.date = DateTime.UtcNow;

            _stackpoleContext.Scraps.Attach(scrap);

            if (scrap.ObjectState == ObjectState.Deleted)
            {
                foreach (ScrapDetailViewModel scrapDetailViewModel in scrapViewModel.ScrapDetails)
                {
                    ScrapDetail scrapDetail = _stackpoleContext.ScrapDetails.Find(scrapDetailViewModel.id);
                    if (scrapDetail != null)
                        scrapDetail.ObjectState = ObjectState.Deleted;
                }
            }
            else
            {
                foreach (int scrapDetailID in scrapViewModel.ScrapDetailsToDelete)
                            {
                                ScrapDetail scrapDetail = _stackpoleContext.ScrapDetails.Find(scrapDetailID);
                                if (scrapDetail != null)
                                    scrapDetail.ObjectState = ObjectState.Deleted;
                            }
            }

            _stackpoleContext.ApplyStateChanges();
            _stackpoleContext.SaveChanges();

            if (scrap.ObjectState == ObjectState.Deleted)
                return Json(new { newLocation = "/Scraps/Index/" });

            string messageToClient = ViewModels.Helpers.GetMessageToClient(scrapViewModel.ObjectState, scrap.id.ToString());
            scrapViewModel = ViewModels.Helpers.CreateScrapViewModelFromScrap(scrap);
            scrapViewModel.MessageToClient = messageToClient;

            return Json(new { scrapViewModel });
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(scrapViewModel);
            //return Json(new { json });

            // to convert date format from Microsoft JSON format to normal JSON
            //return new CustomJsonResult { Data = new { scrapViewModel } };
        }

        public JsonResult getListPlants(bool isForCreate = true)
        {
            IList<SelectListItem> listPlants = _stackpoleContext.Plants
                                    .OrderBy(m => m.id)
                                    .Select(m => new SelectListItem { Text = m.id + " - "+ m.name, Value = m.name })
                                    .ToList();

            ViewBag.listPlants = listPlants;
            return Json(listPlants, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getListDepartments(string plantId)
        {
            IList<SelectListItem> listDepartments = new List<SelectListItem>();
            switch (plantId)
            {
                case "CSD1":
                    listDepartments = _stackpoleContext.Departments
                                        .Where(v => v.id == 2 || v.id == 3 || v.id == 4 || v.id == 5)
                                        .OrderBy(v => v.id)
                                        .Select(m => new SelectListItem { Text = m.id + " - " + m.name, Value = m.name })
                                        .ToList();
                    break;
                case "CSD2":
                    listDepartments = _stackpoleContext.Departments
                                        .Where(v => v.id == 3 || v.id == 4 || v.id == 5)
                                        .OrderBy(v => v.id)
                                        .Select(m => new SelectListItem { Text = m.id + " - " + m.name, Value = m.name })
                                         .ToList();
                    break;
                case "NCL":
                    listDepartments = _stackpoleContext.Departments
                                        .Where(v => v.id == 3 || v.id == 4 || v.id == 5)
                                        .OrderBy(v => v.id)
                                        .Select(m => new SelectListItem { Text = m.id + " - " + m.name, Value = m.name })
                                        .ToList();
                    break;
            }

            ViewBag.listDepartments = listDepartments;
            return Json(listDepartments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getListParts(string departmentName, int sequence)
        {
            IList<SelectListItem> listParts = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(departmentName) && sequence > 0)
            {
                listParts = _stackpoleContext.qryPartByPlants
                                    .Where(m => m.name == departmentName && m.sequence == sequence)
                                    .OrderBy(m => m.name)
                                    .Select(m => new SelectListItem { Text = m.id.ToString(), Value = m.id.ToString() })
                                    .ToList();
            }

            ViewBag.listParts = listParts;
            return Json(listParts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getListPartsByPlantName(string departmentName)
        {
            IList<SelectListItem> listParts = _stackpoleContext.qryPartByPlants
                                    .Where(m => m.name == departmentName)
                                    .GroupBy(m => new { m.id, m.name })
                                    .Select(m => new SelectListItem { Text = m.Key.id.ToString(), Value = m.Key.id.ToString() })
                                    .OrderBy(m => m.Value)
                                    .ToList();

            ViewBag.listParts = listParts;
            return Json(listParts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getListOperations(string partId, string departmentName)
        {
            IList<SelectListItem> listOperations = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(partId) && !string.IsNullOrEmpty(departmentName))
            {
                listOperations = _stackpoleContext.qryPartDeptOp_GetOperator
                                    .Where(pdo => pdo.PartID == partId && pdo.Department == departmentName)
                                    .GroupBy(pdo => new { pdo.FirstOfOperation, pdo.operationID })
                                    .Select(pdo => new SelectListItem { Text = pdo.Key.operationID + " - " + pdo.Key.FirstOfOperation, Value = pdo.Key.FirstOfOperation })
                                    .ToList();
            }

            ViewBag.listOperations = listOperations;
            return Json(listOperations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getLisMachines(int? operationId, string plantName, int? sequence, string partId)
        {
            IList<SelectListItem> listMachines = new List<SelectListItem>();
            IList<SelectListItem> listScrapReasons = new List<SelectListItem>();

            if (operationId != 0)
            {
                listMachines = _stackpoleContext.qryMachNumbers
                                    .Where(m => m.OperationID == operationId)
                                    .GroupBy(m => new { m.machineID })
                                    .Select(m => new SelectListItem { Text = m.Key.machineID.ToString(), Value = m.Key.machineID.ToString() })
                                    .OrderBy(m => m.Value)
                                    .ToList();

                listScrapReasons = _stackpoleContext.qrySrapReasons
                                    .Where(m => m.operationID == operationId)
                                    .Select(m => new SelectListItem { Text = m.description.ToString(), Value = m.id.ToString() })
                                    .OrderBy(m => m.Text)
                                    .ToList();
            }

            // get cost and weight
            double cost = 0.0;
            double weight = 0.0;
            if (!string.IsNullOrEmpty(plantName) && sequence != 0 && !string.IsNullOrEmpty(partId))
            {
                var getWeightAndCost = _stackpoleContext.qryPartByPlants
                                        .Where(pbp => pbp.PlantName == plantName && pbp.sequence == sequence && pbp.id == partId)
                                        .OrderBy(pbp => pbp.id)
                                        .Select(pbp => new PartCodes { partId = pbp.id, departmentName = pbp.name, weight = pbp.weightlbs, cost = pbp.cost })
                                        .FirstOrDefault();
                if (getWeightAndCost != null)
                {
                    cost = getWeightAndCost.cost;
                    weight = getWeightAndCost.weight;
                }
            }

            ViewBag.listMachines = listMachines;
            ViewBag.listScrapReasons = listScrapReasons;

            return Json(new
            {
                listMachines = listMachines,
                listScrapReasons = listScrapReasons,
                cost = cost,
                weight = weight
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getLisMachinesForEdit(string partId, string departmentName, string operationName)
        {
            IList<SelectListItem> listOperations = new List<SelectListItem>();
            IList<SelectListItem> listMachines = new List<SelectListItem>();
            IList<SelectListItem> listScrapReasons = new List<SelectListItem>();
            int operationId = 0;

            if (!string.IsNullOrEmpty(operationName))
            {
                listOperations = _stackpoleContext.qryPartDeptOp_GetOperator
                                    .Where(pdo => pdo.PartID == partId && pdo.Department == departmentName)
                                    .GroupBy(pdo => new { pdo.FirstOfOperation, pdo.operationID })
                                    .Select(pdo => new SelectListItem { Text = pdo.Key.FirstOfOperation, Value = pdo.Key.operationID.ToString() })
                                    .ToList();

                operationId = int.Parse(listOperations
                                    .Where(m => m.Text == operationName)
                                    .GroupBy(m => new { m.Value })
                                    .Select(m => m.Key.Value )
                                    .FirstOrDefault());

                if (operationId > 0)
                {
                    listMachines = _stackpoleContext.qryMachNumbers
                                    .Where(m => m.OperationID == operationId)
                                    .GroupBy(m => new { m.machineID })
                                    .Select(m => new SelectListItem { Text = m.Key.machineID.ToString(), Value = m.Key.machineID.ToString() })
                                    .OrderBy(m => m.Value)
                                    .ToList();
                    listScrapReasons = _stackpoleContext.qrySrapReasons
                                    .Where(m => m.operationID == operationId)
                                    .Select(m => new SelectListItem { Text = m.description.ToString(), Value = m.id.ToString() })
                                    .OrderBy(m => m.Text)
                                    .ToList();
                }
            }

            ViewBag.listMachines = listMachines;
            ViewBag.listScrapReasons = listScrapReasons;
            return Json(listMachines, JsonRequestBehavior.AllowGet);
        }
    }
}
