using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class IntermediaryTableBuilder
    {

        public IntermediaryTableBuilder(db.Db dataBase, calculator.T2Builder t2builder, StreamWriter sw, string tableName, ApiImport.MainImport import, Market.Market market)
        {
            ApiImport.ContainerII office = import.assets.assets.GetContainer("1022964286749");

            long[,] materials = new long[1, 1];
            if (tableName == "T2Components") materials = t2builder.GetGroup(5);
            else if (tableName == "T1modules") materials = t2builder.GetGroup(0);
            else if (tableName == "T1ships") materials = t2builder.GetGroup(1);
            else if (tableName == "Tools") materials = t2builder.GetGroup(3);
            string[] names = new string[materials.Length / 2];
            int i = 0;
            while (i < names.Length)
            {
                names[i] = dataBase.types.TypeName(materials[i, 0]);
                i++;
            }

            sw.WriteLine("<table>");
            sw.WriteLine("<caption><b>" + tableName + "</b></caption>");
            sw.WriteLine("<tr><td>Name</td><td>Needed</td><td>Have</td><td>Building</td><td>Lacking</td><td>Value</td></tr>");
            i = 0;
            decimal totalValue = 0;
            while (i < names.Length)
            {
                long need = materials[i, 1];
                long have = office.FindItem(Convert.ToInt32(materials[i, 0]));
                long building = import.jobs.GetJobs(Convert.ToInt32(materials[i, 0]));
                long lacking;
                if ((have + building) - need >= 0) lacking = 0;
                else lacking = need - building - have;
                decimal value = market.FindPrice("the forge", "buy", materials[i, 0]) * lacking; totalValue += value;
                sw.WriteLine("<tr><td>" + names[i] + "</td><td>" +
                    string.Format("{0:n0}", need) + "</td><td>" +
                    string.Format("{0:n0}", have) + "</td><td>" +
                    string.Format("{0:n0}", building) + "</td><td>" +
                    string.Format("{0:n0}", lacking) + "</td><td>" +
                     string.Format("{0:n0}", value) + "</td></tr>");
                i++;
            }
            sw.WriteLine("<tr><td><b>Sum</b></td><td>" + null + "</td><td>" + null + "</td><td>" + null + "</td><td>" + null + "</td><td><b>" + string.Format("{0:n0}", totalValue) + "</b></td></tr>");
            sw.WriteLine("</table>");
        }
    }
}
