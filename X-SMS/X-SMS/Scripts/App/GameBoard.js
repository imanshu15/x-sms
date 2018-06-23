var clock;

function setUpStocks(data) {

    var currentTurn = data.Turn;
    if (currentTurn != 1) {
        getPlayerStock();
    }
    else {
            $('#lblPlayerNameDisplay').text($('#hdnPlayerName').val());
    }
    generateSectorChart(currentTurn);
    generateTrendChart(currentTurn);
    updateStockChart(currentTurn);

    $('#hdnCurrentTurn').val(currentTurn);
    $('#gameScrnCurntTurn').text(currentTurn);
    var sectorList = data.Sectors;
    var stocksList = [];

    for (var x = 0; x < sectorList.length; x++) {
        var sector = sectorList[x];
        var stocks = sector.Stocks;

        for (var i = 0; i < stocks.length; i++) {

            var stock = {
                stockId: "",
                stockName: "",
                price: "",
                sectorId: "",
                sectorName: "",
                isIncreased: false,
                previousPrice: "",
                percentage:0
            };

            stock.sectorName = sector.Sector.SectorName;
            stock.sectorId = sector.Sector.SectorId;

            var tempStock = stocks[i];
            stock.stockId = tempStock.StockId;
            stock.stockName = tempStock.StockName;
            stock.price = tempStock.CurrentPrice.toFixed(2);
            stock.previousPrice = tempStock.StartingPrice.toFixed(2);
            if (tempStock.CurrentPrice > tempStock.StartingPrice)
                stock.isIncreased = true;

            if (stock.isIncreased) {
                stock.percentage = ((tempStock.CurrentPrice - tempStock.StartingPrice) / tempStock.StartingPrice) * 100;
                stock.percentage = stock.percentage.toFixed(2);
            }
            stocksList.push(stock);
        }
    }

    loadStocksTable(stocksList);  
}

function loadStocksTable(stocks) {
    $("#stockMarketTable > tbody").html("");
    for (var i = 0; i < stocks.length; i++) {
        var stock = stocks[i];
        AddToStocksTable(stock);
    }
}

function AddToStocksTable(stock) {

    var percentage = stock.percentage;
    if (percentage > 0) {
        percentageClass = 'profit-color';
    } else if (percentage < 0) {
        percentageClass = 'loss-color';
    } else {
        percentageClass = 'neutral-color ';
    }

    var profit = stock.price - stock.previousPrice;
    if (profit > 0) {
        profitClass = 'profit-color';
        profitClassIcon = 'fa fa-arrow-up';
    } else if (profit < 0) {
        profitClass = 'loss-color';
        profitClassIcon = 'fa fa-arrow-down';
    } else {
        profitClass = 'neutral-color';
        profitClassIcon = 'fa fa-arrows';
    }

    //<div class="stock-img" > <img class="avatar" alt="Alphabet" src="https://etoro-cdn.etorostatic.com/market-avatars/goog/150x150.png"></div>
    var appendStr = '<tr class="table-row"><td>'
        + '<div class="stock-info"><span class="stock-name">' + stock.stockName + '</span>'
        + '<span class="sector-name">' + stock.sectorName + '</span></div></td><td><div class="' + percentageClass +'">'
        + '<span class="change-num-amount ">' + stock.percentage + ' %</span></div></td><td>'
        + '<div class="stock-trade-button"><div class="left-price-name">P</div><div class="price-value"><span>' + stock.previousPrice + '</span> </div></div></td>'
        + '<td><div class="stock-trade-button"><div class="left-price-name">C</div><div class="price-value"><span>' + stock.price + '</span> </div></div ></td>'
        + '<td style="text-align:center;"><i class="' + profitClassIcon + ' ' + profitClass+'" style="font-size:20px;top:5px;"></i></td>'
        + ' <td style="text-align:center;"><div class="trade-button "><div class="sell-botton  d-inline victoria-sell" onclick = "showStockChart(' + stock.sectorId + ', ' + stock.stockId +')"><span>VIEW</span> </div>'
        + '<div class="sell-botton d-inline victoria-buy" onclick= "buyStockPopUp(' + stock.sectorId + ',' + stock.stockId + ',\'' + stock.stockName +'\')"><span>BUY</span> </div></div></td></tr>';

    $('#stockMarketTable > tbody:last-child').append(appendStr);
}

