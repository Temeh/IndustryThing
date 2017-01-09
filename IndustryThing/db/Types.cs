using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.db
{
    class Types
    {
        static int highestExpectedTypeid = 500000;
        private bool[] typeId = new bool[highestExpectedTypeid];
        private int[] groupId = new int[highestExpectedTypeid];
        public int GroupID(int i) { return groupId[i]; }
        private string[] typeName = new string[highestExpectedTypeid];
        public string TypeName(int id) { return typeName[id]; }
        private string[] description = new string[highestExpectedTypeid];
        private string[] mass = new string[highestExpectedTypeid];
        private string[] volume = new string[highestExpectedTypeid];
        private decimal[] capacity = new decimal[highestExpectedTypeid];
        private int[] PortionSize = new int[highestExpectedTypeid];
        private int[] raceId = new int[highestExpectedTypeid];
        private decimal[] basePrice = new decimal[highestExpectedTypeid];
        private int[] published = new int[highestExpectedTypeid];
        private int[] marketGroupId = new int[highestExpectedTypeid];
        private int[] iconId = new int[highestExpectedTypeid];
        private int[] soundId = new int[highestExpectedTypeid];
        private int[] graphicId = new int[highestExpectedTypeid];

        public Types()
        {
            StreamReader sr = new StreamReader("invTypes.txt");
            string line = sr.ReadLine();
            line = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                int id; id = Convert.ToInt32(line.Substring(0, line.IndexOf("	"))); line = line.Substring(line.IndexOf("	") + 1);
                typeId[id] = true;
                groupId[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	"))); line = line.Substring(line.IndexOf("	") + 1);
                typeName[id] = line.Substring(0, line.IndexOf("	")); line = line.Substring(line.IndexOf("	") + 1);
                while (true)
                {
                    if (line.Contains("	")) { description[id] = line.Substring(0, line.IndexOf("	")); line = line.Substring(line.IndexOf("	") + 1); break; }
                    else line = line + sr.ReadLine();
                }
                mass[id] = line.Substring(0, line.IndexOf("	")); line = line.Substring(line.IndexOf("	") + 1);
                volume[id] = line.Substring(0, line.IndexOf("	")); line = line.Substring(line.IndexOf("	") + 1);
                capacity[id] = Convert.ToDecimal(line.Substring(0, line.IndexOf("	"))); line = line.Substring(line.IndexOf("	") + 1);
                PortionSize[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	"))); line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) raceId[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) basePrice[id] = Convert.ToDecimal((line.Substring(0, line.IndexOf("	"))));
                line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) published[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) marketGroupId[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) iconId[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) soundId[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                line = line.Substring(line.IndexOf("	") + 1);
                if (line.IndexOf("	") > 0) graphicId[id] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                line = line.Substring(line.IndexOf("	") + 1);
                line = sr.ReadLine();
            }
        }
    }
}
