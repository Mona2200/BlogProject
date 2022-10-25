$(document).ready(function () {
    $(".menu-sm").click(function () {
        $(".modal-menu").modal('show')
    });
    $("#btnAddNew").click(function () {
        $("#AddFormsTags").show();
        $("#btnAddNew").hide();
    });
    $("#ExistsTags>p>input[type=checkbox]").change(function () {
        if ($(this).is(':checked')) {
            var data = $("#Tags").val();
            $("#Tags").val(data + $(this).val() + ' ');
            $("#TagsHide").val($("#Tags").val());
        }
        else {
            var data = $("#Tags").val();
            var newdata = data.replace($(this).val(), '');
            console.log(newdata);
            $("#Tags").val(newdata);
            $("#TagsHide").val(newdata);
        }
    });
    $(".btnComm").click(function () {
        $(this).parents(".parentComm").next(".hideComm").toggle();
    });

    $("#linkRemove").click(function () {
        $("#modalAgreement").modal('show')
    });

    $("#closeModalAgreement").click(function () {
        $("#modalAgreement").modal('hide')
    });
});
