var game;

$(document).ready(function () {
    game = $.connection.gameHub;

    setUpClientMethods();

    $.connection.hub.start().done(function () {
        console.log("ConId : %o", $.connection.hub.id);
    });
    clPreloader();

    $(document).on('click', function () {
        $('.collapse').collapse('hide');
    })

    $(window).bind('beforeunload', function (eventObject) {
        var gameId = $("#hdnSelectedGameId").val();
        if(gameId != null)
            game.server.disconnectPlayer(gameId);
    }); 

    $(window).on('beforeunload', function () {
        var gameId = $("#hdnSelectedGameId").val();
        if(gameId != null)
            game.server.disconnectPlayer(gameId);
    });

});

function getAPIUrl() {
    return "http://localhost:1597/api/";
}

function showErrorMsg(title,msg) {
    $('#msgTitle').text(title);
    $('#msgBody').text(msg);
    $('#mdlMessage').modal('show');
}

var clPreloader = function () {

    $("html").addClass('cl-preload');

    $("#loader").fadeOut("slow", function () {
        $("#preloader").delay(400).fadeOut("slow");
    });
    $("html").removeClass('cl-preload');
    $("html").addClass('cl-loaded');

};

function showPreloader() {
    $("html").addClass('cl-preload');
}

function hidePreloader() {
    $("#loader").fadeOut("slow", function () {
        $("#preloader").delay(500).fadeOut("slow");
    });
    $("html").removeClass('cl-preload');
    $("html").addClass('cl-loaded');
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
                $("#hdnPlayerName").val(response.PlayerName);
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
                $("#hdnPlayerName").val(response.PlayerName);
                
                loadMainScreen("Join/Wait");
            }
        }
    };

    game.client.gameCreationFailed = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            showErrorMsg('Error', 'An error occurred while creating a game');
        }
    };

    game.client.invalidGameCode = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            showErrorMsg('Error', 'Invalid game code');
        }
    };

    game.client.gameJoinFailed = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "JOIN") {
            showErrorMsg('Error', 'An error occurred while joining a game');
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
                showPreloader();
                setTimeout(function () {
                    $("#mdlMessage").modal("hide");
                    $("#hdnScreen").val("GAME");
                    loadMainScreen("Game/GameBoard");
                    hidePreloader();
                }, 400);
            }, 800);
        }
    };

    game.client.setUpGameData = function (data) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            dataset = data;
        }
    };

    game.client.setUpSectors = function (data) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            sectors = data;
        }
    };

    game.client.allPlayersConnected = function () {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "WAIT") {
            $('#lblWaitingNotify').text('Please wait till we get things ready');
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
            showPreloader();
            setTimeout(function () {
                hidePreloader();
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
            $('#playerAccountBalance').text(response.Balance.toFixed(2));
            $('#bankWindowBalance').text(response.Balance.toFixed(2));
            $('#playerAllocatedBalance').text(response.AllocatedPrice.toFixed(2));
            $('#playerProfitBalance').text(response.ProfitPrice.toFixed(2));
        }
    };

    game.client.playerBoughtStock = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            addStockBoughtNews(response);
            getPlayerStock();
            getGameTransactions();
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
            $('#playerAccountBalance').text(response.Balance.toFixed(2));
            $('#bankWindowBalance').text(response.Balance.toFixed(2));
            $('#playerAllocatedBalance').text(response.AllocatedPrice.toFixed(2));
            $('#playerProfitBalance').text(response.ProfitPrice.toFixed(2));
        }
    };

    game.client.playerSoldStock = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            console.log(response);
            addStockSoldNews(response);
            getPlayerStock();
            getGameTransactions();
        }
    };

    game.client.stockSellFailed = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            $('#msgTitle').text('Unsuccessful');
            $('#msgBody').text(response.Message);
            $('#mdlMessage').modal('show');
        }
    };

    game.client.loadGameLeaders = function (response) {
        if ($("#hdnScreen").val() != undefined && $("#hdnScreen").val() == "GAME") {
            setUpLeaderBoard(response);
        }
    };
}