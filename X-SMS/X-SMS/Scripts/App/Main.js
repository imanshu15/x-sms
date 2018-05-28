var game;
$(document).ready(function () {
    game = $.connection.gameHub;
    game.client.gameCreated = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            console.log("Game Created Response");
            console.log(response);
            if (response !== null) {
                $("#hdnGameId").val(response.GameId);
                loadMainScreen("Join/Wait");
            }
        }
    };
    setUpClientMethods();

    $.connection.hub.start().done(function () {
        console.log("ConId : %o", $.connection.hub.id);
    });
    clPreloader();
});

var clPreloader = function () {

    $("html").addClass('cl-preload');

    $("#loader").fadeOut("slow", function () {
        $("#preloader").delay(400).fadeOut("slow");
    });
    $("html").removeClass('cl-preload');
    $("html").addClass('cl-loaded');

};

function showPreloader(miliSeconds) {
    $("html").addClass('cl-preload');

    $("#loader").fadeOut("slow", function () {
        $("#preloader").delay(miliSeconds).fadeOut("slow");
    });
    $("html").removeClass('cl-preload');
    $("html").addClass('cl-loaded');
}

function getAPIUrl() {
    return "http://localhost:1597/api/";
}

function setUpClientMethods() {

    // JOIN - GameCreated
    game.client.gameCreated = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            console.log("Game Created Response");
            console.log(response);
            if (response !== null) {
                $("#hdnGameId").val(response.GameId);
                loadMainScreen("Join/Wait");
            }
        }
    };

    // JOIN - UpdateGameLIst
    game.client.updateGameList = function () {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            console.log("Update Game Response");
        }
    };

    // JOIN - JoinSuccess
    game.client.joinSuccess = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            console.log("Join Success Response");
            console.log(response);
            if (response !== null) {
                $("#hdnGameId").val(response.GameId);
                loadMainScreen("Join/Wait");
            }
        }
    };

    //WAIT - PlayerList
    game.client.playerList = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            console.log("Player List");
            console.log(response);
            if (response != null)
                setUpPlayerTable(response);
        }
    };

    //WAIT - GameStarted
    game.client.gameStarted = function () {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            $("#msgTitle").text("Game Starting..");
            $("#msgBody").text("All players connected. Please wait till we get everything ready");
            $("#mdlMessage").modal("show");
            setTimeout(function () {
                showPreloader(400);
                setTimeout(function () {
                    window.location.href = "/Game/" + gameId;
                }, 400);
            }, 800);
        }
    };

    //WAIT - NotifyPlayers
    game.client.notifyJoinedPlayers = function (data) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            playerTable.row.add([data, "Connected"]).draw();
            game.server.IsGameStarted($("#hdnGameId").val());
        }
    };
}