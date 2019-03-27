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
    public class OperationsController : Controller
    {
        private StackpoleEntities db;

        public OperationsController()
        {
            db = new StackpoleEntities();
        }

        [Authorize(Roles ="Admin")]
        public ActionResult AllOperations()
        {
            return View(db.Operations.ToList());
        }

        // GET: Operations
        //public ActionResult Index(string plantName, string departmentName, string partId)
        public ActionResult Index(int plantId, string plantName, string departmentName, string partId, double? weight, double? cost, int sequence)
        {
            ViewBag.partId = partId;
            ViewBag.plantId = plantId;
            ViewBag.plantName = plantName;
            ViewBag.departmentName = departmentName;
            ViewBag.sequence = sequence;

            ViewBag.weight = weight;
            ViewBag.cost = cost;

            var listOperations = db.qryPartDeptOp_GetOperator
                                    .Where(pdo =>  pdo.PartID == partId && pdo.Department == departmentName)
                                    .GroupBy(pdo => new { pdo.FirstOfOperation, pdo.operationID })
                                    .Select(pdo => new Operations { operationName = pdo.Key.FirstOfOperation, operationId = pdo.Key.operationID });

            return View(listOperations.ToList());
        }

        public ActionResult ConfirmSelect(string plantName, 
                                            string departmentName, 
                                            string partId, 
                                            int? operationId, 
                                            string operationName,
                                            double? weight,
                                            double? cost
            )
        {
            ViewBag.departmentName = departmentName;
            ViewBag.plantName = plantName;
            ViewBag.partId = partId;
            ViewBag.operationId = operationId;
            ViewBag.operationName = operationName;
            ViewBag.weight = weight;
            ViewBag.cost = cost;

            try
            {
                return PartialView("_ConfirmSelect");
            }
            catch (Exception ex)
            {
                return Json(new { url = Url.Action("Index", "Error"), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Operations/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Operation operation = db.Operations.Find(id);
            if (operation == null)
            {
                return HttpNotFound();
            }
            return View(operation);
        }

        // GET: Operations/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Operations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description")] Operation operation)
        {
            if (ModelState.IsValid)
            {
                db.Operations.Add(operation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(operation);
        }

        // GET: Operations/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Operation operation = db.Operations.Find(id);
            if (operation == null)
            {
                return HttpNotFound();
            }
            return View(operation);
        }

        // POST: Operations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description")] Operation operation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(operation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(operation);
        }

        // GET: Operations/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Operation operation = db.Operations.Find(id);
            if (operation == null)
            {
                return HttpNotFound();
            }
            return View(operation);
        }

        // POST: Operations/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Operation operation = db.Operations.Find(id);
            db.Operations.Remove(operation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
