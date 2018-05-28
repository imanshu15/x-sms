var gameTable;

$(document).ready(function () {

    $("#hdnScreen").val("JOIN");
    // ----- GAME TABLE ------
    gameTable = $('#tblGameList').DataTable({
        select: true
    });
    getOpenGameList();
    setUpGameTableSelect();

    setTimeout(function () {
        setUpSignalRMethods();
    }, 1000);

});

function setUpSignalRMethods() {
    createGameAction();
    joinGameAction();
}

function createGameAction() {

    $("#btnCreateGame").click(function (e) {
        e.preventDefault();
        var playerName = $("#txtPlayerName").val();
        var playersCount = $("#txtPlayerCount").val();
        var isPrivate = false;
        if ($('#chkIsPrivateGame').is(":checked")) {
            isPrivate = true;
        }
        game.server.createGame(playerName, playersCount, isPrivate);
    });

}

function getOpenGameList() {
    gameTable.clear().draw();
    $.ajax({
        type: "GET",
        url: getAPIUrl() + "game",
        dataType: "json",
        success: function (data) {
            setUpGameTable(data);
        },
        failure: function (errMsg) {
            console.log(errMsg);
        }
    });
}

function setUpGameTable(data) {
    data.forEach(function (entry) {
        if (entry !== null) {
            gameTable.row.add([entry.GameId, entry.GameCode, entry.CreatedPlayer, entry.PlayersCount, 0, entry.StartTime]).draw();
        }
    });
}

function setUpGameTableSelect() {
    $('#tblGameList tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            gameTable.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
            showJoinPopUp(gameTable.rows('.selected').data()[0]);
        }
    }); 
}

function showJoinPopUp(data) {
    $("#hdnSelectedGameId").val(data[0]);
    $("#lblCreatedPlayer").text(data[2]);
    $("#mdlJoinConfirmation").modal('show');
}

function joinGameAction() {

    $("#btnJoinGame").click(function (e) {
        e.preventDefault();
        var playerName = $("#txtJoinPlayerName").val();
        var gameId = $("#hdnSelectedGameId").val();
        game.server.joinGame(playerName, gameId);
    });

}
