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
    public class PartDepartmentsController : Controller
    {
        private StackpoleEntities db;

        public PartDepartmentsController()
        {
            db = new StackpoleEntities();
        }

        // GET: PartDepartments
        public ActionResult Index(int plantId, string plantName, string departmentName, int? sequence)
        {
            ViewBag.plantId = plantId;
            ViewBag.plantName = plantName;
            ViewBag.departmentName = departmentName;
            ViewBag.sequence = sequence;

            var getPartDepartment = db.qryPartByPlants
                                    .Where(pbp => pbp.name == departmentName && pbp.sequence == sequence)
                                    .OrderBy(pbp => pbp.id)
                                    .Select(pbp => new PartCodes { partId = pbp.id, departmentName = pbp.name, weight = pbp.weightlbs, cost = pbp.cost });
            return View(getPartDepartment.ToList());
        }


        // GET: PartDepartments/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartDepartment partDepartment = db.PartDepartments.Find(id);
            if (partDepartment == null)
            {
                return HttpNotFound();
            }
            return View(partDepartment);
        }

        // GET: PartDepartments/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.deptID = new SelectList(db.Departments, "id", "name");
            ViewBag.partID = new SelectList(db.Parts, "id", "description");
            return View();
        }

        // POST: PartDepartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,partID,deptID,sequence,cost,weightlbs,area,dateCost")] PartDepartment partDepartment)
        {
            if (ModelState.IsValid)
            {
                db.PartDepartments.Add(partDepartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.deptID = new SelectList(db.Departments, "id", "name", partDepartment.deptID);
            ViewBag.partID = new SelectList(db.Parts, "id", "description", partDepartment.partID);
            return View(partDepartment);
        }

        // GET: PartDepartments/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartDepartment partDepartment = db.PartDepartments.Find(id);
            if (partDepartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.deptID = new SelectList(db.Departments, "id", "name", partDepartment.deptID);
            ViewBag.partID = new SelectList(db.Parts, "id", "description", partDepartment.partID);
            return View(partDepartment);
        }

        // POST: PartDepartments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,partID,deptID,sequence,cost,weightlbs,area,dateCost")] PartDepartment partDepartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partDepartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.deptID = new SelectList(db.Departments, "id", "name", partDepartment.deptID);
            ViewBag.partID = new SelectList(db.Parts, "id", "description", partDepartment.partID);
            return View(partDepartment);
        }

        // GET: PartDepartments/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartDepartment partDepartment = db.PartDepartments.Find(id);
            if (partDepartment == null)
            {
                return HttpNotFound();
            }
            return View(partDepartment);
        }

        // POST: PartDepartments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PartDepartment partDepartment = db.PartDepartments.Find(id);
            db.PartDepartments.Remove(partDepartment);
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
