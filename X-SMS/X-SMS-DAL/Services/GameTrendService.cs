using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Database;
using X_SMS_DAL.Mapper;
using X_SMS_REP;

namespace X_SMS_DAL.Services
{
    public class GameTrendService:IDisposable
    {
        private static readonly int noOfTurns = 10;
        private static Dictionary<int, GameDetail> gameDetails = new Dictionary<int, GameDetail>();
        private XSmsEntities eventEntities = null;

        public GameTrendService()
        {
            eventEntities = new XSmsEntities();
        }

        public ResultToken GetGameTrendList(int gameId)
        {
            ResultToken result = new ResultToken();
            result.Success = true;
            try
            {
                if (!gameDetails.ContainsKey(gameId))
                {
                    GameDetail gameDetail = new GameDetail();
                    using (TrendService trendService = new TrendService())
                    {
                        gameDetail.RandomTrend = trendService.GenerateRandomTrends();
                        gameDetail.MarketTrend = trendService.GenerateMarketTrends();
                        gameDetail.SectorTrend = trendService.GenerateSectorTrends();
                    }
                        using (EventService eventService = new EventService())
                    {
                        gameDetail.EventDetail = eventService.GenerateEvents();
                    }
                    gameDetail.TurnDetail = calculateTurnScore(gameDetail);

                    gameDetails.Add(gameId, gameDetail);
                }
                result.Message = "Success! Game Turn Details Generated Successfully.";
                result.Data = (GameDetail)gameDetails[gameId];
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        private List<TurnDetail>[] calculateTurnScore(GameDetail gameDetail)
        {
            List<TurnDetail>[] turnDetails = new List<TurnDetail>[noOfTurns];
            int[] randomTrend = (int[])gameDetail.RandomTrend;
            int[] marketTrend = (int[])gameDetail.MarketTrend;
            Dictionary<int, int>[] sectorTrend = (Dictionary<int, int>[])gameDetail.SectorTrend;
            EventDetail[] eventTrend = (EventDetail[])gameDetail.EventDetail;

            var sectors = eventEntities.Sectors.ToList();

            for (int i = 0; i < noOfTurns; i++)
            {
                List<TurnDetail> turnDetailList = new List<TurnDetail>();
                foreach (var st in sectorTrend[i])
                {
                    TurnDetail turnDetail = new TurnDetail();

                    var tempSector = sectors.FirstOrDefault(x => x.SectorId == st.Key);

                    turnDetail.Sector = Mapping.Mapper.Map < SectorDTO > (tempSector);

                    var value=(st.Value + randomTrend[i] + marketTrend[i] + (((eventTrend[i] != null)&& ((eventTrend[i].IsStock) || ((eventTrend[i].IsSector) && (eventTrend[i].SectorId == turnDetail.Sector.SectorId)))) ? eventTrend[i].Effect : 0));
                    turnDetail.Score = value<0?0:value;

                    var stocks = eventEntities.Stocks.Where(a => a.SectorId == turnDetail.Sector.SectorId).ToList();
                    List<StockDetail> stockDetailList = new List<StockDetail>();
                    foreach (var stock in stocks)
                    {
                        StockDetail stockDetail = new StockDetail();
                        stockDetail.StockId = stock.StockId;
                        stockDetail.StockName = stock.StockName;
                        stockDetail.StartingPrice = stock.StartingPrice;
                        stockDetail.CurrentPrice = Decimal.Round(Decimal.Add(stock.StartingPrice, Decimal.Multiply(stock.StartingPrice, Decimal.Divide(turnDetail.Score,100))),2);
                        stockDetailList.Add(stockDetail);
                    }
                    turnDetail.Stocks = new List<StockDetail>(stockDetailList);
                    turnDetailList.Add(turnDetail);
                }
                turnDetails[i] = new List<TurnDetail>(turnDetailList);
            }
            return turnDetails;
        }

        public void Dispose()
        {
           
        }
    }
}
