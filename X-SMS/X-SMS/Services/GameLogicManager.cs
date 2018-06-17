using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X_SMS_REP;

namespace X_SMS.Services
{
    public class GameLogicManager
    {
        public GameDetail GetGameData(int gameId)
        {
            GameDetail returnObj = new GameDetail();
            try
            {
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakeGetRequest("api/trend?gameId=" + gameId.ToString());
                    ResultToken result = apiClient.ConvertObjectToToken(temp);
                    if (result.Success && result.Data != null)
                    {
                        returnObj = JsonConvert.DeserializeObject<GameDetail>(result.Data.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex);
            }

            return returnObj;
        }
    }
}