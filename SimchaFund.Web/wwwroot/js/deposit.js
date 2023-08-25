$(() => {

    $(".deposit-button").on('click', function () {
        const button = $(this);

        const name = button.data('name');
        $("#deposit-name").text(name);
        //console.log(name);

        const Id = button.data('contribid');
        $("#contributor-id").val(Id);
        //console.log(Id);

        new bootstrap.Modal($('#depo')[0]).show();
    })
    
    $(".edit-contrib").on('click', function () {

        const button = $(this);
        const id = button.data('id');
        const firstName = button.data('first-name');
        const lastName = button.data('last-name');
        const cell = button.data('cell-number');
        const alwaysInclude = button.data('always-include');
        const date = button.data('date');

        $(".modal-title").text(`Edit`)
        $("#initialDepositDiv").hide();

        $("#contributor_first_name").val(firstName);
        $("#contributor_last_name").val(lastName);
        $("#contributor_cell_number").val(cell);
        $("#contributor_always_include").prop('checked', alwaysInclude === "True");
        $("#contributor_created_at").val(date);

        $(".modal-body").append(` <input name="id" type="hidden" value="${id}" id="cont-id">`)



        const form = $(".new-contrib form");
        form.attr('action', '/home/edit');
        new bootstrap.Modal($('.new-contrib')[0]).show();





    })
    //add this..
    //$("#search").on("input", function () {
    //    console.log('in search ');
    //    var searchText = $(this).val().toLowerCase();
    //    $(".tr").filter(function () {
    //        return $(this).text().toLowerCase().indexOf(searchText) === -1;
    //    }).hide();
    //    $(".tr").filter(function () {
    //        return $(this).text().toLowerCase().indexOf(searchText) !== -1;
    //    }).show();
    //});

    //$("#clear").on("click", function () {
    //    $("#search").val("");
    //    $(".tr").show();
    //});




}) 
