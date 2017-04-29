using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.Market
{
    class Region
    {
       private ItemValues[] itemValues;
       private ItemHistory[] itemHistory;
       private int region;
       private int count;

        public Region(int reg)
        {
            region = reg;
            itemValues = new ItemValues[1000];
            itemHistory = new ItemHistory[1000];
        }

        /// <summary>
        /// Finds the location in the array of the item. Makes a new entry if item is not found
        /// </summary>
        /// <param name="typeID">the item's typeID</param>
        /// <returns>location in the arrays</returns>
        private int FindLocationOfType(int typeID)
        {
            int i = 0;
            while (i < count)
            {
                if (itemValues[i].TypeID == typeID) return i;
                i++;
            }
            count++;
            itemValues[i] = new ItemValues(typeID, region);
            itemHistory[i] = new ItemHistory(typeID, region);
            return i;
        }

        public long GetAverageVolume(int typeID, int days)
        {
            return itemHistory[FindLocationOfType(typeID)].GetAverageVolume(days);
        }

        public decimal GetPrice(long typeID, string orderType)
        {
            int location = FindLocationOfType(Convert.ToInt32(typeID));
            if (orderType == "buy") return itemValues[location].BuyPrice;
            else if (orderType == "sell") return itemValues[location].SellPrice;
            else return 0;
        }

    }
}
