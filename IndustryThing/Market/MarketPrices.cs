using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace IndustryThing.Market
{
    class MarketPrices
    {
        int[] typeId = new int[11000];
        decimal[] averagePrice = new decimal[11000];
        decimal[] adjustedPrice = new decimal[11000];
        public MarketPrices()
        {
            /* WebRequest getprices = WebRequest.Create("https://esi.tech.ccp.is/dev/markets/prices/?datasource=tranquility");
            Stream objStream = getprices.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(objStream); */
            StreamReader sr = IndustryThing.StaticInfo.GetStream("https://esi.tech.ccp.is/dev/markets/prices/?datasource=tranquility");

            string text = sr.ReadToEnd();
            text = text.Substring(1);
            int i = 0;
            while (!text.StartsWith("]"))
            {
                if (text.StartsWith("{"))
                {
                    text = text.Substring(1);
                    while (!text.StartsWith("}"))
                    {
                        if (text.StartsWith("\"type_id\""))
                        {
                            text = text.Substring(text.IndexOf(":") + 2);
                            typeId[i] = Convert.ToInt32(text.Substring(0, text.IndexOf(",")));
                            text = text.Substring(text.IndexOf(",") + 2);
                        }
                        if (text.StartsWith("\"average_price\""))
                        {
                            text = text.Substring(text.IndexOf(":") + 2);
                            averagePrice[i] = Convert.ToDecimal(text.Substring(0, text.IndexOf(",")));
                            text = text.Substring(text.IndexOf(",") + 2);
                        }
                        if (text.StartsWith("\"adjusted_price\""))
                        {
                            text = text.Substring(text.IndexOf(":") + 2);
                            adjustedPrice[i] = Convert.ToDecimal(text.Substring(0, text.IndexOf("}")));
                            text = text.Substring(text.IndexOf("}"));
                        }
                    }
                    i++;
                    text = text.Substring(1);
                }
                if (text.StartsWith(", ")) text = text.Substring(2);
            }
        }

        /// <summary>
        /// finds the adjusted market price for a typeID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public decimal FindAdjustedPrice(int ID)
        {
            int i = 0;
            while (true)
            {
                 if (typeId[i] == ID) return adjustedPrice[i];
                i++;
            }
        }

        /// <summary>
        /// finds the average market price for a typeID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public decimal FindAveragePrice(int ID)
        {
            int i = 0;
            while (true)
            {
                 if (typeId[i] == ID) return adjustedPrice[i];
                i++;
            }
        }
    }
}