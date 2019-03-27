var ObjectState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
};

var scrapDetailMapping = {
    'ScrapDetails': {
        key: function (scrapDetail) {
            return ko.utils.unwrapObservable(scrapDetail.id);
        },
        create: function (options) {
            return new ScrapDetailViewModel(options.data);
        }
    }
};

ScrapDetailViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, scrapDetailMapping, self);

    self.flaggedDetailAsEdited = function () {
        if (self.ObjectState() != ObjectState.Added) {
            self.ObjectState(ObjectState.Modified);
        }

        return true;
    },

    self.changeQuantity = function (unitCost, unitWeight) {
        const calcWeight = (self.quantity() * unitCost).toFixed(2);
        const calcCost = (unitCost * self.quantity()).toFixed(2);

        self.weight(calcWeight);
        self.cost(calcCost);

        self.flaggedDetailAsEdited();
    },

    self.changeWeight = function (unitCost, unitWeight) {
        const calcQuantity = (self.weight() / unitWeight).toFixed(0);
        const calcCost = (unitCost * self.quantity()).toFixed(2);

        self.quantity(calcQuantity);
        self.cost(calcCost);

        self.flaggedDetailAsEdited();
    },

    self.setReadonlyWeight = function () {
        const reasonId = self.reasonId();
        return (reasonId === "8" || reasonId === "9" || reasonId === "10" || reasonId === "11" || reasonId === "12") ? false : true;
    },

    self.setReadonlyQuantity = function () {
        const reasonId = self.reasonId();
        return (reasonId === "8" || reasonId === "9" || reasonId === "10" || reasonId === "11" || reasonId === "12") ? true : false;
    }
};

