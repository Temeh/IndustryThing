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
    }
}
