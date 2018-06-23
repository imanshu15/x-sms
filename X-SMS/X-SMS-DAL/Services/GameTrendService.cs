using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Database;
using X_SMS_DAL.Global;
using X_SMS_DAL.Mapper;
using X_SMS_REP;

namespace X_SMS_DAL.Services
{
    public class GameTrendService:IDisposable
    {
       
        
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
                if (!GameDataManager.gameDetails.ContainsKey(gameId))
                {
                    GameDetailDTO gameDetail = new GameDetailDTO();
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

                    GameDataManager.gameDetails.Add(gameId, gameDetail);
                }
                result.Message = "Success! Game Turn Details Generated Successfully.";
                result.Data = (GameDetailDTO)GameDataManager.gameDetails[gameId];
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        private List<TurnDetail> calculateTurnScore(GameDetailDTO gameDetail)
        {
            List<TurnDetail> turnDetails = new List<TurnDetail>();
            int[] randomTrend = (int[])gameDetail.RandomTrend;
            int[] marketTrend = (int[])gameDetail.MarketTrend;
            Dictionary<int, int>[] sectorTrend = (Dictionary<int, int>[])gameDetail.SectorTrend;
            EventDetail[] eventTrend = (EventDetail[])gameDetail.EventDetail;

            var sectors = eventEntities.Sectors.ToList();

            for (int i = 0; i < GameDataManager.noOfTurns; i++)
            {
                TurnDetail tempTurn = new TurnDetail();
                tempTurn.Turn = i + 1;
                List<SectorDetail> sectoreDetailList = new List<SectorDetail>();
                foreach (var st in sectorTrend[i])
                {
                    SectorDetail sectoreDetail = new SectorDetail();

                    var tempSector = sectors.FirstOrDefault(x => x.SectorId == st.Key);

                    sectoreDetail.Sector = Mapping.Mapper.Map < SectorDTO > (tempSector);

                    var value=(st.Value + randomTrend[i] + marketTrend[i] + (((eventTrend[i] != null)&& ((eventTrend[i].IsStock) || ((eventTrend[i].IsSector) && (eventTrend[i].SectorId == sectoreDetail.Sector.SectorId)))) ? eventTrend[i].Effect : 0));
                    sectoreDetail.Score = value<0?0:value;

                    var stocks = eventEntities.Stocks.Where(a => a.SectorId == sectoreDetail.Sector.SectorId).ToList();
                    List<StockDetail> stockDetailList = new List<StockDetail>();
                    foreach (var stock in stocks)
                    {
                        StockDetail stockDetail = new StockDetail();
                        stockDetail.StockId = stock.StockId;
                        stockDetail.StockName = stock.StockName;
                        if (i == 0)
                        {
                            stockDetail.StartingPrice = stock.StartingPrice;
                        }
                        else
                        {
                            var tempObj = turnDetails.FirstOrDefault(z => z.Turn == i);
                            stockDetail.StartingPrice = tempObj.Sectors.FirstOrDefault(x => x.Sector.SectorId == sectoreDetail.Sector.SectorId).Stocks.FirstOrDefault(y => y.StockId == stockDetail.StockId).CurrentPrice;
                        }
                        stockDetail.CurrentPrice = Decimal.Round(Decimal.Add(stockDetail.StartingPrice, Decimal.Multiply(stockDetail.StartingPrice, Decimal.Divide(sectoreDetail.Score, 100))), 2);
                        stockDetailList.Add(stockDetail);
                    }
                    sectoreDetail.Stocks = new List<StockDetail>(stockDetailList);
                    sectoreDetailList.Add(sectoreDetail);
                }
                tempTurn.Sectors = sectoreDetailList;

                turnDetails.Add(tempTurn);
            }
            return turnDetails;
        }

        public void Dispose()
        {
           
        }
    }
}
