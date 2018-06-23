using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_REP;

namespace X_SMS_DAL.Global
{
    public class GameDataManager
    {
        public static readonly int noOfTurns = 10;
        public static Dictionary<int, GameDetailDTO> gameDetails = new Dictionary<int, GameDetailDTO>();
     
    }
}
