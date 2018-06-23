using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.ApiImport
{
    class MainImport
    {
        public ESIResponse<List<ESI.Asset>> ESIbuildCorpAssets;
        public ESIResponse<List<ESI.Asset>> ESIempireDonkey;
        public ESIResponse<List<ESI.IndustryJob>> ESIjobs;
        public ESIResponse<List<ESI.CorporationMarketOrder>> ESIcorpMarketOrders;

        public MainImport()
        {
            ESIAssetImport();
            ESIIndustryJobsImport();
            ESIMarketOrdersImport();
        }

        void ESIAssetImport()
        {
            ESIbuildCorpAssets = StaticInfo.ESIImportCrawl<ESI.Asset>("/corporations/{corporation_id}/assets/", ESI.CharacterEnum.BuildCorp);
            ESIempireDonkey = StaticInfo.ESIImportCrawl<ESI.Asset>("/characters/{character_id}/assets/", ESI.CharacterEnum.EmpireDonkey);

            Console.WriteLine("....Done loading assets");
        }

        void ESIIndustryJobsImport()
        {
            ESIjobs = StaticInfo.ESIImportCrawl<ESI.IndustryJob>("corporations/{corporation_id}/industry/jobs/", ESI.CharacterEnum.BuildCorp);
            Console.WriteLine("....Done loading industry jobs");
        }

        void ESIMarketOrdersImport()
        {
            ESIcorpMarketOrders = StaticInfo.ESIImportCrawl<ESI.CorporationMarketOrder>("corporations/{corporation_id}/orders/", ESI.CharacterEnum.EmpireDonkey);
            Console.WriteLine("....Done loading market orders");
        }
    }
}
