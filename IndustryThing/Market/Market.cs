using IndustryThing.ESI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.Market
{
    class Market
    {
        // delve=10000060 jita = 10000002
        // https://esi.tech.ccp.is/latest/markets/10000060/orders/?type_id=2183&order_type=sell&page=1&datasource=tranquility
        db.Db dataBase;
        Region[] region = new Region[2];
        //CostIndices costIndices;
        //MarketPrices marketStandardized;

        ESIResponse<List<ESI.CostIndice>> ESIcostIndices;
        ESIResponse<List<ESI.MarketPrice>> ESImarketPrices;

        public Market(db.Db dataBase)
        {
            ESIcostIndices = StaticInfo.GetESIResponse<List<ESI.CostIndice>>("/industry/systems/");

            ESImarketPrices = StaticInfo.GetESIResponse<List<ESI.MarketPrice>>("/markets/prices/");

            this.dataBase = dataBase;
            region[0] = new Region(10000002); // the forge/jita
          //region[1] = new Region(10000060); // delve
            //costIndices = new CostIndices(dataBase);
            //marketStandardized = new MarketPrices();
        }


        /// <summary>
        /// Tells you the value of an item
        /// </summary>
        /// <param name="regionName">The region name!</param>
        /// <param name="order_type">"buy"/"sell"</param>
        /// <param name="typeID">The typeID</param>
        /// <returns></returns>
        public decimal FindPrice(string regionName, string order_type, long typeID)
        {
            int i = 0;
            if (regionName == "the forge") i = 0;
            //if (regionName == "delve") i = 1;
            return region[i].GetPrice(typeID, order_type);
        }

        /// <summary>
        /// Finds the average volume of items moved on the market each day
        /// </summary>
        /// <param name="regionName">Name of the region you want to look at</param>
        /// <param name="typeID">The item's typeID</param>
        /// <param name="days">How many days back you want to calculate the avg for</param>
        /// <returns>The average volume per day</returns>
        public long FindAverageVolume(string regionName, int typeID, int days)
        {
            int i = 0;
            if (regionName == "the forge") i = 0;
            //if (regionName == "delve") i = 1;
            return region[i].GetAverageVolume(typeID, days);
        }

        /// <summary>
        /// Tells you the adjusted price of an item
        /// </summary>
        /// <param name="typeID">TypeID</param>
        /// <returns>adjusted price</returns>
        public decimal FindAdjustedPrice(int typeID)
        {
            return ESImarketPrices.FindAdjustedPrice(typeID);
        }

        public decimal FindSystemIndexManufacturing()
        {
           return ESIcostIndices.GetBuildCostIndex(dataBase);
        }
    }
}