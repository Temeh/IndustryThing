using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace IndustryThing.Market
{
    class CostIndices
    {
        db.Db dataBase;
        Systems[] systems;
        public CostIndices(db.Db dataBase)
        {
            this.dataBase = dataBase;
            systems = new Systems[10000];
            int systemCount = 0;
            WebRequest getprices = WebRequest.Create("https://esi.tech.ccp.is/dev/industry/systems/?datasource=tranquility");
            Stream objStream = getprices.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(objStream);
            string text = sr.ReadLine();
            text = text.Substring(2);
            while (true)
            {
                if (text.StartsWith("\"solar_system_id\""))
                {
                    systems[systemCount] = new Systems(text.Substring(0, text.IndexOf("]")));
                    systemCount++;
                    text = text.Substring(text.IndexOf("]") + 2);
                }
                if (text.StartsWith(", {"))
                {
                    text = text.Substring(3);
                }
                if (text.StartsWith("]"))
                {
                    break;
                }
            }
        }

        public decimal GetBuildCostIndex()
        {
            int i = 0;
            while (true)
            {
                if (systems[i].SystemID == dataBase.settings.ProductionSystem) { return systems[i].manufacturing; }
                i++;
            }
        }
    }

    class Systems
    {
        public int SystemID;
        public decimal invention;
        public decimal manufacturing;
        public decimal researching_time_efficiency;
        public decimal researching_material_efficiency;
        public decimal copying;

        public Systems(string text)
        {
            text = text.Substring(text.IndexOf(":") + 2);
            SystemID = Convert.ToInt32(text.Substring(0, text.IndexOf(",")));
            text = text.Substring(text.IndexOf("{"));
            while (!(text.Length == 0))
            {
                text = text.Substring(text.IndexOf(":") + 3);
                if (text.StartsWith("invention"))
                {
                    invention = GetValue(text.Substring(text.IndexOf(":") + 2));
                    text = text.Substring(text.IndexOf("}") + 1);
                }
                if (text.StartsWith("manufacturing"))
                {
                    manufacturing = GetValue(text.Substring(text.IndexOf(":") + 2));
                    text = text.Substring(text.IndexOf("}") + 1);
                }
                if (text.StartsWith("researching_time_efficiency"))
                {
                    researching_time_efficiency = GetValue(text.Substring(text.IndexOf(":") + 2));
                    text = text.Substring(text.IndexOf("}") + 1);
                }
                if (text.StartsWith("researching_material_efficiency"))
                {
                    researching_material_efficiency = GetValue(text.Substring(text.IndexOf(":") + 2));
                    text = text.Substring(text.IndexOf("}") + 1);
                }
                if (text.StartsWith("copying"))
                {
                    copying = GetValue(text.Substring(text.IndexOf(":") + 2));
                    text = text.Substring(text.IndexOf("}") + 1);
                }
            }
        }

        decimal GetValue(string text)
        {
            return decimal.Parse(text.Substring(0, text.IndexOf("}")),StaticInfo.ci);
        }
    }
}
