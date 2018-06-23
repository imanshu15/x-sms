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
    public class AnalystService : IDisposable
    {
        private static readonly int noOfTurns = 10;
        private XSmsEntities eventEntities = null;
        private Random rnd = new Random();

        public AnalystService()
        {
            eventEntities = new XSmsEntities();
        }

        public List<AnalystDetail> getRecommendations(int gameId, int currentTurn)
        {
            GameDetailDTO gameDetail = GameDataManager.gameDetails[gameId];
            List<AnalystDetail> analystDetailList = new List<AnalystDetail>();
            analystDetailList = calculateValue(gameDetail, currentTurn);
            return analystDetailList;
        }

        private List<AnalystDetail> calculateValue(GameDetailDTO gameDetail, int currentTurn)
        {
            List<AnalystDetail> analystDetailList = new List<AnalystDetail>();
            string type = "";
            string name = "";
            int id = 0;
            //check access to ternd value (50%)
            //gives 0 or 1
            int randomNo = rnd.Next(0, 2);
            if (randomNo == 1)
            {
                //has access to trend values
                Dictionary<int, int>[] sectorTrends = gameDetail.SectorTrend;
                int[] marketTrends = gameDetail.MarketTrend;
                int[] randomTrends = gameDetail.RandomTrend;

                int maxTurn = (currentTurn - 1) + 10;
                if (maxTurn > noOfTurns)
                {
                    maxTurn = noOfTurns;
                }

                int[] scoreValues = new int[(maxTurn - (currentTurn - 1))];
                int count = 0;
                int value = 0;

                //check if it is a sector or stock recommendation
                //gives 0 or 1
                int rand = rnd.Next(0, 2);
                if (rand == 1)
                {
                    //sector
                    //get random sector
                    var sectors = eventEntities.Sectors.ToList();
                    Sector sector = sectors[rnd.Next(sectors.Count)];

                    //if currentTurn=1 -> index=0
                    for (int i = (currentTurn - 1); i < maxTurn; i++)
                    {
                        type = "sector";
                        name = sector.SectorName;
                        id = sector.SectorId;
                        //get sector trends from gamedetails
                        Dictionary<int, int> sectorTrend = sectorTrends[i];
                        int sectorTrendValue = sectorTrend[sector.SectorId];

                        //get market trends from gamedetails
                        int marketTrendValue = marketTrends[i];

                        //get random trends from gamedetails
                        int randomTrendValue = randomTrends[i];

                        //check access to event value (60%)
                        //get event from gamedetails
                        int eventValue = getEventValueBySector(gameDetail, i, sector.SectorId);

                        value = sectorTrendValue + marketTrendValue + randomTrendValue + eventValue;
                        // analystDetail.score = value;
                        scoreValues[count] = value;
                        count++;
                    }
                }
                else
                {
                    //stock
                    //get random stock
                    var stocks = eventEntities.Stocks.ToList();
                    Stock stock = stocks[rnd.Next(stocks.Count)];

                    //if currentTurn=1 -> index=0
                    for (int i = (currentTurn - 1); i < maxTurn; i++)
                    {
                        type = "stock";
                        name = stock.StockName;
                        id = stock.StockId;
                        //get sector trends from gamedetails
                        Dictionary<int, int> sectorTrend = sectorTrends[i];
                        int sectorTrendValue = sectorTrend[stock.SectorId];

                        //get market trends from gamedetails
                        int marketTrendValue = marketTrends[i];

                        //get random trends from gamedetails
                        int randomTrendValue = randomTrends[i];

                        //check access to event value (60%)
                        //get event from gamedetails
                        int eventValue = getEventValueByStock(gameDetail, i);

                        value = sectorTrendValue + marketTrendValue + randomTrendValue + eventValue;
                        scoreValues[count] = value;
                        count++;
                    }
                }

                for (int i = 0; i < scoreValues.Length;)
                {
                    AnalystDetail analystDetail = new AnalystDetail();
                    analystDetail.Type = type;
                    analystDetail.Name = name;
                    analystDetail.Id = id;
                    analystDetail.Score = scoreValues[i];
                    analystDetail.Turn = currentTurn + i;

                    if (scoreValues[i] <= 0)
                    {
                        analystDetail.Action = "SELL";
                    }
                    else
                    {
                        analystDetail.Action = "BUY";
                    }

                    int duration = 1;

                    for (int j = (i + 1); j < scoreValues.Length; j++)
                    {
                        if (scoreValues[i] == scoreValues[j])
                        {
                            duration++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    analystDetail.Duration = duration;

                    analystDetailList.Add(analystDetail);
                    i = i + duration;
                }

            }
            else
            {
                //no access to trend values
                analystDetailList = null;
            }
            return analystDetailList;
        }

        private int getEventValueBySector(GameDetailDTO gameDetail, int currentTurn, int sectorId)
        {
            int eventValue = 0;
            int randomNo = rnd.Next(1, 11);
            if (randomNo <= 6)
            {
                //has access to event value
                EventDetail[] eventDetails = gameDetail.EventDetail;
                if (eventDetails[currentTurn] != null)
                {
                    EventDetail eventDetail = eventDetails[currentTurn];
                    if ((eventDetail.IsSector) && (eventDetail.SectorId == sectorId))
                    {
                        eventValue = eventDetail.Effect;
                    }
                }
            }
            return eventValue;
        }

        private int getEventValueByStock(GameDetailDTO gameDetail, int currentTurn)
        {
            int eventValue = 0;
            int randomNo = rnd.Next(1, 11);
            if (randomNo <= 6)
            {
                //has access to event value
                EventDetail[] eventDetails = gameDetail.EventDetail;
                if (eventDetails[currentTurn] != null)
                {
                    EventDetail eventDetail = eventDetails[currentTurn];
                    eventValue = eventDetail.Effect;
                }
            }
            return eventValue;
        }

        public void Dispose()
        {

        }
    }
}
