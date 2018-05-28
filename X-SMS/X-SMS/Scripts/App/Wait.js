var playerTable;
var gameId;

$(document).ready(function () {
    $("#hdnScreen").val("WAIT");
    gameId = $("#hdnGameId").val();

    // ----- Player TABLE ------
    playerTable = $('#tblPlayersList').DataTable();

    setTimeout(function () {
        getPlayerList();
    }, 1000);
  

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