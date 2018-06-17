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
    public class TrendService:IDisposable
    {
        private static readonly int noOfTurns = 10;

        private static readonly int maxTrend = 3;
        private static readonly int minTrend = -3;

        private static readonly int maxRandomTrend = 2;
        private static readonly int minRandomTrend = -2;

        private static readonly List<String> chances = new List<string>
        {
            "MIN",
            "MAX",
            "SAME",
            "SAME"
        };

        private XSmsEntities trendEntities = null;

        private Random randNextPossibleChance = new Random();

        public TrendService()
        {
            trendEntities = new XSmsEntities();
        }

        public int[] GenerateMarketTrends()
        {
            int[] marketTrends = new int[noOfTurns];

            try
            {
                for (int i = 0; i < noOfTurns; i++)
                {
                    if (i == 0)
                    {
                        marketTrends[i] = CalculateTrendValue(0);
                    }
                    else
                    {
                        marketTrends[i] = CalculateTrendValue(marketTrends[i - 1]);
                    }
                }

            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return marketTrends;
        }

        public Dictionary<int, int>[] GenerateSectorTrends()
        {
            Dictionary<int, int>[] sectorTrends = new Dictionary<int, int>[noOfTurns];
            var sectors = trendEntities.Sectors.ToList();
            Dictionary<int, int> trendsBysectors = new Dictionary<int, int>();
            try
            {
                for (int i = 0; i < noOfTurns; i++)
                {
                    if (i == 0)
                    {
                        trendsBysectors.Clear();
                        foreach (var sector in sectors)
                        {
                            trendsBysectors.Add(sector.SectorId, CalculateTrendValue(0));
                        }
                        sectorTrends[i] = new Dictionary<int, int>(trendsBysectors);
                    }
                    else
                    {
                        trendsBysectors.Clear();
                        foreach (var sector in sectors)
                        {
                            var q = sector.SectorName;
                            var w = sectorTrends[0][sector.SectorId];
                            trendsBysectors.Add(sector.SectorId, CalculateTrendValue(sectorTrends[i - 1][sector.SectorId]));
                        }
                        sectorTrends[i] = new Dictionary<int, int>(trendsBysectors);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return sectorTrends;
        }

        public int[] GenerateRandomTrends()
        {
            int[] randomTrends = new int[noOfTurns];

            try
            {
                Random randomNo = new Random();
                for (int i = 0; i < noOfTurns; i++)
                {
                    randomTrends[i] = randomNo.Next(minRandomTrend, (maxRandomTrend + 1));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return randomTrends;
        }

        private int CalculateTrendValue(int currentTrendValue)
        {
            int maxVal = 0;
            int midVal = 0;
            int minVal = 0;

            String chance = null;

            if ((currentTrendValue != maxTrend) && (currentTrendValue != minTrend))
            {
                maxVal = currentTrendValue + 1;
                midVal = currentTrendValue;
                minVal = currentTrendValue - 1;
                //option: all
                chance=GetNextPossibleChance(0);
            }
            else if ((currentTrendValue == maxTrend))
            {
                midVal = currentTrendValue;
                minVal = currentTrendValue - 1;
                //option: without max
                chance = GetNextPossibleChance(1);
            }
            else if((currentTrendValue == minTrend))
            {
                maxVal = currentTrendValue + 1;
                midVal = currentTrendValue;
                //option: without min
                chance = GetNextPossibleChance(2);
            }

            return (chance != null && chance == "MAX") ? maxVal :
                ((chance != null && chance == "MIN") ? minVal :
                ((chance != null && chance == "SAME") ? midVal : 0));
        }

        private String GetNextPossibleChance(int option)
        {
            String chance = null;
            List<String> tempChances = new List<String>(chances);
            
            switch (option)
            {
                case 0:
                    tempChances = (List<String>)Shuffle(tempChances);
                    chance = tempChances[randNextPossibleChance.Next(tempChances.Count)];
                    break;
                case 1:
                    //without max value
                    tempChances.Remove("MAX");
                    tempChances = (List<String>)Shuffle(tempChances);
                    chance = tempChances[randNextPossibleChance.Next(tempChances.Count)];
                    break;
                case 2:
                    //without min value
                    tempChances.Remove("MIN");
                    tempChances = (List<String>)Shuffle(tempChances);
                    chance = tempChances[randNextPossibleChance.Next(tempChances.Count)];
                    break;
                default:
                    break;
            }
            return chance;
        }

        private List<T> Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
        public void Dispose()
        {
           
        }
    }
}
