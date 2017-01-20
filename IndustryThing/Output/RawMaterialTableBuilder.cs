using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class RawMaterialTableBuilder
    {

        public RawMaterialTableBuilder(db.Db dataBase, calculator.T2Builder t2builder,Market.Market market, StreamWriter sw, string tableName, ContainerII office)
        {
            long[,] materials = new long[1, 1];
            if (tableName== "Minerals") materials= t2builder.GetGroup(4);
            else if (tableName == "Planetary Interaction") materials = t2builder.GetGroup(2);
            else if (tableName== "Advanced Materials") materials= t2builder.GetGroup(6);
            string[] names = new string[materials.Length / 2];
            int i = 0;
            while (i<names.Length)
            {
                names[i] = dataBase.types.TypeName(materials[i, 0]);
                i++;
            }

            sw.WriteLine("<table>");
            sw.WriteLine("<caption><b>" + tableName + "</b></caption>");
            sw.WriteLine("<tr><td>Name</td><td>Needed</td><td>Have</td><td>Lacking</td><td>Value</td></tr>");
            i = 0;
            decimal[] value=new decimal[names.Length];
            while (i < names.Length)
            {
                 value[i] = market.FindPrice("the forge", "buy", materials[i,0])* materials[i,1];
                sw.WriteLine("<tr><td>" + names[i] + "</td><td>" + string.Format("{0:n0}", materials[i,1]) + "</td><td>" +
                    string.Format("{0:n0}", 0) + "</td><td>" + string.Format("{0:n0}", 0) + "</td><td>" + string.Format("{0:n0}", value[i]) + "</td></tr>");
                i++;
            }
            sw.WriteLine("<tr><td><b>Sum</b></td><td></td><td><b>" + string.Format("{0:n0}", 0) + "</b></td><td><b>" + 
                string.Format("{0:n0}", 0) + "</b></td><td><b>" + string.Format("{0:n0}", value.Sum()) + "</b></td></b></tr>");
        }
    }
}