function showStockChart(sectorId,stockId) {
    var gameId = $('#hdnGameId').val();
    var turn = $('#hdnCurrentTurn').val();
    var data;
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: getAPIUrl() + "Chart/StockValues?gameId=" + gameId + "&sectorId=" + sectorId + "&stockId=" + stockId + '&turn=' + turn,
        async: true,
        success: function (res) {
            generateBarChart(turn, res.PriceList, data.StockName);
            $('#mdlStockChart').modal('show');
        }
    });

}

function buyStockPopUp(sectorId,stockId,stockName) {
    $('#hdnBuyStockId').val(stockId);
    $('#hdnBuySectorId').val(sectorId);
    $('#lblBuyStockName').text(stockName);
    $('#mdlBuyStock').modal('show');
}

function buyStocks() {
    $('#mdlBuyStock').modal('hide');
    var sectorId = $('#hdnBuySectorId').val();
    var stockId = $('#hdnBuyStockId').val();
    var quantity = $("#txtStockQuantity").val();
    var gameId = $('#hdnGameId').val();
    var playerId = $('#hdnPlayerId').val();

    game.server.buyStocks(gameId, playerId, sectorId, stockId, quantity);
    $('#hdnBuySectorId').val(-1);
    $('#hdnBuyStockId').val(-1);
    $("#txtStockQuantity").val(0);
}

function addStockBoughtNews(details) {

    var appendStr = '<li class="left clearfix"><span class="chat-img pull-left"><img src="~/Content/images/user-chat.png" alt="User Avatar" class="img-circle" /></span>';
    appendStr += '<div class="chat-body clearfix"><div class="header"><strong class="primary-font">' + details.PlayerName + '</strong></div>';
    appendStr += '<p class = "green">Type: Buy Stock: ' + details.StockName + ' StockPrice: ' + details.Price + ' Units:' + details.Quantity + ' Balance:' + details.PlayerAccBalance + ' </p> </div></li>';

    $("#stocksNewsFeed").prepend(appendStr);
}

function getPlayerStock() {
    var gameId = $('#hdnGameId').val();
    var playerId = $('#hdnPlayerId').val();
    game.server.getPlayerStocks(gameId, playerId);

}

function loadPlayerStocksGrid(data) {
    $("#myStockMarketTable > tbody").html("");
    for (var i = 0; i < data.length; i++) {
        var stock = data[i];
        AddToMyStocksTable(stock);
    }
}

function AddToMyStocksTable(stock) {

    var percentage = stock.Percentage;
    percentage = percentage.toFixed(2);

    var percentageClass = '';
    var profitClass = '';
    var profitClassIcon = '';

    if (percentage > 0) {
        percentageClass = 'profit-color';
    } else if (percentage < 0){
        percentageClass = 'loss-color';
    }else {
        percentageClass = 'neutral-color ';
    }

    var profit = stock.Profit;
    if (profit > 0) {
        profitClass = 'profit-color';
        profitClassIcon = 'fa fa-arrow-up';
    } else if (profit < 0) {
        profitClass = 'loss-color';
        profitClassIcon = 'fa fa-arrow-down';
    } else {
        profitClass = 'neutral-color';
        profitClassIcon = 'fa fa-arrows';
    }

    var appendStr = '<tr class="table-row"><td>'
        + '<div class="stock-info"><span class="stock-name">' + stock.StockName + '</span><span class="sector-name">' + stock.SectorName + '</span></div></td>'
        + ' <td><div class="units"><span>' + stock.Quantity + '</span> </div></td>'
        + ' <td><div class="stock-trade-button"><div class="left-price-name">O</div><div class="price-value"><span>' + stock.BoughtPrice + '</span> </div></div></td>'
        + '<td><div class="stock-trade-button"><div class="left-price-name">C</div><div class="price-value"><span>' + stock.CurrentPrice + '</span> </div></div></td>'
        + '<td><div class="' + percentageClass+'"><span class="change-num-amount ">' + percentage + '%</span></div></td>'
        + '<td style="text-align:center;"><i class="' + profitClassIcon + ' ' + profitClass +'" style="font-size:20px;top:5px;"></i></td>'
        + '<td><div class="' + profitClass+'"><span class="profit-amount ">' + stock.Profit + '</span></div></td>'
        + ' <td style="text-align:center;"><div class="trade-button "><div class="sell-botton  d-inline victoria-sell" onclick = "showStockChart(' + stock.SectorId + ', ' + stock.StockId + ')"><span>VIEW</span> </div>'
        + '<div class="trade-button "><div class="sell-botton d-inline victoria-buy" onclick="sellStockPopUp(' + stock.SectorId + ',' + stock.StockId + ',' + stock.Quantity + ',\'' + stock.StockName +'\')"><span> SELL </span> </div></div></td> </tr>';

    $('#myStockMarketTable > tbody:last-child').append(appendStr);
}

