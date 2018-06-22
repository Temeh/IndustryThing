using IndustryThing.ESI;
using IndustryThing.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.Market
{
    class Region
    {
       private int regionID;

        // Key is typeID
        // TODO: This should be int - can of worms
        Dictionary<long, ESIResponse<List<ESI.RegionMarketOrder>>> ESIregionMarketOrdersDictionary;
        // Key is typeID
        Dictionary<int, ESIResponse<List<ESI.RegionMarketHistory>>> ESIregionMarketHistoryDictionary;

        public Region(int reg)
        {
            ESIregionMarketOrdersDictionary = new Dictionary<long, ESIResponse<List<RegionMarketOrder>>>();
            ESIregionMarketHistoryDictionary = new Dictionary<int, ESIResponse<List<RegionMarketHistory>>>();

            regionID = reg;
        }

        List<RegionMarketOrder> GetRegionPriceForType(long typeID)
        {
            // If we don't have the data, get it from ESI
            if (!ESIregionMarketOrdersDictionary.ContainsKey(typeID))
            {
                var parms = new Dictionary<string, object>();
                parms.Add("type_id", typeID);
                parms.Add("order_type", "all");
                parms.Add("region_id", regionID);

                var data = StaticInfo.ESIImportCrawl<RegionMarketOrder>("/markets/{region_id}/orders/", CharacterEnum.EmpireDonkey, parms);
                ESIregionMarketOrdersDictionary.Add(typeID, data);

                Console.WriteLine("....Done getting market orders for region " + regionID + " and type " + typeID);
            }

            return ESIregionMarketOrdersDictionary[typeID].Result;
        }

        List<RegionMarketHistory> GetRegionHistoryForType(int typeID)
        {
            // If we don't have the data, get it from ESI
            if (!ESIregionMarketHistoryDictionary.ContainsKey(typeID))
            {
                var parms = new Dictionary<string, object>();
                parms.Add("type_id", typeID);
                parms.Add("region_id", regionID);

                var data = StaticInfo.GetESIResponse<List<RegionMarketHistory>>("/markets/{region_id}/history/", CharacterEnum.EmpireDonkey, parms);
                ESIregionMarketHistoryDictionary.Add(typeID, data);

                Console.WriteLine("....Done getting market history for region " + regionID + " and type " + typeID);
            }

            return ESIregionMarketHistoryDictionary[typeID].Result;
        }

        public long GetAverageVolume(int typeID, int days)
        {
            return GetRegionHistoryForType(typeID).GetAverageVolume(days);
        }

        public decimal GetPrice(long typeID, string orderType)
        {
            switch (orderType)
            {
                case "buy":
                    return GetRegionPriceForType(typeID).FindHighBuyPrice();
                case "sell":
                    return GetRegionPriceForType(typeID).FindLowSellPrice();
                default:
                    return 0;
            }
        }
    }
}
