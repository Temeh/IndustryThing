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
        CostIndices costIndices;
        MarketPrices marketStandardized;

        public Market(db.Db dataBase)
        {
            this.dataBase = dataBase;
            region[0] = new Region(10000002); // the forge/jita
            region[1] = new Region(10000060); // delve
            costIndices = new CostIndices(dataBase);
            marketStandardized = new MarketPrices();
        }

        /// <summary>
        /// Tells you the value of an item
        /// </summary>
        /// <param name="regionName">The region name!</param>
        /// <param name="order_type">"buy"/"sell"</param>
        /// <param name="typeID">The typeID</param>
        /// <returns></returns>
        public decimal FindPrice(string regionName, string order_type, int typeID)
        {
            if (regionName == "the forge") return region[0].GetPrice(typeID, order_type);
            if (regionName == "delve") return region[1].GetPrice(typeID, order_type);

            return 0;
        }
        /// <summary>
        /// Tells you the adjusted price of an item
        /// </summary>
        /// <param name="typeID">TypeID</param>
        /// <returns>adjusted price</returns>
        public decimal FindAdjustedPrice(int typeID)
        {
            return marketStandardized.FindAdjustedPrice(typeID);
        }


        public decimal FindSystemIndexManufacturing()
        {
           return costIndices.GetBuildCostIndex();
        }
    }
}