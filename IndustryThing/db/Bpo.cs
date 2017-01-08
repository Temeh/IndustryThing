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
        static int highestItemId = 50000; //highest expected itemId to be found in the file

        string[] itemName = new string[highestItemId];

        int[][,] copyMats = new int[highestItemId][,];
        int[][,] copySkills = new int[highestItemId][,];
        int[] copyTime = new int[highestItemId];

        int[][,] inventionMats = new int[highestItemId][,];
        int[][,] inventionOutput = new int[highestItemId][,];
        int[][,] inventionSkills = new int[highestItemId][,];
        int[] inventionTime = new int[highestItemId];

        private int[][,] manufacturingMats = new int[highestItemId][,];
        public int[,] ManufacturingMats(int id) { return manufacturingMats[id].Clone() as int[,]; }
        private int[][,] manufacturingOutput = new int[highestItemId][,];
        public int[,] ManufacturingOutput(int id) { return manufacturingOutput[id]; }
        int[][,] manufacturingSkills = new int[highestItemId][,];
        private int[] manufacturingTime = new int[highestItemId];
        public int ManufacturingTime(int id) { return manufacturingTime[id]; }

        int[][,] researchMEMats = new int[highestItemId][,];
        int[][,] researchMESkills = new int[highestItemId][,];
        int[] researchMETime = new int[highestItemId];

        int[][,] researchTEMats = new int[highestItemId][,];
        int[][,] researchTESkills = new int[highestItemId][,];
        int[] researchTETime = new int[highestItemId];

        static public int[] maxProductionLimit = new int[highestItemId];
        string line;
        StreamReader sr = new StreamReader("blueprints.yaml");

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
    }
}