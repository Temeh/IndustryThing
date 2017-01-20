using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class IntermediaryTableBuilder
    {

        public IntermediaryTableBuilder(db.Db dataBase, calculator.T2Builder t2builder, StreamWriter sw, string tableName, ContainerII office)
        {
            long[,] materials = new long[1, 1];
            if (tableName== "T2Components") materials= t2builder.GetGroup(5);
            else if (tableName == "T1modules") materials = t2builder.GetGroup(0);
            else if (tableName== "T1ships") materials= t2builder.GetGroup(1);
            else if (tableName == "Tools") materials = t2builder.GetGroup(3);
            string[] names = new string[materials.Length / 2];
            int i = 0;
            while (i<names.Length)
            {
                names[i] = dataBase.types.TypeName(materials[i, 0]);
                i++;
            }

            sw.WriteLine("<table>");
            sw.WriteLine("<caption><b>" + tableName + "</b></caption>");
            sw.WriteLine("<tr><td>Name</td><td>Needed</td><td>Have</td><td>Building</td><td>Lacking</td></tr>");
            i = 0;
            while (i < names.Length)
            {
                sw.WriteLine("<tr><td>" + names[i] + "</td><td>" + string.Format("{0:n0}", materials[i,1]) + "</td><td>" +
                    string.Format("{0:n0}", 0) + "</td><td>" + string.Format("{0:n0}", 0) + "</td><td>" + string.Format("{0:n0}", 0) + "</td></tr>");
                i++;
            }
        }
    }
}
