$(function () {
    $('.link').click(function () {
        var plantName = $(this).data('plantname');
        var departmentName = $(this).data('departmentname');
        var partId = $(this).data('partid');
        var operationId = $(this).data('operationid');
        var operationName = $(this).data('operationname');
        var weight = $(this).data('weight');
        var cost = $(this).data('cost');

        var urlPath = "Operations/ConfirmSelect/";

        $.ajax({
            //url: '@Url.Action("ConfirmSelect", "Operations")',
            url: urlPath,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {
                "plantName": plantName,
                "departmentName": departmentName,
                "partId": partId,
                "operationId": operationId,
                "operationName": operationName,
                "weight": weight,
                "cost": cost
            },
            success: function (res) {
                $(".modal-body").html(res);
                $("#myModal").modal('show');
            },
            error: function (err) {
                alert(`Dynamic content load failed: ${JSON.stringify(err)}`);
            }
        });
    });
})