function sellStockPopUp(sectorId,stockId,quantity,stockName) {
    $('#hdnSellStockId').val(stockId);
    $('#hdnSellSectorId').val(sectorId);
    $('#lblSellStockName').text(stockName);
    $('#txtSellStockQuantity').val(quantity);
    $("txtSellStockQuantity").attr({
        "max": quantity,        // substitute your own
        "min": 1          // values (or variables) here
    });
    $('#mdlSellStock').modal('show');
}

function sellStocks() {

    $('#mdlSellStock').modal('hide');
    var sectorId = $('#hdnSellSectorId').val();
    var stockId = $('#hdnSellStockId').val();
    var quantity = $("#txtSellStockQuantity").val();
    var gameId = $('#hdnGameId').val();
    var playerId = $('#hdnPlayerId').val();

    game.server.sellStocks(gameId, playerId, sectorId, stockId, quantity);
}

function addStockSoldNews(details) {

    var appendStr = '<li class="left clearfix"><span class="chat-img pull-left"><img src="~/Content/images/user-chat.png" alt="User Avatar" class="img-circle" /></span>';
    appendStr += '<div class="chat-body clearfix"><div class="header"><strong class="primary-font">' + details.PlayerName +'</strong></div>';
    appendStr += '<p class = "red">Type: Sell Stock: ' + details.StockName + ' StockPrice: ' + details.Price + ' Units:' + details.Quantity + ' Balance:' + details.PlayerAccBalance +' </p> </div></li>';

    $("#stocksNewsFeed").prepend(appendStr);
}

function setUpLeaderBoard(data) {
    $("#leaderBoardList > tbody").html("");
    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        addRowToLeaderBoard(row, i + 1);
    }
}

function addRowToLeaderBoard(row,rank) {

    var profit = ((row.BankAccount.Balance - 1000) / 1000) * 100;
    profit = profit.toFixed(2);

    var appendStr = '<tr class="table-row"><td> <div class="rank">#<span>' + rank + '</span> </div> </td>'
        + ' <td><center><div class="stock-img"><img class="avatar" alt="Alphabet" src="https://png.icons8.com/color/50/000000/person-male.png"></div>'
        + '<div class="stock-info"><span class="stock-name">' + row.PlayerName + '</span></div> </center> </td>'
        + '<td> <div class="orders"><span> ' + row.NoOfTransactions + '</span> </div></td>';

    if (profit < 0) {
        appendStr += ' <td> <div class="profit loss-color"> <span class="profit-amount "> ' + profit + '% </span></div></td>';
    } else if (profit > 0) {
        appendStr += ' <td> <div class="profit profit-color"> <span class="profit-amount "> ' + profit + '% </span></div></td>';
    } else {
        appendStr += ' <td> <div class="profit neutral-color"> <span class="profit-amount "> ' + profit + '% </span></div></td>';
    }

    appendStr += '<td> <div class="score"><span>' + row.BankAccount.Balance +'</span> </div> </td> </tr>';

    $('#leaderBoardList > tbody:last-child').append(appendStr);
}

function getGameTransactions() {
    var gameId = $('#hdnGameId').val();
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: getAPIUrl() + "History/GetGameTransactions?gameId=" + gameId,
        success: function (res) {
            setUpTransactions(res);
        }
    });
}

