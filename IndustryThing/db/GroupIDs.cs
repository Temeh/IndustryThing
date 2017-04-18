using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.db
{
    class GroupIDs : Misc.UsefullMethods
    {
        private static int highestExpectedIDs = 500000;
        private bool[] anchorable = new bool[highestExpectedIDs];
        private bool[] anchored = new bool[highestExpectedIDs];
        private int[] categoryID = new int[highestExpectedIDs];
        /// <summary>
        /// takes the groupID and finds the items Category ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CategoryID(int id) { return categoryID[id]; }
        private bool[] fittableNonSingleton = new bool[highestExpectedIDs];
        private string[][] name = new string[highestExpectedIDs][];
        /// <summary>
        /// Shows the name of the group
        /// input one = groupID
        /// input two = language: 0=en, 1=de, 2=fr, 3=ja, 4=ru, 5=zh
        /// </summary>
        public string Name(int groupID, int language) { return name[groupID][language]; }
        private bool[] published = new bool[highestExpectedIDs];
        private bool[] useBasePrice = new bool[highestExpectedIDs];

        public GroupIDs()
        {
            StreamReader sr = new StreamReader("files\\groupIDs.yaml");
            string line = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                int groupID = Convert.ToInt32(line.Substring(0, line.Length - 1));
                line = sr.ReadLine();
                while (line.StartsWith(" "))
                {
                    line = RemoveSpaceFromStartOfLine(line);
                    if (line.StartsWith("anchorable:")) { if (!(line.Substring(line.IndexOf(" ") + 1) == "false")) anchorable[groupID] = true; }
                    else if (line.StartsWith("anchored:")) { if (!(line.Substring(line.IndexOf(" ") + 1) == "false")) anchored[groupID] = true; }
                    else if (line.StartsWith("categoryID:")) { categoryID[groupID] = Convert.ToInt32(line.Substring(line.IndexOf(" ") + 1)); }
                    else if (line.StartsWith("fittableNonSingleton:")) { if (!(line.Substring(line.IndexOf(" ") + 1) == "false")) fittableNonSingleton[groupID] = true; }
                    else if (line.StartsWith("published:")) { if (!(line.Substring(line.IndexOf(" ") + 1) == "false")) published[groupID] = true; }
                    else if (line.StartsWith("useBasePrice:")) { if (!(line.Substring(line.IndexOf(" ") + 1) == "false")) useBasePrice[groupID] = true; }
                    if (line.StartsWith("name:"))
                    {
                        line = sr.ReadLine();
                        name[groupID] = new string[6];
                        while (line.StartsWith("        "))
                        {
                            line = RemoveSpaceFromStartOfLine(line);
                            if (line.StartsWith("en:")) name[groupID][0] = line.Substring(line.IndexOf(": ") + 2);
                            else if (line.StartsWith("de:")) name[groupID][1] = line.Substring(line.IndexOf(": ") + 2);
                            else if (line.StartsWith("fr:")) name[groupID][2] = line.Substring(line.IndexOf(": ") + 2);
                            else if (line.StartsWith("ja:")) name[groupID][3] = line.Substring(line.IndexOf(": ") + 2);
                            else if (line.StartsWith("ru:")) name[groupID][4] = line.Substring(line.IndexOf(": ") + 2);
                            else if (line.StartsWith("zh:")) name[groupID][5] = line.Substring(line.IndexOf(": ") + 2);
                            line = sr.ReadLine();
                        }

                    }
                    else line = sr.ReadLine();
                    if (sr.EndOfStream) break;
                }
            }
        }
    }
}
