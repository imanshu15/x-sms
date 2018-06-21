var clock;

function setUpStocks(data) {

    var currentTurn = data.Turn;
    if (currentTurn != 1) {
        getPlayerStock();
    }
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
            stock.price = tempStock.CurrentPrice;
            stock.previousPrice = tempStock.StartingPrice;
            if (tempStock.CurrentPrice > tempStock.StartingPrice)
                stock.isIncreased = true;

            if (stock.isIncreased) {
                stock.percentage = (tempStock.CurrentPrice / tempStock.StartingPrice) * 100;
                stock.percentage = stock.percentage.toFixed(2);
            }
            stocksList.push(stock);
        }
    }

    loadStocksTable(stocksList);
    console.log(stocksList);
    //if (currentTurn == 1) {
    //    setInterval(function () { loadStocksTable(stocksList); }, 10000);
    //} else {
        
    //}
    
}

function loadStocksTable(stocks) {

    //$(".stocksBuyIfo").bootstrapNews({
    //    newsPerPage: 5,
    //    autoplay: false,
    //    pauseOnHover: true,
    //    direction: 'down',
    //    newsTickerInterval: 4000,
    //    onToDo: function () {
    //        //console.log(this);
    //    }
    //});

    $("#stockMarketTable > tbody").html("");
    for (var i = 0; i < stocks.length; i++) {
        var stock = stocks[i];
        AddToStocksTable(stock);
    }
}

function AddToStocksTable(stock) {

    var appendStr = '<tr class="table-row"><td> <div class="stock-img" > <img class="avatar" alt="Alphabet" src="https://etoro-cdn.etorostatic.com/market-avatars/goog/150x150.png"></div>'
        + '<div class="stock-info"><span class="stock-name">' + stock.stockName + '</span>'
        + '<span class="sector-name">' + stock.sectorName + '</span></div></td><td><div class="red ">'
        + '<span class="change-num-amount ">' + stock.percentage + ' %</span></div></td><td>'
        + '<div class="stock-trade-button"><div class="left-price-name">P</div><div class="price-value"><span>' + stock.previousPrice + '</span> </div></div></td>'
        + '<td><div class="stock-trade-button"><div class="left-price-name">C</div><div class="price-value"><span>' + stock.price + '</span> </div></div ></td>'
        + '<td style="text-align:center;"><i class="fa fa-arrow-up green" style="font-size:20px;top:5px;"></i></td>'
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
        async: false,
        success: function (res) {
                data = res;
        }
    });
    console.log(data);
    generateBarChart(turn, data.PriceList, data.StockName);
    $('#mdlStockChart').modal('show');
}

function NewRowToStocks(stock) {
    console.log(stock);
    var table = document.getElementById("stockList");
    var row = table.insertRow(0);
    var SectorId = row.insertCell(0);
    var Sector = row.insertCell(1);
    var StockId = row.insertCell(2);
    var StockName = row.insertCell(3);
    var Price = row.insertCell(4);
    var Isincreased = row.insertCell(5);
    var Buy = row.insertCell(6);
    SectorId.innerHTML = stock.sectorId;
    Sector.innerHTML = stock.sectorName;
    StockId.innerHTML = stock.stockId;
    StockName.innerHTML = stock.stockName;
    Price.innerHTML = stock.price;
    Isincreased.innerHTML = stock.isIncreased;

    var buyStr = '<button onclick="buyStockPopUp(' + stock.sectorId + ',' + stock.stockId + ',\'' + stock.stockName+'\')"> Buy </button>'
    Buy.innerHTML = buyStr;
}

function buyStockPopUp(sectorId,stockId,stockName) {
    $('#hdnBuyStockId').val(stockId);
    $('#hdnBuySectorId').val(sectorId);
    $('#lblBuyStockName').text(stockName);
    $('#mdlBuyStock').modal('show');
}

function buyStocks() {
    debugger
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

function NewRowToPlayerStocks(stock) {
    console.log(stock);
    var table = document.getElementById("playerStocksList");
    var row = table.insertRow(0);
    var SectorId = row.insertCell(0);
    var Sector = row.insertCell(1);
    var StockId = row.insertCell(2);
    var StockName = row.insertCell(3);
    var Quantity = row.insertCell(4);
    var BoughtPrice = row.insertCell(5);
    var CurrentPrice = row.insertCell(6);
    var Isincreased = row.insertCell(7);
    var Sell = row.insertCell(8);
    SectorId.innerHTML = stock[0].SectorId;
    Sector.innerHTML = stock[0].SectorName;
    StockId.innerHTML = stock[0].StockId;
    StockName.innerHTML = stock[0].StockName;
    Quantity.innerHTML = stock[0].Quantity;
    BoughtPrice.innerHTML = stock[0].BoughtPrice;
    CurrentPrice.innerHTML = stock[0].CurrentPrice;
    Isincreased.innerHTML = stock[0].IsIncreased;

    var sellStr = '<button onclick="sellStockPopUp(' + stock[0].SectorId + ',' + stock[0].StockId + ',' + stock[0].Quantity + ',\'' + stock[0].StockName + '\')"> Sell </button>'
    Sell.innerHTML = sellStr;
}

function AddToMyStocksTable(stock) {
    var appendStr = '<tr class="table-row"><td><div class="stock-img"><img class="avatar" alt="Alphabet" src="https://etoro-cdn.etorostatic.com/market-avatars/goog/150x150.png"></div>'
        + '<div class="stock-info"><span class="stock-name">' + stock[0].StockName + '</span><span class="sector-name">' + stock[0].SectorName + '</span></div></td>'
        + ' <td><div class="units"><span>' + stock[0].Quantity + '</span> </div></td>'
        + ' <td><div class="stock-trade-button"><div class="left-price-name">O</div><div class="price-value"><span>' + stock[0].BoughtPrice + '</span> </div></div></td>'
        + '<td><div class="stock-trade-button"><div class="left-price-name">C</div><div class="price-value"><span>' + stock[0].CurrentPrice + '</span> </div></div></td>'
        + '<td><div class="red"><span class="change-num-amount ">' + stock[0].Percentage + '</span></div></td>'
        + '<td style="text-align:center;"><i class="fa fa-arrow-up green" style="font-size:20px;top:5px;"></i></td>'
        + '<td><div class="red"><span class="profit-amount ">' + stock[0].Profit + '</span></div></td>'
        + ' <td style="text-align:center;"><div class="trade-button "><div class="sell-botton  d-inline victoria-sell" onclick = "showStockChart(' + stock[0].SectorId + ', ' + stock[0].StockId + ')"><span>VIEW</span> </div>'
        + '<td style="text-align:center;"><div class="trade-button "><div class="sell-botton d-inline victoria-buy" onclick="sellStockPopUp(' + stock[0].SectorId + ',' + stock[0].StockId + ',' + stock[0].Quantity + ',\'' + stock[0].StockName +'\')"><span> SELL </span> </div></div></td> </tr>';

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