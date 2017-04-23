using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace IndustryThing.Market
{
    class ItemValues
    {
        private int typeID;
        public long TypeID { get { return typeID; } }
        private int regionID;
        private decimal sellPrice;
        public decimal SellPrice { get { return sellPrice; } }
        private decimal buyPrice;
        public decimal BuyPrice { get { return buyPrice; } }

        Orders orders;


        public ItemValues(int id, int regionID)
        {
            typeID = id;
            this.regionID = regionID;

             orders = new Orders(typeID, regionID);
            sellPrice = orders.FindLowSellPrize();
            buyPrice = orders.FindHighBuyPrize();
        }
    }

    class Orders
    {
        long[] orderId;
        long[] typeId;
        long[] locationId;
        int[] volumeTotal;
        int[] volumeRemain;
        int[] minVolume;
        decimal[] price;
        bool[] isBuyOrder;
        int[] duration;
        string[] issued;
        string[] range;

        int regionID;
        int page = 1; // 1 is deafult value for now as its not really used yet!

        public Orders(int typeID_, int regionID_)
        {
            regionID = regionID_;

       /*     WebRequest getprices = WebRequest.Create(BuildUrl(typeID_));
            Stream objStream = getprices.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);  */
            StreamReader objReader = IndustryThing.StaticInfo.GetStream(BuildUrl(typeID_));
            string text = objReader.ReadLine();
            int i = text.Count(f => f == '{');
            orderId = new long[i];
            typeId = new long[i];
            locationId = new long[i];
            volumeTotal = new int[i];
            volumeRemain = new int[i];
            minVolume = new int[i];
            price = new decimal[i];
            isBuyOrder = new bool[i];
            duration = new int[i];
            issued = new string[i];
            range = new string[i];

            int count = 0;
            while (true)
            {
                if (text.StartsWith("{"))
                {
                    string segment = text.Substring(1, text.IndexOf("}") - 1);
                    text = text.Substring(text.IndexOf("}") + 1);
                    segment = segment.Substring(segment.IndexOf(":") + 2);
                    orderId[count] = Convert.ToInt64(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    typeId[count] = Convert.ToInt32(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    locationId[count] = Convert.ToInt64(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    volumeTotal[count] = Convert.ToInt32(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    volumeRemain[count] = Convert.ToInt32(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    minVolume[count] = Convert.ToInt32(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    price[count] = decimal.Parse(segment.Substring(0, segment.IndexOf(",")),StaticInfo.ci); segment = segment.Substring(segment.IndexOf(":") + 2);
                    {
                        if (segment.Substring(0, segment.IndexOf(",")) == "true") isBuyOrder[count] = true;
                        else isBuyOrder[count] = false;
                        segment = segment.Substring(segment.IndexOf(":") + 2);
                    }
                    duration[count] = Convert.ToInt32(segment.Substring(0, segment.IndexOf(","))); segment = segment.Substring(segment.IndexOf(":") + 2);
                    issued[count] = segment.Substring(0, segment.IndexOf(",")); segment = segment.Substring(segment.IndexOf(",") + 2); segment = segment.Substring(segment.IndexOf(":") + 2);
                    range[count] = segment.Substring(1, segment.Length - 2);
                    count++;
                }
                if (text.StartsWith(", ")) text = text.Substring(2);
                else if (text.StartsWith("[")) text = text.Substring(1);
                else if (text.StartsWith("]")) break;
            }
        }

        /// <summary>
        /// Makes a url to lookup the orders of an item in in the spesified region.
        /// </summary>
        /// <param name="ordertype">"buy"/"sell" string</param>
        /// <param name="typeID">the lookup's type ID</param>
        /// <returns>Full url for gathering market data of the item</returns>
        string BuildUrl(long typeID)
        {
            string url="https://esi.tech.ccp.is/latest/markets/" + regionID + "/orders/?type_id=" + typeID + "&page=" + page + "&datasource=tranquility";
            return url;
        }

        /// <summary>
        /// Finds the highests buy order on the market.
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public decimal FindHighBuyPrize()
        {
            int i = 0; decimal high = 0;
            while (i < price.Length)
            {
                if ((price[i] > high) && (isBuyOrder[i] == true)) high = price[i];
                i++;
            }
            return high;
        }

        /// <summary>
        /// Finds the lowest sell order on the market
        /// </summary>
        /// <returns></returns>
        public decimal FindLowSellPrize()
        {
            decimal low = price[0]; int i = 1;
            while (i < price.Length)
            {
                if (isBuyOrder[i] == false) if (price[i] < low) low = price[i];
                i++;
            }
            return low;
        }
    }
}
