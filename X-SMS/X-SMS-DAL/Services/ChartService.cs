using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Global;
using X_SMS_REP;

namespace X_SMS_DAL.Services
{
    public class ChartService : IDisposable
    {
        public void Dispose()
        {
            
        }

        public ChartStock GetStocksValues(int gameId, int sectorId, int stockId, int turn)
        {
            var game = (GameDetailDTO)GameDataManager.gameDetails[gameId];
            var turnDetails = game.TurnDetail.Where(x => x.Turn <= turn).ToList();

            List<decimal> stockPrices = new List<decimal>();

            foreach (var tempTurn in turnDetails) {
                var tempStock = tempTurn.Sectors.FirstOrDefault(x => x.Sector.SectorId == sectorId).Stocks.FirstOrDefault(y => y.StockId == stockId);
                stockPrices.Add(tempStock.CurrentPrice);
            }

            var stock = turnDetails.FirstOrDefault().Sectors.FirstOrDefault(x => x.Sector.SectorId == sectorId).Stocks.FirstOrDefault(y => y.StockId == stockId);
            ChartStock chartStock = new ChartStock();
            chartStock.StockId = stock.StockId;
            chartStock.StockName = stock.StockName;
            chartStock.PriceList = stockPrices;

            return chartStock;
        }

        public List<ChartStock> GetStocksChartValues(int gameId, int turn)
        {
            List<ChartStock> stockCharts = new List<ChartStock>();
            var game = (GameDetailDTO)GameDataManager.gameDetails[gameId];
            var turnDetails = game.TurnDetail.Where(x => x.Turn <= turn).ToList();
            var tempTurn = turnDetails.FirstOrDefault();

            var sectors = tempTurn.Sectors.ToList();
            foreach (var sector in sectors) {
                var stocks = sector.Stocks;
                foreach (var stock in stocks) {
                    var chart = GetStocksValues(gameId, sector.Sector.SectorId, stock.StockId, turn);
                    stockCharts.Add(chart);
                }
            }

            return stockCharts;
        }

        public List<ChartStock> GetSectorStockValues(int gameId, int sectorId, int turn)
        {
            var game = (GameDetailDTO)GameDataManager.gameDetails[gameId];
            var turnDetails = game.TurnDetail.Where(x => x.Turn <= turn).ToList();

            List<ChartStock> sectorStocks = new List<ChartStock>();
            List<decimal> stockPrices = new List<decimal>();

            var tempStocks = turnDetails.FirstOrDefault().Sectors.FirstOrDefault(x => x.Sector.SectorId == sectorId).Stocks.ToList();

            foreach (var tempStk in tempStocks) {
                var chartStock = GetStocksValues(gameId, sectorId, tempStk.StockId, turn);
                sectorStocks.Add(chartStock);
            }

            return sectorStocks;
        }
    }
}
