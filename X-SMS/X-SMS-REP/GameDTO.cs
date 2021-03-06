﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X_SMS_REP
{
    public class GameDTO
    {
        public int GameId { get; set; }
        public string GameCode { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string CreatedPlayer { get; set; }
        public int PlayersCount { get; set; }
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCanceled { get; set; }
        public int CurrentRound { get; set; }
        public string Winner { get; set; }
        public int JoinedPlayerCount { get; set; }

        public bool IsPlayerAIAvailable { get; set; }
        public GameDetailDTO GameDetail { get; set; }
        public List<PlayerDTO> Players { get; set; }
    }
}
