$(function () {
    // default disable weight
    $(".weight").prop("readonly", true);
    $(".cost").prop("readonly", true);

    // re-calculate quantity and cost when user inputs weight
    $(document).on("input", ".weight", function (e) {
        e.preventDefault();

        const unitWeight = parseFloat($("#scrapModal_unitWeight").val());
        const unitCost = parseFloat($("#scrapModal_unitCost").val());
        const cost = $(".cost");
        const quantity = $(".quantity");
        const weight = parseInt(e.currentTarget.value);

        quantity.value = parseInt(weight / unitWeight);
        cost.value = unitCost * parseInt(quantity.value);

        quantity.val(quantity.value);
        cost.val(cost.value);
    });

    // re-calculate weight and cost when user inputs quantity
    $(document).on("input", ".quantity", function (e) {
        e.preventDefault();

        const unitWeight = parseFloat($("#scrapModal_unitWeight").val());
        const unitCost = parseFloat($("#scrapModal_unitCost").val());
        const cost = $(".cost");
        const quantity = parseInt(e.currentTarget.value);
        const weight = $(".weight");

        //quantity.value = weight / unitWeight.val();
        weight.value = quantity * unitCost;
        cost.value = unitCost * quantity;

        weight.val(weight.value);
        cost.val(cost.value);
    });
})

function tapNum(val) {
    const unitWeight = parseFloat($("#scrapModal_unitWeight").val());
    const unitCost = parseFloat($("#scrapModal_unitCost").val());

    const cost = $(".cost");
    const weight = $(".weight");
    const quantity = $(".quantity");

    if (weight.is('[readonly]')) {
        if (val == "Reset") {
            quantity.val(0);
            weight.val(0);
            cost.val(0);
        }
        else {
            const quantityString = parseInt($(".quantity").val() + val);
            quantity.val(quantityString);
            const quantityInput = parseInt($(".quantity").val());
            weight.value = quantityInput * unitCost;
            cost.value = unitCost * quantityInput;
            weight.val(weight.value);
            cost.val(cost.value);
        }
    }
    else {
        if (val == "Reset") {
            quantity.val(0);
            weight.val(0);
            cost.val(0);
        }
        else {
            const weightString = parseInt($(".weight").val() + val);
            weight.val(weightString);
            const weightInput = parseInt($(".weight").val());
            quantity.value = parseInt(weightInput / unitWeight);
            cost.value = unitCost * parseInt(quantity.value);
            quantity.val(quantity.value);
            cost.val(cost.value);
        }
    }

}

// toggle show/hide quantity and weight base on reason selection
function selectReason(val) {
    const elequantity = $(".quantity");
    const eleWeight = $(".weight");

    if (val === "8" || val === "9" || val === "10" || val === "11" || val === "12") {
        eleWeight.attr("readonly", false);
        elequantity.attr("readonly", true);
    }
    else {
        eleWeight.attr("readonly", true);
        elequantity.attr("readonly", false);
    }
}

