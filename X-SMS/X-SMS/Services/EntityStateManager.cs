using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X_SMS_REP;

namespace X_SMS.Services
{
    public static class EntityStateManager
    {
        public static readonly int NumberOfRounds = 5;
        public static List<GameDTO> CurrentGames = new List<GameDTO>();
        public static List<PlayerDTO> Players = new List<PlayerDTO>();
    }
}