ScrapViewModel = function (data) {
    var self = this;
    self.listPlants = ko.observableArray([]);
    self.listDepartments = ko.observableArray([]);
    self.listParts = ko.observableArray([]);
    self.listOperations = ko.observableArray([]);
    self.listMachines = ko.observableArray([]);
    self.listScrapReasons = ko.observableArray([]);

    data.date = moment(data.date).format('MMMM Do YYYY, h:mm:ss a');
    ko.mapping.fromJS(data, scrapDetailMapping, self);

    self.save = function () {

        $.ajax({
            url: "/Scraps/Save/",
            type: "POST",
            data: ko.toJSON(self),
            contentType: "application/json",
            success: function (data) {
                if (data.scrapViewModel) {
                    ko.mapping.fromJS(data.scrapViewModel, {}, self);

                    if (data.scrapViewModel.MessageToClient) {
                        $.notify({
                            // options
                            title: '<strong>Scrap Reporting System: </strong>',
                            message: `Scrap ID: ${data.scrapViewModel.MessageToClient}`
                        }, {
                            // settings
                            type: 'info',
                            delay:  5000,
                            timer: 1000,
                            animate: {
                                enter: 'animated fadeInDown',
                                exit: 'animated fadeOutUp'
                            }
                        });
                    }
                }

                if (data.newLocation != null)
                    window.location = data.newLocation;
            },
            error: function (data) {
                alert(`error while saving: ${JSON.stringify(data)}`);
            }
        });
    },

    self.flaggedAsEdited = function () {
        if (self.ObjectState() != ObjectState.Added) {
            self.ObjectState(ObjectState.Modified);
        }

        return true;
    },

    self.addScrapDetailItem = function () {
        var scrapDetailItem = new ScrapDetailViewModel({
            id: 0, machineId: 0, reasonId: 0, quantity: 0,
            cost: 0, weight: 0, employeeNumber: 0, ObjectState: ObjectState.Added
        });
        self.ScrapDetails.push(scrapDetailItem);

    },

    self.Total = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.ScrapDetails(), function (scrapDetail) {
            total += parseFloat(scrapDetail.cost());
        });
        return total.toFixed(2);
    }),

    self.deleteScrapDetail = function (scrapDetail) {
        self.ScrapDetails.remove(this);

        if (scrapDetail.id() > 0 && self.ScrapDetailsToDelete.indexOf(scrapDetail.id()) == -1)
            self.ScrapDetailsToDelete.push(scrapDetail.id());
    },
    
    self.fetchListDepartments = function () {
        const selectedPlantId = $("#plantId").val();

        $.ajax({
            url: '/Scraps/getListDepartments/',
            type: "GET",
            data: { plantId: selectedPlantId },
            datatype: "json",
            contentType: "application/json charset=utf-8",
            success: function (data) {
                if (data && data.length) {
                    scrapViewModel.listDepartments(data);
                    scrapViewModel.listParts.removeAll();
                    scrapViewModel.listOperations.removeAll();
                    scrapViewModel.listMachines.removeAll();
                    scrapViewModel.unitCost(0);
                    scrapViewModel.unitWeight(0);
                    scrapViewModel.listScrapReasons.removeAll();
                } else {
                    scrapViewModel.listDepartments.removeAll();
                    scrapViewModel.listParts.removeAll();
                    scrapViewModel.listOperations.removeAll();
                    scrapViewModel.listMachines.removeAll();
                    scrapViewModel.unitCost(0);
                    scrapViewModel.unitWeight(0);
                    scrapViewModel.listScrapReasons.removeAll();
                }
            },
            error: function (data) {
                //alert("Cannot get departments now!");
            }
        });

        self.flaggedAsEdited();
    },

    self.fetchListParts = function (viewModel, event) {
        const selectedDepartmentName = $("#departmentId option:selected").text().split(" - ")[1];
        const selectedSequence = $("#departmentId option:selected").text().split(" - ")[0];

        $.ajax({
            url: '/Scraps/getListParts/',
            type: "GET",
            data: { departmentName: selectedDepartmentName, sequence: selectedSequence },
            datatype: "json",
            contentType: "application/json charset=utf-8",
            success: function (data) {
                if (data && data.length) {
                    scrapViewModel.listParts(data);
                    scrapViewModel.listOperations.removeAll();
                    scrapViewModel.listMachines.removeAll();
                    scrapViewModel.unitCost(0);
                    scrapViewModel.unitWeight(0);
                    scrapViewModel.listScrapReasons.removeAll();
                } else {
                    scrapViewModel.listParts.removeAll();
                    scrapViewModel.listOperations.removeAll();
                    scrapViewModel.listMachines.removeAll();
                    scrapViewModel.unitCost(0);
                    scrapViewModel.unitWeight(0);
                    scrapViewModel.listScrapReasons.removeAll();
                }
            },
            error: function (data) {
                //alert("Cannot get parts now!");
            }
        });

        self.flaggedAsEdited();
    },

    self.fetchListOperations = function () {
        setTimeout(function () {
            const selectedPartId = $("#partId").val();
            const selectedDepartmentName = $("#departmentId option:selected").text().split(" - ")[1];

            $.ajax({
                url: '/Scraps/getListOperations/',
                type: "GET",
                data: { partId: selectedPartId, departmentName: selectedDepartmentName },
                datatype: "json",
                contentType: "application/json charset=utf-8",
                success: function (data) {
                    if (data && data.length) {
                        scrapViewModel.listOperations(data);
                        scrapViewModel.listMachines.removeAll();
                        scrapViewModel.unitCost(0);
                        scrapViewModel.unitWeight(0);
                        scrapViewModel.listScrapReasons.removeAll();
                    } else {
                        scrapViewModel.listOperations.removeAll();
                        scrapViewModel.listMachines.removeAll();
                        scrapViewModel.unitCost(0);
                        scrapViewModel.unitWeight(0);
                        scrapViewModel.listScrapReasons.removeAll();
                    }
                },
                error: function (data) {
                    //alert("Cannot get operations now!");
                }
            });
        }, 500);

        self.flaggedAsEdited();
    },

    // get machines, scrap reasons, weight and cost
    // should rename function name later
    self.fetchListMachines = function () {
        setTimeout(function () {
            const selectedOperationId = $("#operationId option:selected").text().split(" - ")[0];

            const selectedPlantName = $("#plantId").val();
            const selectedSequence = $("#departmentId option:selected").text().split(" - ")[0];
            const selectedPartId = $("#partId").val();

            $.ajax({
                url: '/Scraps/getLisMachines/',
                type: "GET",
                data: {
                    operationId: selectedOperationId, 
                    plantName: selectedPlantName,
                    sequence: selectedSequence,
                    partId: selectedPartId
                },
                datatype: "json",
                contentType: "application/json charset=utf-8",
                success: function (data) {
                    if (data) {
                        if (data.listMachines && data.listMachines.length) {
                            scrapViewModel.listMachines(data.listMachines);
                        } else {
                            scrapViewModel.listMachines.removeAll();
                        }

                        if (data.listScrapReasons && data.listScrapReasons.length) {
                            scrapViewModel.listScrapReasons(data.listScrapReasons);
                        } else {
                            scrapViewModel.listScrapReasons.removeAll();
                        }

                        scrapViewModel.unitCost(data.cost);
                        scrapViewModel.unitWeight(data.weight);
                    }
                },
                error: function (data) {
                    //alert("Cannot get machines now!");
                }
            });
        }, 500);

        self.flaggedAsEdited();
    },

    self.date.formatted = ko.pureComputed(function () {
        return moment(date()).format("DD/MM/YYYY");
    });
};

$("#edit-scrap").validate({
    submitHandler: function () {
        scrapViewModel.save();
    },

    rules: {
        plantId: {
            required: true
        },
        departmentId: {
            required: true
        },
        partId: {
            required: true
        },
        operationId: {
            required: true
        },
        machineId: {
            required: true
        },
        reasonId: {
            required: true
        },
        quantity: {
            required: true,
            digits: true,
            range: [1, 1000000]
        },
        cost: {
            required: true,
            range: [0, 1000000]
        },
        weight: {
            required: true,
            range: [0, 1000000]
        }
    },

    messages: {
        plantId: {
            required: "You should select plant."
        },
        departmentId: {
            required: "You should select department."
        },
        partId: {
            required: "You should select part."
        },
        operationId: {
            required: "You should select operation."
        }
    },

    tooltip_options: {
        plantId: {
            placement: 'right'
        },
        departmentId: {
            placement: 'right'
        },
        partId: {
            placement: 'right'
        },
        operationId: {
            placement: 'right'
        }
    }
})