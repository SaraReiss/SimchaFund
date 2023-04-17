$(() => {

    $("#new-contributor").on('click', function () {
        $(".modal-title").text("New Contributor")
        new bootstrap.Modal($(".new-contrib")[0]).show();
    })

}) 