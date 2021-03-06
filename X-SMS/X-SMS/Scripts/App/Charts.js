﻿var noOfTurns = 10;
var dataset;
var sectors;

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
                backgroundColor: color(window.chartColors.blue).alpha(0.5).rgbString(),
                borderColor: window.chartColors.blue,
                borderWidth: 1,
                data: stockValues
            }]

        };

        var ctx = document.getElementById('barCanvas').getContext('2d');
        if (window.bar != undefined)
            window.bar.destroy();

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

function generateTrendChart(currentTurn) {
    if (currentTurn <= noOfTurns) {
        //generate lables
        var labels = [];
        for (i = 0; i < currentTurn; i++) {
            labels[i] = i + 1;
        }

        var config = {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Random Trend',
                    backgroundColor: window.chartColors.red,
                    borderColor: window.chartColors.red,
                    data: dataset.RandomTrend.slice(0, currentTurn),
                    fill: false,
                }, {
                    label: 'Market Trend',
                    fill: false,
                    backgroundColor: window.chartColors.blue,
                    borderColor: window.chartColors.blue,
                    data: dataset.MarketTrend.slice(0, currentTurn),
                }, {
                    label: 'Event',
                    fill: false,
                    backgroundColor: window.chartColors.green,
                    borderColor: window.chartColors.green,
                    data: getEventArr(currentTurn),
                }]
            },
            options: {
                responsive: true,
                title: {
                    display: true,
                    text: 'Trend and Event Chart'
                },
                tooltips: {
                    mode: 'index',
                    intersect: false,
                },
                hover: {
                    mode: 'nearest',
                    intersect: true
                },
                scales: {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Turn'
                        }
                    }],
                    yAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Value'
                        }
                    }]
                }
            }
        };

        var ctx = document.getElementById('canvas').getContext('2d');
        if (window.myLine != undefined)
            window.myLine.destroy();
        window.myLine = new Chart(ctx, config);

    }
}

function getEventArr(currentTurn) {
    var eventDataset = dataset.EventDetail;
    var eventDataArr = [];

    for (i = 0; i < currentTurn; i++) {
        if (eventDataset[i] == null) {
            eventDataArr[i] = 0;
        } else {
            eventDataArr[i] = eventDataset[i].Effect;
        }
    }
    return eventDataArr;
}


function generateSectorChart(currentTurn) {    
    if (currentTurn <= noOfTurns) {
        //generate lables
        var labels = [];
        for (i = 0; i < currentTurn; i++) {
            labels[i] = i + 1;
        }

        var sectorTrends = dataset.SectorTrend;

        var config = {
            type: 'line',
            data: {
                labels: labels,
                datasets: []
            },
            options: {
                responsive: true,
                title: {
                    display: true,
                    text: 'Sector Trend Chart'
                },
                tooltips: {
                    mode: 'index',
                    intersect: false,
                },
                hover: {
                    mode: 'nearest',
                    intersect: true
                },
                scales: {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Turn'
                        }
                    }],
                    yAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Value'
                        }
                    }]
                }
            }
        };

        var ctx = document.getElementById('sectorCanvas').getContext('2d');
        if (window.myLine1 != undefined)
            window.myLine1.destroy();
        window.myLine1 = new Chart(ctx, config);

        var colorNames = Object.keys(window.chartColors);
        var currentMinValue = 999;
        var currentMaxValue = -999;
        for (i = 0; i < sectorTrends.length; i++) {

            var sectorTrendsKeys = Object.keys(sectorTrends[i]);
            for (k = 0; k < sectorTrendsKeys.length; k++) {

                var sectorName = getSectorById(sectorTrendsKeys[k]);
                var dataArray = getDataArray(sectorTrendsKeys[k]);

                var colorName = colorNames[config.data.datasets.length % colorNames.length];
                var newColor = window.chartColors[colorName];
                var newDataset = {
                    label: sectorName,
                    backgroundColor: newColor,
                    borderColor: newColor,
                    data: dataArray,
                    fill: false
                };

                for (var index = 0; index < config.data.labels.length; ++index) {
                    newDataset.data.push(randomScalingFactor());
                }

                config.data.datasets.push(newDataset);
                window.myLine1.update();

                if (currentMinValue > (Math.min.apply(this, dataArray.slice(0, currentTurn)))) {
                    currentMinValue = Math.min.apply(this, dataArray.slice(0, currentTurn));
                }
                if (currentMaxValue < (Math.max.apply(this, dataArray.slice(0, currentTurn)))) {
                    currentMaxValue = Math.max.apply(this, dataArray.slice(0, currentTurn));
                }

                myLine1.options.scales = {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Turn'
                        }
                    }],
                    yAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Value'
                        },
                        ticks: {
                            min: currentMinValue,
                            max: currentMaxValue + 0.5
                        }
                    }]
                }
                myLine1.update();

            }
        }
    }
}

function getSectorById(id) {
    var sectorName = "";
    for (i = 0; i < sectors.length; i++) {
        if (sectors[i].SectorId == id) {
            sectorName = sectors[i].SectorName;
        }
    }
    return sectorName;
}

function getDataArray(key) {
    var sectorTrends = dataset.SectorTrend;
    var data = [];
    for (i = 0; i < sectorTrends.length; i++) {
        var sectorHashMap = sectorTrends[i];
        data[i] = sectorHashMap[key];
    }
    return data;
}

function generateStockValueChart(stocksArr) {

    var datasetStock = stocksArr;
    //generate lables
    var labels = [];
    for (i = 0; i < datasetStock[0].PriceList.length; i++) {
        labels[i] = i + 1;
    }

    var config = {
        type: 'line',
        data: {
            labels: labels,
            datasets: []
        },
        options: {
            responsive: true,
            title: {
                display: true,
                text: 'Stock Value Chart'
            },
            tooltips: {
                mode: 'index',
                intersect: false,
            },
            hover: {
                mode: 'nearest',
                intersect: true
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Turn'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Value'
                    }
                }]
            }
        }
    };

    var ctx = document.getElementById('stockCanvas').getContext('2d');
    if (window.myLine4 != undefined)
        window.myLine4.destroy();
    window.myLine4 = new Chart(ctx, config);

    var colorNames = Object.keys(window.chartColors);

    for (j = 0; j < datasetStock.length; j++) {

        var stockName = datasetStock[j].StockName;
        var stockValue = datasetStock[j].PriceList;

        var colorName = colorNames[config.data.datasets.length % colorNames.length];
        var newColor = window.chartColors[colorName];
        var newDataset = {
            label: stockName,
            backgroundColor: newColor,
            borderColor: newColor,
            data: stockValue,
            fill: false
        };

        config.data.datasets.push(newDataset);
        window.myLine4.update();

    }
}