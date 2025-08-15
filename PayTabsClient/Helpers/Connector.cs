using Newtonsoft.Json;
using PayTabsClient.Models;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;

namespace PayTabsClient.Helpers
{
    public class Connector
    {
        public Connector()
        {
        }

        public Transaction_Response Send(Transaction transaction)
        {
            string base_url = transaction.Endpoint; // "https://secure.paytabs.com/";
            string payment_url = base_url + "payment/request";

            string body = JsonConvert.SerializeObject(transaction);

            var client = new RestClient(payment_url, configureSerialization: s => s.UseNewtonsoftJson());
            //client.UseJson();
            //client.AddHandler("applications/json", )

            var request = new RestRequest(resource: payment_url, method: Method.Post);
            request.AddHeader("authorization", transaction.ServerKey);
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            //request.AddJsonBody(transaction);

            var response = client.Execute<Transaction_Response>(request);

            // Old RestSharp code
            // Transaction_Response tran_res = new JsonDeserializer().Deserialize<Transaction_Response>(response);
            // New RestSharp code
            //Transaction_Response tran_res = JsonConvert.DeserializeObject<Transaction_Response>(response.Content);

            //return tran_res;

            if (response.Data != null)
                return response.Data;
            else 
                throw new Exception("Failed to get a valid response from PayTabs API.");            
        }
    }
}
