var gameTable;

$(document).ready(function () {

    $("#hdnScreen").val("JOIN");
    // ----- GAME TABLE ------
    gameTable = $('#tblGameList').DataTable({
        select: true
    });
    setTimeout(function () {
        getOpenGameList();
    }, 500);
    setUpGameTableSelect();
    setUpSignalRMethods();
});

function setUpSignalRMethods() {
    createGameAction();
    joinGameAction();
}

function createGameAction() {

    $("#btnCreateGame").click(function (e) {
        e.preventDefault();
        showPreloader();
        var playerName = $("#txtPlayerName").val();
        var playersCount = $("#txtPlayerCount").val();
        var isPrivate = false;
        if ($('#chkIsPrivateGame').is(":checked")) {
            isPrivate = true;
        }
        if (playerName != null && playerName != undefined && playerName != "") {
            if (!DoesPlayerExist(playerName)) {
                if (playersCount != null && playersCount != undefined && $.isNumeric(playersCount)) {
                    if (playersCount <= 4) {
                        game.server.createGame(playerName, playersCount, isPrivate);
                    } else {                  
                        showErrorMsg('Validation', 'Maximum players count is 4');
                    }
                } else {
                    showErrorMsg('Validation', 'Invalid player count');
                }
            } else {
                showErrorMsg('Validation', 'Sorry, Player name is already in use');
            }
        } else {
            showErrorMsg('Validation', 'Player name is required');
        }
        hidePreloader();
    });

}

function getOpenGameList() {
    gameTable.clear().draw();
    game.server.getCurrentGameList();
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
        $("#mdlJoinConfirmation").modal("hide");
        var playerName = $("#txtJoinPlayerName").val();
        var gameId = $("#hdnSelectedGameId").val();
        if (playerName != null && playerName != undefined && playerName != "") {
            if (!DoesPlayerExist(playerName)) {
                game.server.joinGame(playerName, gameId, "");
            } else {
                showErrorMsg('Validation', 'Sorry, Player name is already in use');
            }
        } else {
            showErrorMsg('Validation', 'Player name is required');
        }
    });

    $("#btnJoinPrivateGame").click(function (e) {
        e.preventDefault();
        debugger
        var playerName = $("#txtJoinPrivatePlayer").val();
        var gameCode = $("#txtJoinGameCode").val();
        if (playerName != null && playerName != undefined && playerName != "") {
            if (!DoesPlayerExist(playerName)) {
                game.server.joinGame(playerName, 0, gameCode);
            } else {
                    showErrorMsg('Validation', 'Sorry, Player name is already in use');
                }
            } else {
            showErrorMsg('Validation', 'Player name is required');
        }
    });
}

function DoesPlayerExist(player) {
    var returnValue = false;
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: getAPIUrl() + "Game/PlayerExist?playerName=" + player,
        async: false,
        success: function (res) {
        if (returnValue != null && returnValue != undefined)
            returnValue = res;
        }
    });

    return returnValue;
}
