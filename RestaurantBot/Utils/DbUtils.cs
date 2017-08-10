using RestaurantBot.Constants;
using RestaurantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantBot.Utils
{
    public class DbUtils
    {
       public static Dictionary<string, string> getBingUrlHash(RootObject rootObject)
        {
            /**
             * adding filtervalue.name -> url and scalarvalue.minLevel -> url to the bingurlhash
             **/
            Dictionary<string, string> bingUrlHash = new Dictionary<string, string>();
            if(rootObject.filters.Count > 0)
            {
                foreach(Filter filter in rootObject.filters)
                {
                    foreach(FilterValue filterValue in filter.filterValues)
                    {
                        addItemsToHash(bingUrlHash, filterValue.name.ToLower(), filterValue.url);
                        if(filterValue.scalarValue != null && filterValue.scalarValue.minLevel != null)
                        {
                            addItemsToHash(bingUrlHash, filterValue.scalarValue.minLevel.ToLower(), filterValue.url);
                        }
                    }
                }
            }
            bingUrlHash.Add(AppInfo.CURRENT_URL, rootObject.currentUrl);
            return bingUrlHash;
        }

        private static void addItemsToHash(Dictionary<string, string> bingUrlHash, string key, string value)
        {
            if(key!= null && value!=null && key !="" && value != "") {
                bingUrlHash.Add(key.ToLower(), value);
            }
        }
    }
}