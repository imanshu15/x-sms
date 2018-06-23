var historyTable;
$(document).ready(function () {
    loadGameHistoryList();

    historyTable = $('#gameHistoyTable').DataTable({
        select: true
    });

    setUpHistoryTableSelect();
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
            historyTable.row.add([entry.GameId, entry.CreatedPlayer, entry.PlayersCount, entry.EndTime, entry.Winner]).draw();
        }
    });
}

function setUpHistoryTableSelect() {
    $('#gameHistoyTable tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            historyTable.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
            var gameId = historyTable.rows('.selected').data()[0][0];
            window.location.href = '../Game/Summary?gameId=' + gameId;
        }
    });
}
