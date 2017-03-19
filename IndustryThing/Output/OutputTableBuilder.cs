using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class OutputTableBuilder
    {
        public OutputTableBuilder(db.Db dataBase, calculator.T2Builder t2builder, StreamWriter sw, string tableName)
        {
    string[] name = t2builder.OutputName;
    int[,] output = t2builder.Output;
    decimal[] totalCost = t2builder.OutputTotalCost;
    decimal[] totalValue = t2builder.OutputTotalValue;

    sw.WriteLine("<table>");
    sw.WriteLine("<caption><b>" + tableName + "</b></caption>");
    sw.WriteLine("<tr><td>Name</td><td>Amount</td><td>Value</td><td>Cost</td><td>Profit</td><td>Profit/m^3 ratio</td></tr>");
    int i = 0;
    while (i < name.Length)
    {
        decimal profit = totalValue[i] - totalCost[i];
        decimal profitRatio = profit / (dataBase.types.GetRepackagedVolume(output[i, 0]) * output[i, 1]);
        sw.WriteLine("<tr><td>" + 
            string.Format("{0:n0}", name[i]) + "</td><td>" +
            string.Format("{0:n0}", output[i, 1]) + "</td><td>" +
            string.Format("{0:n0}", totalValue[i]) + "</td><td>" +
            string.Format("{0:n0}", totalCost[i]) + "</td><td>" +
            string.Format("{0:n0}", profit) + "</td><td>" +
            string.Format("{0:n0}", profitRatio) + "</td></tr>");
        i++;
    }
    decimal valueSum = totalValue.Sum();
    decimal costSum = totalCost.Sum();
    i = 0;
    decimal profitSum = 0;
    while (i < name.Length)
    {
        if ((totalValue[i] - totalCost[i]) > 0) profitSum += totalValue[i] - totalCost[i];
        i++;
    }
    sw.WriteLine("<tr><td><b>Sum</b></td><td></td><td><b>" + string.Format("{0:n0}", valueSum) + "</b></td><td><b>" + string.Format("{0:n0}", costSum) + "</b></td><td><b>" + string.Format("{0:n0}", profitSum) + "</b></td></b><td></td></tr>");
    sw.WriteLine("</table>");
        }
    }
}
