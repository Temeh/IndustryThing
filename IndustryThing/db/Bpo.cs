using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace IndustryThing.db
{
    class Bpo : Misc.UsefullMethods
    {
       private static int highestItemId = 500000; //highest expected itemId to be found in the file

       private string[] itemName = new string[highestItemId];

       private int[][,] copyMats = new int[highestItemId][,];
       private int[][,] copySkills = new int[highestItemId][,];
       private int[] copyTime = new int[highestItemId];

       private int[][,] inventionMats = new int[highestItemId][,];
       private int[][,] inventionOutput = new int[highestItemId][,];
       private int[][,] inventionSkills = new int[highestItemId][,];
       private int[] inventionTime = new int[highestItemId];

        private int[][,] manufacturingMats = new int[highestItemId][,];
        public int[,] ManufacturingMats(int id) { return manufacturingMats[id].Clone() as int[,]; }
        /// <summary>
        /// [][,0] = typeID, [][,1] = quantity
        /// </summary>
        private int[][,] manufacturingOutput = new int[highestItemId][,];
        public int[,] ManufacturingOutput(int id)
        {
            if (manufacturingOutput[id] == null) return null;
            return manufacturingOutput[id].Clone() as int[,]; ;
        }
        int[][,] manufacturingSkills = new int[highestItemId][,];
        private int[] manufacturingTime = new int[highestItemId];
        public int ManufacturingTime(int id) { return manufacturingTime[id]; }

        private int[][,] researchMEMats = new int[highestItemId][,];
        private int[][,] researchMESkills = new int[highestItemId][,];
        private int[] researchMETime = new int[highestItemId];

        private int[][,] researchTEMats = new int[highestItemId][,];
        private int[][,] researchTESkills = new int[highestItemId][,];
        private int[] researchTETime = new int[highestItemId];

        static private int[] maxProductionLimit = new int[highestItemId];
        private string line;
        private StreamReader sr = new StreamReader("files\\blueprints.yaml");

        public Bpo()
        {
            line = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                if (!line.StartsWith(" "))
                {
                    int itemId = Convert.ToInt32(line.Substring(0, line.Length - 1));
                    line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                    while (true)
                    {

                        if (line == "activities:")
                        {
                            line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                            if (line == "copying:") { line = Reader(itemId, "copy"); }
                            if (line == "invention:") { line = Reader(itemId, "invent"); }
                            if (line == "manufacturing:") { line = Reader(itemId, "build"); }
                            if (line == "research_material:") { line = Reader(itemId, "ME"); }
                            if (line == "research_time:") { line = Reader(itemId, "TE"); }
                        }
                        else if (line.StartsWith("blueprintTypeID:")) { line = RemoveSpaceFromStartOfLine(sr.ReadLine()); }
                        else if (line.StartsWith("maxProductionLimit:"))
                        {
                            if (!sr.EndOfStream) line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                            else break;
                        }
                        else break;
                    }
                }
            }
        }

        string Reader(int itemId, string jobtype)
        {
            line = RemoveSpaceFromStartOfLine(sr.ReadLine());
            while (true)
            {
                if (line.StartsWith("materials:"))
                {
                    if (jobtype == "copy") copyMats[itemId] = ContentsReader();
                    else if (jobtype == "invent") inventionMats[itemId] = ContentsReader();
                    else if (jobtype == "build") manufacturingMats[itemId] = ContentsReader();
                    else if (jobtype == "ME") researchMEMats[itemId] = ContentsReader();
                    else if (jobtype == "TE") researchTEMats[itemId] = ContentsReader();
                }
                else if (line.StartsWith("products:"))
                {
                    if (jobtype == "copy") ContentsReader(); //Not logging anything here
                    else if (jobtype == "invent") inventionOutput[itemId] = ContentsReader();
                    else if (jobtype == "build") manufacturingOutput[itemId] = ContentsReader();
                    else if (jobtype == "ME") ContentsReader(); //Not logging anything here
                    else if (jobtype == "TE") ContentsReader(); //Not logging anything here
                }
                else if (line.StartsWith("skills:"))
                {
                    if (jobtype == "copy") copySkills[itemId] = ContentsReader();
                    else if (jobtype == "invent") inventionSkills[itemId] = ContentsReader();
                    else if (jobtype == "build") manufacturingSkills[itemId] = ContentsReader();
                    else if (jobtype == "ME") researchMESkills[itemId] = ContentsReader();
                    else if (jobtype == "TE") researchMESkills[itemId] = ContentsReader();
                }
                else if (line.StartsWith("time:"))
                {
                    int time = Convert.ToInt32(line.Substring(6));
                    if (jobtype == "copy") copyTime[itemId] = time;
                    else if (jobtype == "invent") inventionTime[itemId] = time;
                    else if (jobtype == "build") manufacturingTime[itemId] = time;
                    else if (jobtype == "ME") researchMETime[itemId] = time;
                    else if (jobtype == "TE") researchMETime[itemId] = time;
                    line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                }
                else return line;
            }
        }

        int[,] ContentsReader()
        {
            int[,] contents = new int[32, 2];
            int count = 0;
            line = RemoveSpaceFromStartOfLine(sr.ReadLine());
            while (true)
            {

                if (line.StartsWith("-"))
                {
                    if (line.StartsWith("-   probability")) line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                    contents[count, 1] = Convert.ToInt32(line.Substring(line.IndexOf(": ") + 2));
                    line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                    contents[count, 0] = Convert.ToInt32(line.Substring(line.IndexOf(": ") + 2));
                    count++;
                    line = RemoveSpaceFromStartOfLine(sr.ReadLine());
                }
                else
                {
                    int[,] contents_ = new int[count, 2];
                    int i = 0;
                    while (i < count)
                    {
                        contents_[i, 0] = contents[i, 0];
                        contents_[i, 1] = contents[i, 1];
                        i++;
                    }
                    return contents_;
                }
            }
        }

        /// <summary>
        /// Takes an item's TypeID and returns the TypeID of the bpo that can build this item.
        /// </summary>
        /// <param name="itemID">the typeID of the item being buildt</param>
        /// <returns>Total amount of items being output by all existing jobs</returns>
        public int FindBpoTypeIdForItem(int itemID)
        {
            int i = 0;
            while (true)
            {
                int[,] temp = ManufacturingOutput(i);
                if (!(temp == null)) if (itemID == temp[0, 0])
                    {
                        return i;
                    }
                i++;
            }
        }
    }
}