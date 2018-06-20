var historyTable;
$(document).ready(function () {
    loadGameHistoryList();

    historyTable = $('#gameHistoyTable').DataTable({
        select: true
    });
});

function loadGameHistoryList() {
    $.ajax({
        type: "GET",
        dataType: 'json',
        url: getAPIUrl() + "History/GetGameList",
        success: function (res) {
            drawGameHistoryTable(res);
        }
    });
}

function drawGameHistoryTable(dataList) {
    dataList.forEach(function (entry) {
        if (entry !== null) {
            console.log(entry);
            historyTable.row.add([entry.GameId, entry.CreatedPlayer, entry.PlayersCount, entry.EndTime, entry.Winner]).draw();
        }
    });
}
