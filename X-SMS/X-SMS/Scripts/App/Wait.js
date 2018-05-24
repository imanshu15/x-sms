var playerTable;
var gameId;

$(document).ready(function () {

    gameId = $("#hdnGameId").val();

    // ----- Player TABLE ------
    playerTable = $('#tblPlayersList').DataTable();

    getPlayerList();
    
    game.client.startGame = function () {
        window.location.href = "/Game/" + gameId;
    };
});

function getPlayerList() {
    playerTable.clear().draw();
    $.ajax({
        type: "GET",
        url: getAPIUrl() + "GamePlayer",
        dataType: "json",
        success: function (data) {
            setUpPlayerTable(data);
        },
        failure: function (errMsg) {
            console.log(errMsg);
        }
    });
}

function setUpPlayerTable(data) {
    data.forEach(function (entry) {
        if (entry != null) {
            playerTable.row.add([entry.PlayerName, "Connected"]).draw();
        }
    });
}