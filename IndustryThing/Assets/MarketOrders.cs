using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.ApiImport
{
    class MarketOrders
    {
        private long[] orderID = new long[256];
        private int[] charID = new int[256];
        private long[] stationID = new long[256];
        private int[] volEntered = new int[256];
        private int[] volRemaining = new int[256];
        private int[] minVolume = new int[256];
        private int[] orderState = new int[256];
        private int[] typeID = new int[256];
        private int[] range = new int[256];
        private int[] accountKey = new int[256];
        private int[] duration = new int[256];
        private decimal[] escrow = new decimal[256];
        private decimal[] price = new decimal[256];
        private int[] bid = new int[256];
        private DateTime[] issued = new DateTime[256];
        private int totalOrders;
        private DateTime cachedUntil;
        public DateTime CachedUntil { get { return cachedUntil; } }

        public MarketOrders(StreamReader api)
        {
            api.ReadLine(); api.ReadLine(); api.ReadLine(); api.ReadLine(); api.ReadLine();
            int i = 0;
            while (i < 256)
            {
                string line = api.ReadLine();
                line = line.Trim();
                if (line.StartsWith("<row "))
                {
                    line = line.Substring(line.IndexOf("\"") + 1);//Gets order ID
                    orderID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets charID
                    charID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets stationID
                    stationID[i] = Convert.ToInt64(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets volume entered
                    volEntered[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets volume remaining
                    volRemaining[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets minimum volume
                    minVolume[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets order state
                    orderState[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets typeID
                    typeID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets range
                    range[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets account key
                    accountKey[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets duration
                    duration[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets escrow
                    escrow[i] = decimal.Parse(line.Substring(0, line.IndexOf("\"")),StaticInfo.ci);
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets price
                    price[i] = decimal.Parse(line.Substring(0, line.IndexOf("\"")),StaticInfo.ci);
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets ordertype
                    bid[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets issued time
                    issued[i] = Convert.ToDateTime(line.Substring(0, line.IndexOf("\"")));
                }
                else if (line.StartsWith("</rowset>"))
                {
                    totalOrders = i;
                    break;
                }
                i++;
            }
            while (!api.EndOfStream)
            {
                string line = api.ReadLine(); line = line.Trim();
                if (line.StartsWith("<cachedUntil>")) cachedUntil = Convert.ToDateTime(line.Substring(13, 19));
            }

        }

        /// <summary>
        /// Takes a typeID and check how many items of it is on sell orders(currently only shows the first order it finds)
        /// </summary>
        /// <param name="findType">the typeID you are looking up</param>
        /// <returns></returns>
        public int ItemsOnMarket(int findType)
        {
            int order = FindOrder(findType);
            if (order >= 0) return volRemaining[order];
            else return 0;
        }

        /// <summary>
        /// Finds the current price of a market order
        /// </summary>
        /// <param name="findType">typeid</param>
        /// <returns>current order price</returns>
        public decimal SellOrderPrice(int findType)
        {
            int order = FindOrder(findType);
            if (order >= 0) return price[order];
            else return 0;
        }

        /// <summary>
        /// finds witch order holds a spesific type
        /// </summary>
        /// <param name="findType">typeid</param>
        /// <returns>position in array(int) of the spesified item. Returns -1 if it cant find the item in question</returns>
        private int FindOrder(int findType)
        {
            int i = 0;
            while (i < totalOrders)
            {
                if (typeID[i] == findType)
                {
                    if (bid[i] == 0 && orderState[i] == 0) return i;
                }
                i++;
            }
            return -1;
        }
    }
}
