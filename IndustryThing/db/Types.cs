using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.db
{
    class Types
    {
        static long highestExpectedTypeid = 500000;
        private bool[] typeId = new bool[highestExpectedTypeid];
        private int[] groupId = new int[highestExpectedTypeid];
        /// <summary>
        /// Finds an items group ID
        /// </summary>
        /// <param name="i">typeID</param>
        /// <returns>GroupID</returns>
        public int GroupID(int i) { return groupId[i]; }
        private string[] typeName = new string[highestExpectedTypeid];
        public string TypeName(long id) { return typeName[id]; }
        private string[] description = new string[highestExpectedTypeid];
        private string[] mass = new string[highestExpectedTypeid];
        private decimal[] volume = new decimal[highestExpectedTypeid];
        public decimal Volume(int id) { return volume[id]; }
        private decimal[] capacity = new decimal[highestExpectedTypeid];
        private int[] PortionSize = new int[highestExpectedTypeid];
        private int[] raceId = new int[highestExpectedTypeid];
        private decimal[] basePrice = new decimal[highestExpectedTypeid];
        private int[] published = new int[highestExpectedTypeid];
        private int[] marketGroupId = new int[highestExpectedTypeid];
        private int[] iconId = new int[highestExpectedTypeid];
        private int[] soundId = new int[highestExpectedTypeid];
        private int[] graphicId = new int[highestExpectedTypeid];
        private VolumeOverrideByGroupID volumeOverride;

        public Types()
        {
            StreamReader sr = new StreamReader(StaticInfo.installDir+"\\files\\invTypes.txt");
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
                volume[id] = Convert.ToDecimal(line.Substring(0, line.IndexOf("	"))); line = line.Substring(line.IndexOf("	") + 1);
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
            volumeOverride = new VolumeOverrideByGroupID();
        }

        /// <summary>
        /// This method gets the volume of a repackaged item. If it cant find a spesified volume for the item, it returns the volume from invTypes.
        /// </summary>
        /// <param name="groupId">takes the itemID</param>
        /// <returns>Volume</returns>
        public decimal GetRepackagedVolume(int itemID)
        {
            int i = 0;
            while (i < volumeOverride.groupID.Length)
            {
                if (GroupID(itemID) == volumeOverride.groupID[i]) return volumeOverride.volume[i];
                i++;
            }
            return Volume(itemID);
        }
    }

    /// <summary>
    /// This class reads a text file in the root directory for a list of items that have smaller size when repackaged.
    /// the volume is linked to group ID's.
    /// </summary>
    class VolumeOverrideByGroupID
    {
        public int[] groupID;
        public decimal[] volume;

        public VolumeOverrideByGroupID()
        {
            groupID = new int[100];
            volume = new decimal[100];
            StreamReader sr = new StreamReader(StaticInfo.installDir+"\\files\\ItemPackagedVolume.txt");
            int i = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                groupID[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                volume[i] = Convert.ToDecimal(line.Substring(line.IndexOf("	") + 1));
                i++; 
            }

            //just fixes the size of the arrays
            int[] tempGroupID = new int[i];
            decimal[] tempVolume = new decimal[i];
            int j = 0;
            while (j < i)
            {
                tempGroupID[j] = groupID[j];
                tempVolume[j] = volume[j];
                j++;
            }
            groupID = tempGroupID;
            volume = tempVolume;
        }
    }
}
