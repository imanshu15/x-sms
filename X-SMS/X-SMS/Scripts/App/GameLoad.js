$(document).ready(function () { 
    loadMainScreen("Join/Index");
});

function loadMainScreen(url) {
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            $("#gameContent").html(data);
        },
        failure: function (errMsg) {
            console.log(errMsg);
        }
    });
}