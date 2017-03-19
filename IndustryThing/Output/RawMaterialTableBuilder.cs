using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class RawMaterialTableBuilder
    {

        public RawMaterialTableBuilder(db.Db dataBase, calculator.T2Builder t2builder, Market.Market market, StreamWriter sw, string tableName, ApiImport.MainImport import)
        {
            ApiImport.ContainerII office = import.buildCorpAssets.assets.GetContainer("1022964286749");
            long[,] materials = new long[1, 1];
            if (tableName == "Minerals") materials = t2builder.GetGroup(4);
            else if (tableName == "Planetary Interaction") materials = t2builder.GetGroup(2);
            else if (tableName == "Advanced Materials") materials = t2builder.GetGroup(6);
            string[] names = new string[materials.Length / 2];
            int i = 0;
            while (i < names.Length)
            {
                names[i] = dataBase.types.TypeName(materials[i, 0]);
                i++;
            }

            sw.WriteLine("<table>");
            sw.WriteLine("<caption><b>" + tableName + "</b></caption>");
            sw.WriteLine("<tr><td>Name</td><td>Needed</td><td>Have</td><td>Lacking</td><td>Value</td></tr>");
            i = 0;
            decimal totalValue = 0;
            while (i < names.Length)
            {
                long need = materials[i, 1];
                long have = office.FindItem(Convert.ToInt32(materials[i, 0]));
                long lacking;
                decimal value = 0;
                if (have - need >= 0) lacking = 0;
                else
                {
                    lacking = need - have;
                    value = market.FindPrice("the forge", "buy", materials[i, 0]) * lacking; totalValue += value;
                }
                sw.WriteLine("<tr><td>" + names[i] + "</td><td>" +
                    string.Format("{0:n0}", need) + "</td><td>" +
                    string.Format("{0:n0}", have) + "</td><td>" +
                    string.Format("{0:n0}", lacking) + "</td><td>" +
                    string.Format("{0:n0}", value) + "</td></tr>");
                i++;
            }
            sw.WriteLine("<tr><td><b>Sum</b></td><td></td><td><b>" +
               null + "</b></td><td><b>" +
               null + "</b></td><td><b>" +
               string.Format("{0:n0}", totalValue) + "</b></td></tr>");
            sw.WriteLine("</table>");
        }
    }
}
