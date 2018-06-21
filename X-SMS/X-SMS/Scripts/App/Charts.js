var noOfTurns = 10;

function generateBarChart(currentTurn, stockValues, stockName) {
    if (currentTurn <= noOfTurns) {
        //generate lables
        var labels = [];
        for (i = 0; i < currentTurn; i++) {
            labels[i] = i + 1;
        }
        var color = Chart.helpers.color;

        var barChartData = {
            labels: labels,
            datasets: [{
                label: stockName,
                backgroundColor: color(window.chartColors.red).alpha(0.5).rgbString(),
                borderColor: window.chartColors.red,
                borderWidth: 1,
                data: stockValues
            }]

        };

        var ctx = document.getElementById('barCanvas').getContext('2d');
        window.myBar = new Chart(ctx, {
            type: 'bar',
            data: barChartData,
            options: {
                responsive: true,
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Stock Value Chart'
                }
            }
        });
    }
}
