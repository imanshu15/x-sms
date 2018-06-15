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
    public class EventService:IDisposable
    {
        private static readonly int noOfTurns = 10;
        private XSmsEntities eventEntities = null;
        private static readonly Dictionary<String,int> eventType = new Dictionary<String, int>
        {
           { "SECTOR",33 },
           { "SOCK",67 },
        };

        public EventService()
        {
            eventEntities = new XSmsEntities();
        }

        public EventDetail[] GenerateEvents()
        {
            EventDetail[] events = new EventDetail[noOfTurns];
            Double eventProb = 0.0;

            try
            {
                for (int i = 0; i < noOfTurns; i++)
                {
                    if (i == 0)
                    {
                        //probablility of an event occure in the first turn = 0
                        events[i] = null;
                    }
                    else
                    {
                        //If pre turn doesn't have an event
                        if (events[i - 1] == null)
                        {
                            //probability increases by 0.1 for each turn that an event does not occur
                            eventProb += 0.1;
                            //Match probability with random numbers
                            int randProb = (new Random().Next(1, 10));
                            Double tempEventProb = Convert.ToInt32(eventProb * 10);
                            if (randProb <= tempEventProb)
                            {
                                int randomNo = new Random().Next(1, 101);
                                if (randomNo <= eventType["SECTOR"])
                                {
                                    events[i] = GenerateSectorEvent();
                                    for (int j = (i + 1); j < ((i + events[i].Duration)<=noOfTurns ? (i + events[i].Duration) : noOfTurns); j++)
                                    {
                                        events[j] = events[i];
                                    }
                                    i = i + events[i].Duration;
                                    eventProb = 0.0;
                                    continue;
                                }
                                else if (randomNo > eventType["SECTOR"])
                                {
                                    events[i] = GenerateStockEvent();
                                    for (int j = (i + 1); j < ((i + events[i].Duration) <= noOfTurns ? (i + events[i].Duration) : noOfTurns); j++)
                                    {
                                        events[j] = events[i];
                                    }
                                    i = i + events[i].Duration;
                                    eventProb = 0.0;
                                    continue;
                                }
                            }else
                            {
                                events[i] = null;
                            }

                        }else
                        {
                            //probablility of an event occure after an event has occurred = 0
                            events[i] = null;
                        }
                       
                    }
                }

            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return events;
        }

        private EventDetail GenerateSectorEvent()
        {
            var sectorEvents = eventEntities.Events.Where(a => a.IsSector == true).ToList();
            var eventDTOList = Mapping.Mapper.Map<List<EventDTO>>(sectorEvents);
            return GetNextPossibleEvent(GenerateEventChanceList(eventDTOList));
        }

        private EventDetail GenerateStockEvent()
        {
            var sectorEvents = eventEntities.Events.Where(a => a.IsStock == true).ToList();
            var eventDTOList = Mapping.Mapper.Map<List<EventDTO>>(sectorEvents);
            return GetNextPossibleEvent(GenerateEventChanceList(eventDTOList));
        }

        private EventDetail GetNextPossibleEvent(List<EventDTO> chances)
        {
            Random rand = new Random();
            chances = (List<EventDTO>)Shuffle(chances);
            EventDTO chance = chances[rand.Next(chances.Count)];

            EventDetail eventDetails = new EventDetail();
            eventDetails.EventId = chance.EventId;
            eventDetails.EventName = chance.EventName;
            eventDetails.IsSector = chance.IsSector;
            var sectors = eventEntities.Sectors.ToList();
            eventDetails.SectorId = eventDetails.IsSector ? sectors[new Random().Next(sectors.Count)].SectorId : 0;
            eventDetails.IsStock = chance.IsStock;
            eventDetails.Effect = new Random().Next(chance.MinEffect, chance.MaxEffect + 1);
            eventDetails.Duration = new Random().Next(chance.MinDuration, chance.MaxDuration + 1);

            return eventDetails;
        }

        private List<EventDTO> GenerateEventChanceList(List<EventDTO> eventDTOList)
        {
            List<EventDTO> chances = new List<EventDTO>();
           for(int i = 0; i < eventDTOList.Count; i++)
            {
                if (eventDTOList[i].Chance == 1)
                {
                    chances.Add(eventDTOList[i]);
                }
                else
                {
                    for (int j = 0; j < eventDTOList[i].Chance; j++)
                    {
                        chances.Add(eventDTOList[i]);
                    }
                }
            }
            return chances;
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
