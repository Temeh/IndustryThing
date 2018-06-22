using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class Output
    {
        public Output(calculator.T2Builder t2mods, db.Db dataBase, Market.Market market, ApiImport.MainImport import)
        {
            StreamReader sr;
            using (StreamWriter sw = new StreamWriter("moduleNumbers.html"))
            {
                using (sr = new StreamReader("files\\htmloutputone.txt"))
                {
                    sw.WriteLine(sr.ReadToEnd());
                    OutputTableBuilder otb = new OutputTableBuilder(dataBase, t2mods, sw, "T2Modules(and ships)");
                    IntermediaryTableBuilder itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "T2Components", import, market);
                    itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "T1modules", import, market);
                    itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "T1ships", import, market);
                    itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "Tools", import, market);

                    RawMaterialTableBuilder rmtb = new RawMaterialTableBuilder(dataBase, t2mods, market, sw, "Minerals", import);
                    rmtb = new RawMaterialTableBuilder(dataBase, t2mods, market, sw, "Planetary Interaction", import);
                    rmtb = new RawMaterialTableBuilder(dataBase, t2mods, market, sw, "Advanced Materials", import);
                }

                using (sr = new StreamReader("files\\htmloutputtwo.txt"))
                    sw.WriteLine(sr.ReadToEnd());
                sw.Close();
            }
          
            System.Diagnostics.Process.Start(@"moduleNumbers.html"); 
            
            new MarketInfo(dataBase, t2mods, import, market);
        }
    }
}
///
//  if (tableName== "T2Components") materials= t2builder.GetGroup(5);
//    else if (tableName == "T1modules") materials = t2builder.GetGroup(0);
//else if (tableName== "T1ships") materials= t2builder.GetGroup(1);
//     else if (tableName == "Tools") materials = t2builder.GetGroup(3);
