using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public static class Extensions
    {
        public static long FindItem(this ESIResponse<List<Asset>> response, int typeID)
        {
            return response.Result.FindItem(typeID);
        }

        public static long FindItem(this List<Asset> assets, int typeID)
        {
            int itemsCount = 0;

            foreach (var asset in assets)
            {
                if (asset.type_id == typeID)
                    itemsCount += asset.quantity;
            }

            return itemsCount;
        }

        public static List<Asset> GetContainer(this ESIResponse<List<Asset>> assets, long containerID)
        {
            return assets.Result.GetContainer(containerID);
        }

        public static List<Asset> GetContainer(this List<Asset> assets, long containerID)
        {
            var containerAssets = new List<Asset>();

            foreach (var asset in assets)
            {
                if (asset.location_id == containerID)
                    containerAssets.Add(asset);
            }

            return containerAssets;
        }
    }
}