function setUpTransactions(data) {
    var playerId = $("#hdnPlayerId").val();

    $('#gameTransactionsDiv').html("");

    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        if (row.PlayerId == playerId) {
            drawPlayerTransactions(row,true);
            break;
        }
    }

    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        if (row.PlayerId != playerId) {
            drawPlayerTransactions(row,false);
        }
    }
}

function drawPlayerTransactions(player,isCurrentPlayer) {

    var divId = "plyTrans" + player.PlayerId;

    var appendStr = '<div><div class=" navbar-light bg-light players-name-bar " data-toggle="collapse" data-target="#' + divId + '"'
        + '<span style=" margin-left:5px; ">' + player.PlayerName + '</span><span style=" margin-left:20px; ">' + player.Balance.toFixed(2) + '</span><i class="fas fa-chevron-down d-inline"></i></div>'
        + '<div id="' + divId + '" class="collapse"><table class=" table"><thead>'
        + '<tr class="table-head table-sm "><th scope="col" style="text-align:left;padding-left:14px;">Markets</th>'
        + '<th scope="col" class="col">Type</th><th scope="col" class="col">Units</th>'
        + '<th scope="col" class="col">Price</th><th scope="col" class="col">Cost</th>'
        +'<th scope="col" class="col">Action</th> </tr ></thead > <tbody>';

    var tableRows = '';
    var data = player.Transactions;

    if (isCurrentPlayer)
        $("#miniStatementList > tbody").html("");

    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        tableRows += drawrPlayerTransactionLine(row);
        if (isCurrentPlayer)
            drawBankTableRow(row,i+1);

    }

    appendStr += tableRows;
    appendStr += '</tbody></table></div></div>';

    $('#gameTransactionsDiv').append(appendStr);
}

function drawrPlayerTransactionLine(trans) {

    var transType = 'BUY';
    var transClass = 'profit-color';
    var units = trans.Quantity;

    if (trans.Quantity < 0) {
        transType = 'SELL';
        transClass = 'loss-color';
        units = -(trans.Quantity);
    }
    
    var appendStr = '<tr class="table-row"><td><div class="stock-info"><span class="stock-name">' + trans.StockName + '</span><span class="sector-name"> ' + trans.SectorName + '</span ></div></td>'
        + '<td><div class="' + transClass + '" style="padding-top:5px;"><span class="profit-amount ">' + transType + '</span></div></td><td><div class="units"><span>' + units + '</span> </div></td>'
        + '<td><div class="stock-trade-button"><div class="left-price-name">O</div><div class="price-value"><span> ' + trans.UnitPrice.toFixed(2) + ' </span> </div></div></td>'
        + '<td><div class="stock-trade-button"><div class="left-price-name">C</div><div class="price-value"><span> ' + trans.Amount.toFixed(2) + ' </span> </div> </div></td>'
        + '<td style="text-align:center;"> <div class="trade-button "><div class="sell-botton  d-inline victoria-sell" onclick="showStockChart(' + trans.SectorId + ', ' + trans.StockId + ')"><span>VIEW</span> </div></div></tr >';

    return appendStr;
}

function drawBankTableRow(row,id) {

    var transactionType = 'WITHDRAW';
    var transClass = 'profit-color';
    if (row.Quantity < 0) {
        transactionType = 'DEPOSIT';
        transClass = 'loss-color';
    }

    var appendStr = '<tr class="table-row"><td> <div class="rank"><span>' + id + '</span> </div> </td>'
        + ' <td><center><div class="stock-info transClass"><span class="stock-name">' + transactionType + '</span></div> </center> </td>'
        + '<td> <div class="orders"><span> ' + row.Amount.toFixed(2) + '</span> </div></td> </tr>';

    $('#miniStatementList > tbody:last-child').append(appendStr);
}

function updateStockChart(currentTurn) {

    var gameId = $('#hdnGameId').val();

    var data;
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: getAPIUrl() + "Chart/StockChartValues?gameId=" + gameId + '&turn=' + currentTurn,
        async: true,
        success: function (res) {
            generateStockValueChart(res);
        }
    });

}

