using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.Market
{
    class Region
    {
        ItemValues[] itemvalues;
        int region;
        int count;

        public Region(int reg)
        {
            region = reg;
            itemvalues = new ItemValues[1000];
        }
        public void TestMethod()
        {

        }

        public decimal GetPrice(long typeID, string orderType)
        {
            int i=0;
            while (i < count)
            {
                if (itemvalues[i].TypeID == typeID)
                {
                    if (orderType == "buy") return itemvalues[i].BuyPrice;
                    else if (orderType == "sell") return itemvalues[i].SellPrice;
                    else return 0;
                }
                i++;
            }
            count++;
            itemvalues[i] = new ItemValues(typeID, region);
            if (orderType == "buy") return itemvalues[i].BuyPrice;
            else if (orderType == "sell") return itemvalues[i].SellPrice;
            else return 0;
        }

    }
}
