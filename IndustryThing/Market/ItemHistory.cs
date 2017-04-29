using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace IndustryThing.Market
{
    class ItemHistory
    {
        private int count;
        private DateTime[] date;
        private int[] orderCount;
        private long[] volume;
        private decimal[] highest;
        private decimal[] average;
        private decimal[] lowest;

        public ItemHistory(int typeID, int regionID)
        {
            StreamReader objReader = StaticInfo.GetStream(BuildUrl(typeID, regionID));
            string text = objReader.ReadToEnd();
            int i = text.Count(f => f == '{');
            date = new DateTime[i];
            orderCount = new int[i];
            volume = new long[i];
            highest = new decimal[i];
            average = new decimal[i];
            lowest = new decimal[i];

            count = 0;
            while (true)
            {
                if (text.StartsWith("{"))
                {
                    string segment = text.Substring(1, text.IndexOf("}") - 1);
                    text = text.Substring(text.IndexOf("}") + 1);
                    segment = segment.Substring(segment.IndexOf(":") + 2);
                    string test = segment.Substring(1, segment.IndexOf(",") - 2);
                    date[count] = Convert.ToDateTime(segment.Substring(1, segment.IndexOf(",")-2)); segment = segment.Substring(segment.IndexOf(":") + 2);
                    orderCount[count] = Convert.ToInt32(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    volume[count] = Convert.ToInt64(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    highest[count] = decimal.Parse(segment.Substring(0, segment.IndexOf(",")),StaticInfo.ci); segment = segment.Substring(segment.IndexOf(":") + 2);
                    average[count] = decimal.Parse(segment.Substring(0, segment.IndexOf(",")),StaticInfo.ci); segment = segment.Substring(segment.IndexOf(":") + 2);
                    lowest[count] = decimal.Parse(segment,StaticInfo.ci); 
                    count++;
                }
                if (text.StartsWith(", ")) text = text.Substring(2);
                else if (text.StartsWith("[")) text = text.Substring(1);
                else if (text.StartsWith("]")) break;
            }
        }

        /// <summary>
        /// Works out average market volume in the region
        /// </summary>
        /// <param name="typeID">the typeID of the item in question</param>
        /// <param name="days">How many days back you want to calc the average for</param>
        /// <returns>The average items per day</returns>
        public long GetAverageVolume(int days)
        {
            long totalvolume = 0;
            int i = 0;
            while (i<days)
            {
                i++;
                totalvolume += volume[count - i];
            }
            return totalvolume / days;
        }

        /// <summary>
        /// Makes a url to lookup the orders of an item in in the spesified region.
        /// </summary>
        /// <param name="ordertype">"buy"/"sell" string</param>
        /// <param name="typeID">the lookup's type ID</param>
        /// <returns>Full url for gathering market data of the item</returns>
        string BuildUrl(long typeID, int regionID)
        {
            string url="https://esi.tech.ccp.is/latest/markets/" + regionID + "/history/?type_id=" + typeID + "&datasource=tranquility";
            return url;
        }
    }
}
