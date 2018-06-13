var playerTable;
var gameId;

$(document).ready(function () {
    $("#hdnScreen").val("WAIT");
    gameId = $("#hdnGameId").val();

    var gameCode = $("#hdnGameCode").val();
    $("#lblPrivateGameCode").text(gameCode);
    // ----- Player TABLE ------
    playerTable = $('#tblPlayersList').DataTable();
    getPlayerList();

});

function getPlayerList() {
    if ($("#hdnGameId").val() != undefined) {
        playerTable.clear().draw();
        game.server.requestPlayerList($("#hdnGameId").val());
    }
}

function setUpPlayerTable(data) {
   
    data.forEach(function (entry) {
        if (entry != null) {
            playerTable.row.add([entry.PlayerName, "Connected"]).draw();
        }
    });
}

