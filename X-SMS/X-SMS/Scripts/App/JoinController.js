$(document).ready(function () {
    console.log("Join Controller");
});

function createAGame() {

    var playerName = $("#playerName").val();
    var playersCount = $("#playersCount").val();

    $.ajax({
        type: "POST",
        url: "/Join/CreateAGame",
        data: JSON.stringify({ playerName: playerName, noOfPlayers: playersCount }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            alert(data);
        },
        failure: function (errMsg) {
            console.log(errMsg);
        }
    });
}