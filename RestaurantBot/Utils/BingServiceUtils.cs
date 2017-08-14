using Microsoft.Bot.Builder.Dialogs;
using RestaurantBot.Constants;
using RestaurantBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Microsoft.Practices.Unity;
using Microsoft.Bot.Connector;

namespace RestaurantBot.Utils
{
    public static class BingServiceUtils
    {
        public static string URL =  "https://www.bingapis.com/api/v7/Places/search?q=";
        public static Dictionary<string, string> getRootObject(IDialogContext context, Activity activity, string query)
        {
            var azureStorageProvider = InjectionContainer.Instance.Container.Resolve<AzureStorageProvider>();

           // var rootObject = null;

            var rootObject = azureStorageProvider.GetConversationsData(activity.Conversation.Id, activity.ChannelId);
            if(rootObject.Count<ConversationEntity>() != 0)
            {
                ConversationEntity conversationEntity = rootObject.Last<ConversationEntity>();
                return DeserializeToDict(conversationEntity.ConversationData);
            }
            
          return null;
        }

        public static RootObject getRootObject(String url)
        {
            if (url == null || url == "") return null;
            string result = "";
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(HttpUtility.UrlPathEncode(url));
                HttpResponseMessage response = client.GetAsync(AddQuery(url, "AppID", AppInfo.APP_ID)).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            return Deserialize(result, url);
        }
        public static RootObject Deserialize(string json, string url)
        {
            RootObject rootObject = null;
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                // Deserialization from JSON  
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(RootObject));
                rootObject = (RootObject)deserializer.ReadObject(ms);

            }
            rootObject.currentUrl = url;
            return rootObject;
        }

        public static Dictionary<string, string> DeserializeToDict(string json)
        {
            Dictionary<string, string> bingUrlHash = null;
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                // Deserialization from JSON  
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
                bingUrlHash = (Dictionary<string, string>)deserializer.ReadObject(ms);

            }
            return bingUrlHash;
        }

        public static string Serialize(RootObject rootObject)
        {
            MemoryStream ms = new MemoryStream();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
            ser.WriteObject(ms, DbUtils.getBingUrlHash(rootObject));
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
        private static String AddQuery(string uri, string name, string value)
        {
            var ub = new UriBuilder(new Uri(uri));

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(ub.Query);

            httpValueCollection.Add(name, value);
            httpValueCollection.Add("mkt", "en-in");

            // urlencodes the whole HttpValueCollection
            ub.Query = httpValueCollection.ToString();

            return ub.ToString();
        }
    }
}