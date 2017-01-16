using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class Output
    {
        public Output(string[] name, int[,] output, decimal[] totalCost, decimal[] totalValue)
        {
            StreamWriter sw = new StreamWriter("moduleNumbers.html");
            StreamReader sr = new StreamReader("htmloutputone.txt");
            sw.WriteLine(sr.ReadToEnd());
            sw.WriteLine("<table>");
            sw.WriteLine("<caption><b>Glorious t2 module profits</b></caption>");
            sw.WriteLine("<tr><td>Name</td><td>Amount</td><td>Value</td><td>Cost</td><td>Profit</td></tr>");
            int i = 0;
            while (i<name.Length)
            {
                decimal profit = totalValue[i]-totalCost[i];
                 sw.WriteLine("<tr><td>"+string.Format("{0:n0}", name[i])+"</td><td>"+string.Format("{0:n0}", output[i,1])+"</td><td>"+
                     string.Format("{0:n0}", totalValue[i])+"</td><td>"+string.Format("{0:n0}", totalCost[i])+"</td><td>"+string.Format("{0:n0}",profit)+"</td></tr>");
                 i++;
            }
            decimal valueSum = totalValue.Sum();
            decimal costSum = totalCost.Sum();
            i = 0;
            decimal profitSum = 0;
            while (i<name.Length)
            {
                if ((totalValue[i] - totalCost[i]) > 0) profitSum += totalValue[i] - totalCost[i];
                i++;
            }
            sw.WriteLine("<tr><td><b>Sum</b></td><td></td><td><b>" + string.Format("{0:n0}", valueSum) + "</b></td><td><b>" + string.Format("{0:n0}", costSum) + "</b></td><td><b>" + string.Format("{0:n0}", profitSum) + "</b></td></b></tr>");
            sr = new StreamReader("htmloutputtwo.txt");
            sw.WriteLine(sr.ReadToEnd());
            sw.Close();
            System.Diagnostics.Process.Start(@"moduleNumbers.html");
        }
    }
}
