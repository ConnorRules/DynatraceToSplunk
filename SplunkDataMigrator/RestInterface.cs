using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SplunkDataMigrator
{
    class RestInterface
    {
        private string URL = "";
        private string urlParameters = "?type=XML&format=XML+Export";

        public string Get(string dashboard, string admin ="admin", string pass="admin", string server ="localhost")
        {
            HttpClient client = new HttpClient();
            //http://localhost:8020/rest/management/reports/create/easyTravel%20Purepaths?type=XML&format=XML+Export
            URL = "http://" + server + ":8020/rest/management/reports/create/" + dashboard.Replace(" ", "%20") + urlParameters;
            client.BaseAddress = new Uri(URL);

            //Set the authorization header according the the REST interface doc
            string AuthorizationValue = Base64Encode(admin + ":" + pass);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + AuthorizationValue);
           
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;

            var bodyString = response.Content.ReadAsStringAsync();

            return bodyString.Result;


        }


        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }


}

