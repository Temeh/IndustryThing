using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public static class Extensions
    {
        public static long FindItem(this ESIResponse<Asset[]> response, int typeID)
        {
            return response.Result.FindItem(typeID);
        }

        public static long FindItem(this Asset[] assets, int typeID)
        {
            int itemsCount = 0;

            foreach (var asset in assets)
            {
                if (asset.type_id == typeID)
                    itemsCount += asset.quantity;
            }

            return itemsCount;
        }

        public static Asset[] GetContainer(this ESIResponse<Asset[]> assets, long containerID)
        {
            return assets.GetContainer(containerID);
        }

        public static Asset[] GetContainer(this Asset[] assets, long containerID)
        {
            var containerAssets = new List<Asset>();

            foreach (var asset in assets)
            {
                if (asset.location_id == containerID)
                    containerAssets.Add(asset);
            }

            return containerAssets.ToArray();
        }
    }
}
