var game;
$(document).ready(function () {
    game = $.connection.gameHub;

    setUpClientMethods();

    $.connection.hub.start().done(function () {
        console.log("ConId : %o", $.connection.hub.id);
    });
    clPreloader();
});


$(window).on('beforeunload', function () {
    var gameId = $("#hdnSelectedGameId").val();
    game.server.disconnectPlayer(gameId);
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

    setupJoinClientMethods();
    setUpWaitClientMethods();
    setUpGameClientMethods();

}

function setupJoinClientMethods() {
    // JOIN - GameCreated
    game.client.gameCreated = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            console.log("Game Created Response");
            console.log(response);
            if (response !== null) {
                $("#hdnGameId").val(response.GameId);
                $("#hdnGameCode").val(response.GameCode);
                $("#hdnPlayerId").val(response.PlayerId);
                loadMainScreen("Join/Wait");
            }
        }
    };

    // JOIN - UpdateGameLIst
    game.client.updateGameList = function (data) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            setUpGameTable(data);
        }
    };

    // JOIN - UpdateGameLIst
    game.client.currentGameList = function (data) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            setUpGameTable(data);
        }
    };

    // JOIN - JoinSuccess
    game.client.joinSuccess = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            console.log("Join Success Response");
            console.log(response);
            if (response !== null) {
                $("#hdnGameId").val(response.GameId);
                $("#hdnPlayerId").val(response.PlayerId);
                $("#hdnGameCode").val(response.GameCode);
                loadMainScreen("Join/Wait");
            }
        }
    };


}

function setUpWaitClientMethods() {

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
        console.log("Game Started");
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            $("#msgTitle").text("Game Starting..");
            $("#msgBody").text("All players connected. Please wait till we get everything ready");
            $("#mdlMessage").modal("show");
            setTimeout(function () {
                showPreloader(400);
                setTimeout(function () {
                    $("#mdlMessage").modal("hide");
                    $("#hdnScreen").val("GAME");
                    loadMainScreen("Game/GameBoard");
                }, 400);
            }, 800);
        }
    };

    //WAIT - NotifyPlayers
    game.client.notifyJoinedPlayers = function (data) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            playerTable.row.add([data, "Connected"]).draw();
            game.server.isGameStarted($("#hdnGameId").val());
        }
    };

    //Player - Disconnected
    game.client.playerDisconnected = function (response) {
        $("#msgTitle").text("Player Disconnected");
        $("#msgBody").text("Sorry, A Player disconnected from the game.");
        $("#mdlMessage").modal("show");
        setTimeout(function () {
            showPreloader(400);
            setTimeout(function () {
                window.location.href = "/Game";
            }, 400);
        }, 1000);
    };
}

function setUpGameClientMethods() {

    game.client.startRound = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            setUpStocks(response);

            // -- Clock
            clock = $('.clock').FlipClock(60,  {
                clockFace: 'MinuteCounter',
                countdown: true,
                callbacks: {
                    stop: function () {
                        //$('.message').html('The clock has stopped!');
                    }
                }
            });
       }
    };

    game.client.stockBuySuccess = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            $('#gameScrnBalance').text(response);
        }
    };

    game.client.playerBoughtStock = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            addStockBoughtNews(response);
            getPlayerStock();
        }
    };

    game.client.stockBuyFailed = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            $('#msgTitle').text('Unsuccessful');
            $('#msgBody').text(response.Message);
            $('#mdlMessage').modal('show');
        }
    };

    game.client.loadPlayerStocksList = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            loadPlayerStocksGrid(response);
        }
    };

    game.client.stockSellSuccess = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            $('#gameScrnBalance').text(response);
        }
    };

    game.client.playerSoldStock = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            addStockSoldNews(response);
            getPlayerStock();
        }
    };

    game.client.stockSellFailed = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            $('#msgTitle').text('Unsuccessful');
            $('#msgBody').text(response.Message);
            $('#mdlMessage').modal('show');
        }
    };
}