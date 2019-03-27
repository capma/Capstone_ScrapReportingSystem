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
    public class ScrapDetailsController : Controller
    {
        private StackpoleEntities _stackpoleContext;

        public ScrapDetailsController()
        {
            _stackpoleContext = new StackpoleEntities();
        }

        // GET: ScrapDetails
        public ActionResult Index(string plantName, 
                                    string departmentName, 
                                    string partId, 
                                    string operationName, 
                                    int operationId,
                                    double? weight,
                                    double? cost,
                                    int? scrapId = 0
            )
        {
            ViewBag.departmentName = departmentName;
            ViewBag.plantName = plantName;
            ViewBag.partId = partId;
            ViewBag.operationName = operationName;
            ViewBag.operationId = operationId;
            ViewBag.unitWeight = weight;
            ViewBag.unitCost = cost;

            ViewBag.date = DateTime.Now;
            ViewBag.scrapId = 0;

            // NOTE: once one new scrap is added
            // then we want to display that scrap with its details (scrap details) again
            // everytime the page is refreshed
            if (scrapId != 0)
            {
                ViewBag.scrapId = scrapId;

                // initialize the scrap details
                ScrapsViewModel scrapsViewModel = new ScrapsViewModel();

                scrapsViewModel.scrap = _stackpoleContext.Scraps
                                        .Where(s => s.id == scrapId)
                                        .OrderByDescending(s => s.date)
                                        .FirstOrDefault();

                //var scrapDetails = db.ScrapDetails.Include(s => s.ScrapReason).Include(s => s.Scrap);
                if (scrapsViewModel.scrap != null)
                {
                    scrapsViewModel.scrapDetails = _stackpoleContext.ScrapDetails
                                        .Where(sd => sd.scrapId == scrapsViewModel.scrap.id)
                                        .ToList();
                }

                return View(scrapsViewModel);
            }

            return View();
        }

        public ActionResult IndexForUpdate(string plantName,
                                    string departmentName,
                                    string partId,
                                    string operationName,
                                    int operationId,
                                    double? weight,
                                    double? cost,
                                    int? scrapId = 0)
        {
            ViewBag.departmentName = departmentName;
            ViewBag.plantName = plantName;
            ViewBag.partId = partId;
            ViewBag.operationName = operationName;
            ViewBag.operationId = operationId;
            ViewBag.unitWeight = weight;
            ViewBag.unitCost = cost;
            ViewBag.date = DateTime.Now;
            ViewBag.scrapId = 0;

            ScrapViewModel scrapViewModel = new ScrapViewModel();
            scrapViewModel.plantId = plantName;
            scrapViewModel.departmentId = departmentName;
            scrapViewModel.partId = partId;
            scrapViewModel.operationId = operationName;
            scrapViewModel.unitWeight = weight;
            scrapViewModel.unitCost = cost;
            scrapViewModel.date = System.DateTime.Now;
            scrapViewModel.ObjectState = ObjectState.Added;

            getLisMachinesForEdit(partId, departmentName, operationName);

            return View(scrapViewModel);
        }

        [HttpPost]
        public ActionResult CreateNewScrapModal(ScrapBasicInfoModal scrapBasicInfoModal)
        {
            // initialize data for drop down list in create view
            //getDropDownList(scrapBasicInfoModal.operationIdInt);
            getDropDownListForCreateScrap(scrapBasicInfoModal.operationIdInt, scrapBasicInfoModal);

            // NOTE: avoid error when saving as null value 
            // we should init values as 0 before passing them to the view
            scrapBasicInfoModal.scrapDetailModal.quantity = 0;
            scrapBasicInfoModal.scrapDetailModal.weight = 0;

            return PartialView("_CreateNewScrapModal", scrapBasicInfoModal);
        }

        // POST: ScrapDetails/CreateNewScrapPostModal
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,scrapId,reasonId,machineId,quantity,weight,cost,employeeNumber")] ScrapDetail scrapDetail)
        //public ActionResult CreateNewScrapPost([Bind(Include = "reasonId,machineId,quantity,weight,cost,employeeNumber")] ScrapBasicInfo scrapBasicInfo)
        public ActionResult CreateNewScrapPostModal(ScrapBasicInfoModal scrapBasicInfoModal)
        {
            if (ModelState.IsValid)
            {
                Scrap scrap = new Scrap();

                /* case add first new scrap */
                if (scrapBasicInfoModal.scrapDetailModal.scrapId == 0)
                {
                    scrap.date = scrapBasicInfoModal.scrapModal.date;
                    scrap.machineId = scrapBasicInfoModal.scrapDetailModal.machineId;
                    scrap.operationId = scrapBasicInfoModal.scrapModal.operationId;
                    scrap.partId = scrapBasicInfoModal.scrapModal.partId;
                    scrap.plantId = scrapBasicInfoModal.scrapModal.plantId;
                    scrap.unitCost = scrapBasicInfoModal.scrapModal.unitCost;
                    scrap.unitWeight = scrapBasicInfoModal.scrapModal.unitWeight;
                    scrap.departmentId = scrapBasicInfoModal.scrapModal.departmentId;
                    //scrap.cancelled = false;

                    _stackpoleContext.Scraps.Add(scrap);
                    _stackpoleContext.SaveChanges();

                    scrapBasicInfoModal.scrapModal.id = scrap.id;
                    scrapBasicInfoModal.scrapDetailModal.scrapId = scrap.id;
                }

                ScrapDetail scrapDetail = new ScrapDetail();
                scrapDetail.scrapId = scrapBasicInfoModal.scrapDetailModal.scrapId;
                scrapDetail.reasonId = scrapBasicInfoModal.scrapDetailModal.reasonId;
                scrapDetail.machineId = scrapBasicInfoModal.scrapDetailModal.machineId;
                scrapDetail.quantity = scrapBasicInfoModal.scrapDetailModal.quantity;
                scrapDetail.weight = scrapBasicInfoModal.scrapDetailModal.weight;
                scrapDetail.cost = scrapBasicInfoModal.scrapDetailModal.cost;
                scrapDetail.employeeNumber = scrapBasicInfoModal.scrapDetailModal.employeeNumber;
                
                _stackpoleContext.ScrapDetails.Add(scrapDetail);
                _stackpoleContext.SaveChanges();

                //Index(string plantName, string departmentName, string partId, string operationName, int operationId)
                return RedirectToAction("Index", new
                {
                    plantName = scrapBasicInfoModal.scrapModal.plantId,
                    departmentName = scrapBasicInfoModal.scrapModal.departmentId,
                    partId = scrapBasicInfoModal.scrapModal.partId,
                    operationName = scrapBasicInfoModal.scrapModal.operationId,
                    operationId = scrapBasicInfoModal.operationIdInt,
                    weight = scrapBasicInfoModal.scrapModal.unitWeight,
                    cost= scrapBasicInfoModal.scrapModal.unitCost,
                    scrapId = scrapBasicInfoModal.scrapDetailModal.scrapId
                });
            }
            var errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .Select(x => new { x.Key, x.Value.Errors })
                            .ToArray();

            //ViewBag.scrapID = new SelectList(db.Scraps, "id", "partId", scrapDetail.scrapId);
            //ViewBag.machineId = new SelectList(db.Scraps, "id", "partId", scrapDetail.scrapId);
            //ViewBag.reasonID = new SelectList(db.ScrapReasons, "id", "description", scrapDetail.reasonId);
            //return View(scrapDetail);

            //return View();
            return PartialView("_CreateNewScrapModal", scrapBasicInfoModal);
        }

        [HttpPost]
        public ActionResult CancelEntry(int? scrapId)
        {
            return PartialView("_CancelEntry", scrapId);
        }

        [HttpPost]
        public ActionResult CancelEntrySubmit(int? scrapId)
        {
            Scrap scrap = _stackpoleContext.Scraps.Where(s => s.id == scrapId).FirstOrDefault();
            scrap.cancelled = "c";
            _stackpoleContext.Entry(scrap).State = EntityState.Modified;
            _stackpoleContext.SaveChanges();

            return RedirectToAction("Index", "Operations", new
            {
                plantName = scrap.plantId,
                departmentName = scrap.departmentId,
                partId = scrap.partId,
                weight = scrap.unitWeight,
                cost = scrap.unitCost
            });
        }

        private void getDropDownListForEditScrap(int? operationId, ScrapDetail scrapDetail = null)
        {
            // get data for drop down list of machines
            var getMachNumber = _stackpoleContext.qryMachNumbers
                                    .Where(m => m.OperationID == operationId)
                                    .GroupBy(m => new { m.machineID })
                                    .Select(m => new { m.Key.machineID })
                                    .OrderBy(m => m.machineID)
                                    .ToList();
            
            // get data for drop down list of reason
            var getReason = _stackpoleContext.qrySrapReasons
                                    .Where(m => m.operationID == operationId)
                                    .Select(m => new { m.id, m.description })
                                    .OrderBy(m => m.description)
                                    .ToList();
            if (scrapDetail == null)
            {
                ViewBag.machineId = new SelectList(getMachNumber, "machineId", "machineId", null);
                ViewBag.reasonId = new SelectList(getReason, "id", "description", null);
            }
            else
            {
                ViewBag.machineId = new SelectList(getMachNumber, "machineId", "machineId", scrapDetail.machineId);
                ViewBag.reasonId = new SelectList(getReason, "id", "description", scrapDetail.reasonId);
            }
       }

        private void getDropDownListForCreateScrap(int? operationId, ScrapBasicInfoModal scrapBasicInfoModal)
        {
            // get data for drop down list of machines
            scrapBasicInfoModal.Machines = _stackpoleContext.qryMachNumbers
                                    .Where(m => m.OperationID == operationId)
                                    .GroupBy(m => new { m.machineID })
                                    .Select(m => new SelectListItem { Text = m.Key.machineID.ToString(), Value = m.Key.machineID.ToString() })
                                    .OrderBy(m => m.Value)
                                    .ToList();

            scrapBasicInfoModal.Machines.Insert(0, new SelectListItem { Text = "Choose a machine", Value = "" });

            // get data for drop down list of reason
            scrapBasicInfoModal.Reasons = _stackpoleContext.qrySrapReasons
                                    .Where(m => m.operationID == operationId)
                                    .Select(m => new SelectListItem { Text = m.description.ToString(), Value = m.id.ToString() })
                                    .OrderBy(m => m.Text)
                                    .ToList();
            scrapBasicInfoModal.Reasons.Insert(0, new SelectListItem { Text = "Choose a reason", Value = "" });
        }

        // GET: ScrapDetails/EditModal/5
        public ActionResult EditModal(int? scrapDetailId, int? scrapId, int? operationId)
        {
            if (scrapId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Scrap scrap = _stackpoleContext.Scraps.Find(scrapId);
            if (scrap == null)
            {
                return HttpNotFound();
            }

            ScrapDetail scrapDetail = _stackpoleContext.ScrapDetails.Where(sd => sd.id == scrapDetailId).FirstOrDefault();
            if (scrapDetail == null)
            {
                return HttpNotFound();
            }

            getDropDownListForEditScrap(operationId, scrapDetail);

            ViewBag.departmentName = scrap.departmentId;
            ViewBag.plantName = scrap.plantId;
            ViewBag.partId = scrap.partId;
            ViewBag.operationName = scrap.operationId;
            ViewBag.operationId = operationId;
            ViewBag.unitWeight = scrap.unitWeight;
            ViewBag.unitCost = scrap.unitCost;

            return PartialView("_EditModal", scrapDetail);
        }

        // POST: ScrapDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,scrapId,reasonId,machineId,quantity,weight,cost,employeeNumber")] ScrapDetail scrapDetail, int? operationId)
        {
            if (ModelState.IsValid)
            {
                _stackpoleContext.Entry(scrapDetail).State = EntityState.Modified;
                _stackpoleContext.SaveChanges();

                //return Redirect(Request.QueryString["r"]);
                Scrap scrap = _stackpoleContext.Scraps.Where(s => s.id == scrapDetail.scrapId).FirstOrDefault();

                getDropDownListForEditScrap(operationId, scrapDetail);

                //return View("_EditModal");
                return RedirectToAction("Index", new
                {
                    plantName = scrap.plantId,
                    departmentName = scrap.departmentId,
                    partId = scrap.partId,
                    operationName = scrap.operationId,
                    operationId = operationId,
                    weight = scrap.unitWeight,
                    cost = scrap.unitCost,
                    scrapId = scrap.id
                });
                //return Json(new { success = true });
            }
            var errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .Select(x => new { x.Key, x.Value.Errors })
                            .ToArray();

            getDropDownListForEditScrap(operationId, scrapDetail);
            return PartialView("_EditModal", scrapDetail);
        }

        // GET: ScrapDetails/Delete/5
        public ActionResult Delete(DeleteViewModel deleteViewModel)
        {
            if (deleteViewModel.scrapDetailId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScrapDetail scrapDetail = _stackpoleContext.ScrapDetails.Where(sd => sd.id == deleteViewModel.scrapDetailId).FirstOrDefault();
            if (scrapDetail == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteConfirm", deleteViewModel);
        }

        // POST: ScrapDetails/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        public ActionResult DeleteConfirmed(DeleteViewModel deleteViewModel)
        {
            ScrapDetail scrapDetail = _stackpoleContext.ScrapDetails.Where(sd => sd.id == deleteViewModel.scrapDetailId).FirstOrDefault();
            _stackpoleContext.ScrapDetails.Remove(scrapDetail);
            _stackpoleContext.SaveChanges();
            Scrap scrap = _stackpoleContext.Scraps.Where(s => s.id == scrapDetail.scrapId).FirstOrDefault();

            return RedirectToAction("Index", new
            {
                plantName = scrap.plantId,
                departmentName = scrap.departmentId,
                partId = scrap.partId,
                operationName = scrap.operationId,
                operationId = deleteViewModel.operationId,
                weight = scrap.unitWeight,
                cost = scrap.unitCost,
                scrapId = scrap.id
            });
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
                                    .Select(m => m.Key.Value)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stackpoleContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
