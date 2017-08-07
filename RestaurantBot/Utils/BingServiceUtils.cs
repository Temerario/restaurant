using RestaurantBot.Constants;
using RestaurantBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace RestaurantBot.Utils
{
    public static class BingServiceUtils
    {
        public static RootObject getRootObject(String url)
        {
            string result = "";
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage response = client.GetAsync(AddQuery(url, "AppID", AppInfo.APP_ID)).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            return Deserialize(result);
        }

        private static RootObject Deserialize(string json)
        {
            RootObject rootObject = null;
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                // Deserialization from JSON  
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(RootObject));
                rootObject = (RootObject)deserializer.ReadObject(ms);

            }
            return rootObject;
        }

        private static String AddQuery(string uri, string name, string value)
        {
            var ub = new UriBuilder(new Uri(uri));

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(ub.Query);

            httpValueCollection.Add(name, value);

            // urlencodes the whole HttpValueCollection
            ub.Query = httpValueCollection.ToString();

            return ub.ToString();
        }
    }
}