$(document).ready(function () {

    var gameId = getUrlParameter('gameId');
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: getAPIUrl() + "Game/GetWinner?gameId=" + gameId,
        async: true,
        success: function (res) {
            drawWinnerBoard(res);
            setUpWinnerBoard(res[0]);
        }
    });

});


function drawWinnerBoard(data) {
    $("#winnerList > tbody").html("");
    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        addRowsToWinnerBoard(row, i + 1);
    }
}

function setUpWinnerBoard(winner) {
    var profit = winner.BankAccount.Balance - 1000;
    if (profit < 0)
        profit = 0.00;
    $('#lblPlayerNameDisplay').text(winner.PlayerName);
    $('#lblWinnerProfit').text(profit);
    $('#lblWinnerBalance').text(winner.BankAccount.Balance);
    $('#mdlWinner').modal('show');
}

function addRowsToWinnerBoard(row, rank) {
    var profit = ((row.BankAccount.Balance - 1000) / 1000) * 100;
    profit = profit.toFixed(2);

    var appendStr = '<tr class="winner-info" style="background-color:#3dbdec"><td>' + rank + '</td><td><center>'
        + '<div class="stock-img"><img class="avatar" alt="Alphabet" src="https://png.icons8.com/color/50/000000/person-male.png">'
        + '</div > <div class="stock-info"><span class="game-winner-name">' + row.PlayerName + '</span></div></center ></td >'
        + '<td> <div class="orders"><span> ' + row.NoOfTransactions + '</span> </div></td>';

    if (profit < 0) {
        appendStr += ' <td> <div class="profit loss-color"> <span class="profit-amount "> ' + profit + '% </span></div></td>';
    } else if (profit > 0) {
        appendStr += ' <td> <div class="profit profit-color"> <span class="profit-amount "> ' + profit + '% </span></div></td>';
    } else {
        appendStr += ' <td> <div class="profit neutral-color"> <span class="profit-amount "> ' + profit + '% </span></div></td>';
    }

    appendStr += '<td> <div class="score"><span>' + row.BankAccount.Balance + '</span> </div> </td> </tr>';

    $('#winnerList > tbody:last-child').append(appendStr);
}

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};