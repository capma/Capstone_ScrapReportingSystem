using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stackpole.Models;


namespace Stackpole.ViewModels
{
    public static class Helpers
    {
        public static ScrapViewModel CreateScrapViewModelFromScrap(Scrap scrap)
        {
            ScrapViewModel scrapViewModel = new ScrapViewModel();

            scrapViewModel.id = scrap.id;
            scrapViewModel.machineId = scrap.machineId;
            scrapViewModel.operationId = scrap.operationId;
            scrapViewModel.partId = scrap.partId;
            scrapViewModel.plantId = scrap.plantId;
            scrapViewModel.unitCost = scrap.unitCost;
            scrapViewModel.unitWeight = scrap.unitWeight;
            scrapViewModel.date = scrap.date;
            scrapViewModel.departmentId = scrap.departmentId;
            scrapViewModel.cancelled = scrap.cancelled;
            scrapViewModel.ObjectState = ObjectState.Unchanged;

            foreach (ScrapDetail scrapDetail in scrap.ScrapDetails)
            {
                ScrapDetailViewModel scrapDetailViewModel = new ScrapDetailViewModel();

                scrapDetailViewModel.id = scrapDetail.id;
                scrapDetailViewModel.machineId = scrapDetail.machineId;
                scrapDetailViewModel.reasonId = scrapDetail.reasonId;
                scrapDetailViewModel.quantity = scrapDetail.quantity;
                scrapDetailViewModel.weight = scrapDetail.weight;
                scrapDetailViewModel.cost = scrapDetail.cost;
                scrapDetailViewModel.employeeNumber = scrapDetail.employeeNumber;
                scrapDetailViewModel.ObjectState = ObjectState.Unchanged;
                scrapDetailViewModel.scrapId = scrapDetail.scrapId;

                scrapViewModel.ScrapDetails.Add(scrapDetailViewModel);
            }


            return scrapViewModel;
        }

        public static Scrap CreateScrapFromScrapViewModel(ScrapViewModel scrapViewModel)
        {
            Scrap scrap = new Scrap();

            scrap.id = scrapViewModel.id;
            scrap.machineId = scrapViewModel.machineId;
            scrap.operationId = scrapViewModel.operationId;
            scrap.partId = scrapViewModel.partId;
            scrap.plantId = scrapViewModel.plantId;
            scrap.unitCost = scrapViewModel.unitCost;
            scrap.unitWeight = scrapViewModel.unitWeight;
            scrap.date = scrapViewModel.date;
            scrap.departmentId = scrapViewModel.departmentId;
            scrap.cancelled = scrapViewModel.cancelled;
            scrap.ObjectState = scrapViewModel.ObjectState;

            int temporaryScrapDetailId = -1;

            foreach (ScrapDetailViewModel scrapDetailViewModel in scrapViewModel.ScrapDetails)
            {
                ScrapDetail scrapDetail = new ScrapDetail();

                scrapDetail.machineId = scrapDetailViewModel.machineId;
                scrapDetail.reasonId = scrapDetailViewModel.reasonId;
                scrapDetail.quantity = scrapDetailViewModel.quantity;
                scrapDetail.cost = scrapDetailViewModel.cost;
                scrapDetail.weight = scrapDetailViewModel.weight;
                scrapDetail.employeeNumber = scrapDetailViewModel.employeeNumber;
                scrapDetail.ObjectState = scrapDetailViewModel.ObjectState;

                if (scrapDetailViewModel.ObjectState != ObjectState.Added)
                    scrapDetail.id = scrapDetailViewModel.id;
                else
                {
                    scrapDetail.id = temporaryScrapDetailId;
                    temporaryScrapDetailId--;
                }


                scrapDetail.scrapId = scrapViewModel.id;
                scrap.ScrapDetails.Add(scrapDetail);
            }

            return scrap;
        }

        public static string GetMessageToClient(ObjectState objectState, string name)
        {
            string messageToClient = string.Empty;

            switch (objectState)
            {
                case ObjectState.Added:
                    messageToClient = string.Format("{0} has been added to the database", name);
                    break;
                case ObjectState.Modified:
                    messageToClient = string.Format("{0}'s has been updated", name);
                    break;
            }

            return messageToClient;
        }

        public static PlantViewModel CreatePlantViewModelFromPlant(Plant plant)
        {
            PlantViewModel plantViewModel = new PlantViewModel();
            plantViewModel.id = plant.id;
            plantViewModel.name = plant.name;
            plantViewModel.description = plant.description;
            plantViewModel.area = plant.area;

            return plantViewModel;
        }

        public static Plant CreatePlantFromPlantViewModel(PlantViewModel plantViewModel)
        {
            Plant plant = new Plant();
            plant.id = plantViewModel.id;
            plant.name = plantViewModel.name;
            plant.description = plantViewModel.description;
            plant.area = plantViewModel.area;

            return plant;
        }

    }
}