var clock;

function setUpStocks(data) {

    var currentTurn = data.Tun;
    $('#hdnCurrentTurn').val(currentTurn);
    $('#gameScrnCurntTurn').val(currentTurn);
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
                isIncreased: false
            };

            stock.sectorName = sector.Sector.SectorName;
            stock.sectorId = sector.Sector.SectorId;

            var tempStock = stocks[i];
            stock.stockId = tempStock.StockId;
            stock.stockName = tempStock.StockName;
            stock.price = tempStock.CurrentPrice;
            if (tempStock.CurrentPrice > tempStock.StartingPrice)
                stock.isIncreased = true;

            stocksList.push(stock);
        }
    }

    console.log(stocksList);
    if (currentTurn == 1) {
        setInterval(function () { loadStocksTable(stocksList); }, 10000);
    } else {
        loadStocksTable(stocksList);
    }
    
}

function loadStocksTable(stocks) {

    $(".stocksBuyIfo").bootstrapNews({
        newsPerPage: 5,
        autoplay: false,
        pauseOnHover: true,
        direction: 'down',
        newsTickerInterval: 4000,
        onToDo: function () {
            //console.log(this);
        }
    });

    $("#stockList > tbody").html("");
    for (var i = 0; i < stocks.length; i++) {
        var stock = stocks[i];
        NewRowToStocks(stock);
    }
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

    var newsStr = details.PlayerName + ' bought ' + details.Quantity + ' stocks of ' + details.StockName + ' at a price of Rs. ' + details.Price;
    var appendStr = '<li class="news-item"> <table cellpadding="4"><tr class="buy-stock">';
    appendStr += '<td>' + newsStr+'<td>';
    appendStr += '</tr> </table> </li>';

    $(".stocksBuyIfo").append(appendStr);
}

function getPlayerStock() {
    var gameId = $('#hdnGameId').val();
    var playerId = $('#hdnPlayerId').val();
    game.server.getPlayerStocks(gameId, playerId);

}

function loadPlayerStocksGrid(data) {
    $("#playerStocksList > tbody").html("");
    for (var i = 0; i < data.length; i++) {
        var stock = data[i];
        NewRowToPlayerStocks(stock);
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

function sellStockPopUp(sectorId,stockId,quantity,stockName) {
    $('#hdnSellStockId').val(stockId);
    $('#hdnSellSectorId').val(sectorId);
    $('#lblSellStockName').text(stockName);
    $('#txtSellStockQuantity').val(quantity);
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

    var newsStr = details.PlayerName + ' sold ' + details.Quantity + ' stocks of ' + details.StockName + ' at a price of Rs. ' + details.Price;
    var appendStr = '<li class="news-item"> <table cellpadding="4"><tr class="sell-stock">';
    appendStr += '<td>' + newsStr + '<td>';
    appendStr += '</tr> </table> </li>';

    $(".stocksBuyIfo").append(appendStr);
}