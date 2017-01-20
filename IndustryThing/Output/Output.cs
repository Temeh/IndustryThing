﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.Output
{
    class Output
    {

        public Output(calculator.T2Builder t2mods, db.Db dataBase, Market.Market market)
        {
            MainImport import = new MainImport();
            ContainerII office = import.assets.assets.GetContainer("1022964286749");

            StreamWriter sw = new StreamWriter("moduleNumbers.html");
            StreamReader sr = new StreamReader("htmloutputone.txt");
            sw.WriteLine(sr.ReadToEnd());
            OutputTableBuilder otb = new OutputTableBuilder(dataBase, t2mods, sw, "T2Modules(and ships)");
            IntermediaryTableBuilder itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "T2Components", office);
            itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "T1modules", office);
            itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "T1ships", office);
            itb = new IntermediaryTableBuilder(dataBase, t2mods, sw, "Tools", office);

            RawMaterialTableBuilder rmtb = new RawMaterialTableBuilder(dataBase, t2mods, market, sw, "Minerals", office);
            rmtb = new RawMaterialTableBuilder(dataBase, t2mods, market, sw, "Planetary Interaction", office);
            rmtb = new RawMaterialTableBuilder(dataBase, t2mods, market, sw, "Advanced Materials", office);

            sr = new StreamReader("htmloutputtwo.txt");
            sw.WriteLine(sr.ReadToEnd());
            sw.Close();
            System.Diagnostics.Process.Start(@"moduleNumbers.html");
        }
    }
}
///
//  if (tableName== "T2Components") materials= t2builder.GetGroup(5);
//    else if (tableName == "T1modules") materials = t2builder.GetGroup(0);
//else if (tableName== "T1ships") materials= t2builder.GetGroup(1);
//     else if (tableName == "Tools") materials = t2builder.GetGroup(3);
