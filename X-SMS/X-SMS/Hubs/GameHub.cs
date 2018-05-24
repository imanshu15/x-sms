using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using X_SMS.Services;
using X_SMS_REP;

namespace X_SMS.Hubs
{
    public class GameHub : Hub
    {
        public static List<Client> _clients = new List<Client>();

        private object _syncRoot = new object();

        public void CreateGame(string playerName,int playerCount,bool isPrivate) {
           
            lock (_syncRoot) {

                var client = _clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

                if (client == null) {
                    client = new Client { ConnectionId = Context.ConnectionId, Name = playerName };
                    _clients.Add(client);
                }

                GameDTO game = new GameDTO();
                game.CreatedPlayer = playerName;
                game.PlayersCount = playerCount;
                game.IsPublic = !isPrivate;
                using (APIService apiClient = new APIService())
                {
                    var temp = apiClient.MakePostRequest("api/Game", game);
                    ResultToken result = apiClient.ConvertObjectToToken(temp);
                    if (result.Success && result.Data != null)
                    {
                        client.Game = JsonConvert.DeserializeObject<GameDTO>(result.Data.ToString());
                        client.IsPlaying = true;
                    }
                    else
                    {
                        _clients.Remove(client);
                    }
                    Clients.Client(Context.ConnectionId).gameCreated(result);
                    Clients.AllExcept(Context.ConnectionId).updateGameList();
                }


            }
        }

    }
}