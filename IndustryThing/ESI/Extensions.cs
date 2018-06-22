using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public static class Extensions
    {
        #region Asset
        internal static long FindItem(this ESIResponse<List<Asset>> response, int typeID)
        {
            return response.Result.FindItem(typeID);
        }

        internal static long FindItem(this List<Asset> assets, int typeID)
        {
            int itemsCount = 0;

            foreach (var asset in assets)
            {
                if (asset.type_id == typeID)
                    itemsCount += asset.quantity;
            }

            return itemsCount;
        }

        internal static List<Asset> GetContainer(this ESIResponse<List<Asset>> assets, long containerID)
        {
            return assets.Result.GetContainer(containerID);
        }

        internal static List<Asset> GetContainer(this List<Asset> assets, long containerID)
        {
            var containerAssets = new List<Asset>();

            foreach (var asset in assets)
            {
                if (asset.location_id == containerID)
                    containerAssets.Add(asset);
            }

            return containerAssets;
        }
        #endregion

        #region Industry jobs
        internal static int GetJobs(this ESIResponse<List<IndustryJob>> response, int typeID, db.Db dataBase)
        {
            return response.Result.GetJobs(typeID, dataBase);
        }
        /// <summary>
        /// takes a typeID and returns the amount of items being produced currently
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        internal static int GetJobs(this List<IndustryJob> jobs, int typeID, db.Db dataBase)
        {
            int totalRuns = 0;

            foreach(var job in jobs)
            {
                if (job.product_type_id == typeID)
                    totalRuns += job.runs;
            }

            int[,] bpoOutput = dataBase.bpo.ManufacturingOutput(dataBase.bpo.FindBpoTypeIdForItem(typeID));
            return totalRuns * bpoOutput[0, 1];
        }
        #endregion

        #region Market orders
        internal static int FindOrder(this ESIResponse<List<CorporationMarketOrder>> response, int typeID)
        {
            return response.Result.FindOrder(typeID);
        }

        /// <summary>
        /// finds witch order holds a spesific type
        /// </summary>
        /// <param name="typeID">typeid</param>
        /// <returns>position in array(int) of the spesified item. Returns -1 if it cant find the item in question</returns>
        internal static int FindOrder(this List<CorporationMarketOrder> orders, int typeID)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                if (order.type_id == typeID && order.is_buy_order != true)
                    return i;
            }

            return -1;
        }

        internal static decimal SellOrderPrice(this ESIResponse<List<CorporationMarketOrder>> response, int typeID)
        {
            return response.Result.SellOrderPrice(typeID);
        }

        /// <summary>
        /// Finds the current price of a market order
        /// </summary>
        /// <param name="typeID">typeid</param>
        /// <returns>current order price</returns>
        internal static decimal SellOrderPrice(this List<CorporationMarketOrder> orders, int typeID)
        {
            int i = orders.FindOrder(typeID);

            if (i >= 0)
                return orders[i].price;

            return 0;
        }

        internal static int ItemsOnMarket(this ESIResponse<List<CorporationMarketOrder>> response, int typeID)
        {
            return response.Result.ItemsOnMarket(typeID);
        }

        /// <summary>
        /// Takes a typeID and check how many items of it is on sell orders(currently only shows the first order it finds)
        /// </summary>
        /// <param name="typeID">the typeID you are looking up</param>
        /// <returns></returns>
        internal static int ItemsOnMarket(this List<CorporationMarketOrder> orders, int typeID)
        {
            int i = orders.FindOrder(typeID);

            if (i >= 0)
                return orders[i].volume_remain;

            else return 0;
        }
        #endregion

        #region Cost indices
        internal static decimal GetBuildCostIndex(this ESIResponse<List<CostIndice>> response, db.Db dataBase)
        {
            return response.Result.GetBuildCostIndex(dataBase);
        }

        internal static decimal GetBuildCostIndex(this List<CostIndice> indices, db.Db dataBase)
        {
            foreach (var indice in indices)
                if (indice.solar_system_id == dataBase.settings.ProductionSystem)
                    return indice.manufacturing;

            return 0;
        }
        #endregion

        #region Market price
        public static decimal FindAdjustedPrice(this ESIResponse<List<MarketPrice>> response, int typeID)
        {
            return response.Result.FindAdjustedPrice(typeID);
        }

        public static decimal FindAdjustedPrice(this List<MarketPrice> prices, int typeID)
        {
            foreach (var price in prices)
                if (price.type_id == typeID)
                    return price.adjusted_price ?? 0;

            return 0;
        }


        /// <summary>
        /// Finds the highests buy order on the market.
        /// </summary>
        /// <returns></returns>
        public static decimal FindHighBuyPrice(this List<RegionMarketOrder> orders, long locationID = 60003760, int? typeID = null) // default jita 4-4
        {
            decimal high = 0;

            foreach (var order in orders)
            {
                if (order.location_id != locationID)
                    continue;

                if (typeID != null && order.type_id != typeID)
                    continue;

                if (order.is_buy_order == true && order.price > high)
                    high = order.price;
            }

            return high;
        }

        /// <summary>
        /// Finds the lowest sell order on the market
        /// </summary>
        public static decimal FindLowSellPrice(this List<RegionMarketOrder> orders, long locationID = 60003760, int? typeID = null) // default jita 4-4
        {
            decimal? low = null;

            foreach (var order in orders)
            {
                if (order.location_id != locationID)
                    continue;

                if (typeID != null && order.type_id != typeID)
                    continue;

                if (order.is_buy_order == false && (low == null || order.price < low))
                    low = order.price;
            }

            return low ?? 0;
        }

        /// <summary>
        /// Works out average market volume in the region
        /// </summary>
        public static long GetAverageVolume(this List<RegionMarketHistory> history, int days)
        {
            // No divide by zero, thanks
            if (days == 0)
                days = 1;

            DateTime d = DateTime.UtcNow.Date.AddDays(-days);
            long totalvolume = 0;

            foreach (var h in history)
            {
                if (d >= h.date)
                    totalvolume += h.volume;
            }

            return totalvolume / days;
        }
        #endregion
    }
}
