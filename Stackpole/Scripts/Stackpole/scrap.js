$(function () {
    $('.btn-add-scrap').click(function (e) {
        e.preventDefault();

        var scrapModal = {
            id: $(this).data('scrapid'),
            date: $(this).data('date'),
            partId: $(this).data('partid'),
            departmentId: $(this).data('departmentid'),
            plantId: $(this).data('plantid'),
            machineId: 0,
            operationId: $(this).data('operationname'),
            unitCost: $(this).data('unitcost'),
            unitWeight: $(this).data('unitweight'),
            cancelled: false
        };

        var scrapDetailModal = {
            id: 0,
            scrapId: $(this).data('scrapid'),
            reasonId: 0,
            machineId: 0,
            quantity: 0,
            weight: 0,
            cost: 0,
            employeeNumber: 0,
        };

        var scrapBasicInfoModal = {
            "scrapModal": scrapModal,
            "scrapDetailModal": scrapDetailModal,
            "operationIdInt": $(this).data('operationid')
        };

        var urlPath = "/ScrapDetails/CreateNewScrapModal";

        $.ajax({
            url: urlPath,
            type: "post",
            datatype: "json",
            data: scrapBasicInfoModal,
            cache: false,
            success: function (res) {
                $(".modal-content").html(res);
                $("#myModal").modal('show');

                $(".weight").prop("readonly", true);
                $(".cost").prop("readonly", true);

                $.validator.unobtrusive.parse("#frm-add-scrap");
            },
            error: function (err) {
                alert(`Dynamic content load failed: ${JSON.stringify(err)}`);
            }
        });
    });

    $(".btn-edit-scrap").click(function (e) {
        e.preventDefault();

        var scrapDetailId = $(this).data("scrapdetailid");
        var scrapId = $(this).data("scrapid");
        var operationId = $(this).data("operationid");
        var reasonId = $(this).data("reasonid");

        $.ajax({
            url: "ScrapDetails/EditModal/",
            type: "get",
            data: {
                "scrapDetailId": scrapDetailId,
                "scrapId": scrapId,
                "operationId": operationId
            },
            success: function (res) {
                $(".modal-content").html(res);
                $("#myModal").modal('show');
                $(".cost").prop("readonly", true);
                if (reasonId === 8 || reasonId === 9 || reasonId === 10 || reasonId === 11 || reasonId === 12) {
                    $(".weight").prop("readonly", false);
                    $(".quantity").prop("readonly", true);
                }
                else {
                    $(".weight").prop("readonly", true);
                    $(".quantity").prop("readonly", false);
                }

                $.validator.unobtrusive.parse("#frm-edit-scrap");
            },
            error: function (err) {
                alert(`Dynamic content load failed: ${JSON.stringify(err)}`);
            }
        });
    });

    $(".btn-delete-scrap").click(function (e) {
        e.preventDefault();

        var scrapDetailId = $(this).data("scrapdetailid");
        var operationId = $(this).data("operationid");

        $.ajax({
            url: "ScrapDetails/Delete/",
            type: "GET",
            data: {
                "scrapDetailId": scrapDetailId,
                "operationId": operationId
            },
            success: function (res) {
                $(".modal-content").html(res);
                $("#myModal").modal('show');
            },
            error: function (err) {
                alert(`Dynamic content load failed: ${JSON.stringify(err)}`);
            }
        });
    });

    $(".btn-cancel-entry").click(function (e) {
        e.preventDefault();

        const scrapId = $(this).data("scrapid");

        $.ajax({
            url: "/ScrapDetails/CancelEntry/",
            type: "POST",
            data: {
                "scrapId": scrapId
            },
            success: function (data) {
                $(".modal-content").html(data);
                $("#myModal").modal('show');
            }
        });
    });
})