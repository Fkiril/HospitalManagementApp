$(document).ready(function () {
    $('.select-drug').on('change', function () {
        var idDrug = $(this).val();
        console.log(idDrug);
        $.ajax({
            type: 'GET',
            url: `/Drugs/GetDrugById/`,
            data: {
                id: idDrug
            },
            success: function (response) {
                if (response != null) {
                    var quantity = $('.form-add-detail').find('.quantity-drug')[0];
                    $(quantity).val(response.quantity);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    });

    $('.btn-delete-drug').on('click', function () {
        var idDrug = $(this).val();
        if (confirm('Do you want to delete this drug?')) {
            $.ajax({
                type: 'POST',
                url: `/Drugs/Remove_v2/`,
                data: {
                    id: idDrug
                },
                success: function (response) {
                    if (response == "ok") {
                        location.reload();
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            })
        }
    });

    $('.btn-delete-pres').on('click', function () {
        var idPres = $(this).val();
        if (confirm('Do you want to delete this prescription?')) {
            $.ajax({
                type: 'POST',
                url: `/Prescription/Remove_v2/`,
                data: {
                    id: idPres
                },
                success: function (response) {
                    if (response == "ok") {
                        location.reload();
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            })
        }
    });
});