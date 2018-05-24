var gameTable;

$(document).ready(function () {

    // ----- GAME TABLE ------
    gameTable = $('#tblGameList').DataTable({
        select: true
    });
    getOpenGameList();
    setUpGameTableSelect();

    createGameAction();

    game.client.updateGameList = function () {
        console.log("Update Game Response");
    };
});

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

    game.client.gameCreated = function (response) {
        console.log("Game Created Response");
        console.log(response);
        if(response != null)
             window.location.href = "/Waiting/" + response.Data.GameId;
    };
}

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
            console.log("API");
            console.log(data);
        },
        failure: function (errMsg) {
            console.log(errMsg);
        }
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
        if (entry != null) {
            gameTable.row.add([entry.GameCode, entry.CreatedPlayer, entry.PlayersCount, 0, entry.StartTime]).draw();
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
    $("#lblCreatedPlayer").text(data[1]);
    $("#mdlJoinConfirmation").modal('show');
}