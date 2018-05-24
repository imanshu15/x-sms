using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using X_SMS_REP;

namespace X_SMS.Services
{
    public class APIService : IDisposable
    {
        private string url;
        private static HttpClient client = null;

        public APIService() {
            url = WebConfigurationManager.AppSettings["ApiUrl"];
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public object MakeGetRequest(string transaction,string urlParameters) {

            object result = null;

            string requestStr = transaction + "?" + urlParameters;

            HttpResponseMessage response = client.GetAsync(requestStr).Result;

            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsAsync<object>();
            }
            else
            {
                Logger logger = LogManager.GetLogger("errorLogger");
                logger.Error(requestStr + " : " + response.StatusCode);
            }

            return result;
        }

        public ResultToken MakePostRequest(string transaction,object parameters)
        {

            ResultToken result = new ResultToken();

            try
            {
                var response =  client.PostAsJsonAsync(transaction, parameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<ResultToken>().Result;
                }
                else
                {
                    Logger logger = LogManager.GetLogger("errorLogger");
                    logger.Error(transaction + " : " + response.StatusCode);
                }

            }catch(Exception ex){
                Logger logger = LogManager.GetLogger("excpLogger");
                logger.Error(ex,transaction);
            }

            return result;
        }

        public ResultToken ConvertObjectToToken(object input) {
            return (ResultToken) input;
        }

        public void Dispose()
        {
            if (client != null) {
                client.Dispose();
                client = null;
            }
        }
    }
}