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
        internal static int FindOrder(this ESIResponse<List<MarketOrder>> response, int typeID)
        {
            return response.Result.FindOrder(typeID);
        }

        /// <summary>
        /// finds witch order holds a spesific type
        /// </summary>
        /// <param name="typeID">typeid</param>
        /// <returns>position in array(int) of the spesified item. Returns -1 if it cant find the item in question</returns>
        internal static int FindOrder(this List<MarketOrder> orders, int typeID)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                if (order.type_id == typeID && order.is_buy_order != true)
                    return i;
            }

            return -1;
        }

        internal static decimal SellOrderPrice(this ESIResponse<List<MarketOrder>> response, int typeID)
        {
            return response.Result.SellOrderPrice(typeID);
        }

        /// <summary>
        /// Finds the current price of a market order
        /// </summary>
        /// <param name="typeID">typeid</param>
        /// <returns>current order price</returns>
        internal static decimal SellOrderPrice(this List<MarketOrder> orders, int typeID)
        {
            int i = orders.FindOrder(typeID);

            if (i >= 0)
                return orders[i].price;

            return 0;
        }

        internal static int ItemsOnMarket(this ESIResponse<List<MarketOrder>> response, int typeID)
        {
            return response.Result.ItemsOnMarket(typeID);
        }

        /// <summary>
        /// Takes a typeID and check how many items of it is on sell orders(currently only shows the first order it finds)
        /// </summary>
        /// <param name="typeID">the typeID you are looking up</param>
        /// <returns></returns>
        internal static int ItemsOnMarket(this List<MarketOrder> orders, int typeID)
        {
            int i = orders.FindOrder(typeID);

            if (i >= 0)
                return orders[i].volume_remain;

            else return 0;
        }
        #endregion
    }
}
