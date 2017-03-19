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
        private int[] stationID = new int[256];
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
                    line = line.Substring(line.IndexOf("\"")+1);//Gets order ID
                    orderID[i] = Convert.ToInt64(line.Substring(0,line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets charID
                    charID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets stationID
                    stationID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("\"")));
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
                    escrow[i] = Convert.ToDecimal(line.Substring(0, line.IndexOf("\"")));
                    line = line.Substring(line.IndexOf("\"") + 1); line = line.Substring(line.IndexOf("\"") + 1);//Gets price
                    price[i] = Convert.ToDecimal(line.Substring(0, line.IndexOf("\"")));
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

        }

        /// <summary>
        /// Takes a typeID and check how many items of it is on sell orders
        /// </summary>
        /// <param name="findType">the typeID you are looking up</param>
        /// <returns></returns>
        public int ItemsOnMarket(int findType)
        {
            int i = 0;
            while (i < totalOrders)
            {
                if (typeID[i] == findType)
                {
                   if (bid[i]== 0) return volRemaining[i];
                }
                i++;
            }
            return 0;
        }
    }
}
