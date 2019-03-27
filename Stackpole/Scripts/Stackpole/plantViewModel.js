var ObjectState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
}

plantViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);

    self.save = function () {
        $.ajax({
            url: "/Plants/Save/",
            type: "POST",
            data: ko.toJSON(self),
            contentType: "application/json",
            success: function (data) {
                if (data.plantViewModel)
                    ko.mapping.fromJS(data.plantViewModel, {}, self);

                if (data.newLocation != null)
                    window.location = data.newLocation;
            },
            error: function (data) {
                
            }
        });
    }

    self.flagPlantAsEdited = function () {
        if (self.ObjectState() != ObjectState.Added) {
            self.ObjectState(ObjectState.Modified);
        }
        return true;
    }
}

$("#edit-plant").validate({
    submitHandler: function() {
        plantViewModel.save();
    },

    rules: {
        area: {
            digit: true
        }
    },

    messages: {
        area: {
            digit: "Please input digits only"
        }
    }
})