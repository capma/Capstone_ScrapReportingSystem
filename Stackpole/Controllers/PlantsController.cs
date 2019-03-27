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
    public class PlantsController : Controller
    {
        private StackpoleEntities _stackpoleContext;

        public PlantsController()
        {
            _stackpoleContext = new StackpoleEntities();
        }

        
        public ActionResult Index()
        {
            return View(_stackpoleContext.Plants.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = _stackpoleContext.Plants.Find(id);
            if (plant == null)
            {
                return HttpNotFound();
            }

            PlantViewModel plantViewModel = ViewModels.Helpers.CreatePlantViewModelFromPlant(plant);
            plantViewModel.MessageToClient = "View from viewmodel";

            return View(plantViewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = _stackpoleContext.Plants.Find(id);
            if (plant == null)
            {
                return HttpNotFound();
            }

            PlantViewModel plantViewModel = ViewModels.Helpers.CreatePlantViewModelFromPlant(plant);
            plantViewModel.ObjectState = ObjectState.Unchanged;
            plantViewModel.MessageToClient = string.Format("The original value of plant name is {0}", plantViewModel.name);

            return View(plantViewModel);
        }

        // GET: Plants/Create
        public ActionResult Create()
        {
            PlantViewModel plantViewModel = new PlantViewModel();
            plantViewModel.ObjectState = ObjectState.Added;
            return View(plantViewModel);
        }

        public JsonResult Save(PlantViewModel plantViewModel)
        {
            Plant plant = ViewModels.Helpers.CreatePlantFromPlantViewModel(plantViewModel);
            plant.ObjectState = plantViewModel.ObjectState;

            _stackpoleContext.Plants.Attach(plant);
            _stackpoleContext.ChangeTracker.Entries<IObjectWithState>().Single().State = Models.Helpers.ConvertState(plant.ObjectState);
            _stackpoleContext.SaveChanges();

            if (plantViewModel.ObjectState == ObjectState.Deleted)
            {
                return Json(new { newLocation = "/Plants/Index/" });
            }

            plantViewModel.MessageToClient = ViewModels.Helpers.GetMessageToClient(plantViewModel.ObjectState, plant.name);
            plantViewModel.id = plant.id;
            plantViewModel.ObjectState = ObjectState.Unchanged;

            return Json(new { plantViewModel });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = _stackpoleContext.Plants.Find(id);
            if (plant == null)
            {
                return HttpNotFound();
            }

            PlantViewModel plantViewModel = new PlantViewModel();
            //plantViewModel.id = plant.id;
            //plantViewModel.description = plant.description;
            //plantViewModel.area = plant.area;
            //plantViewModel.name = plant.name;

            plantViewModel = ViewModels.Helpers.CreatePlantViewModelFromPlant(plant);
            plantViewModel.MessageToClient = "You are about to permanently delete this plant";
            plantViewModel.ObjectState = ObjectState.Deleted;

            return View(plantViewModel);
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
