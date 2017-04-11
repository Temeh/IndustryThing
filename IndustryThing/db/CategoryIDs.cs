using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.db
{
    class CategoryIDs : Misc.UsefullMethods
    {
        private static int highestExpectedIDs = 500000;
        private string[][] name = new string[highestExpectedIDs][];
        /// <summary>
        /// 0=en, 1=de, 2=fr, 3=ja, 4=ru, 5=zh
        /// </summary>
        /// <param name="id">CategoryID</param>
        /// <param name="language">Language</param>
        /// <returns>Name</returns>
        public string GetName(int id, int language) { return name[id][language]; }
        private bool[] published = new bool[highestExpectedIDs];

        public CategoryIDs()
        {
            StreamReader sr = new StreamReader(StaticInfo.installDir+"\\files\\categoryIDs.yaml");
            string line = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                int groupID = Convert.ToInt32(line.Substring(0, line.Length - 1));
                line = sr.ReadLine();
                while (line.StartsWith(" "))
                {
                    line = RemoveSpaceFromStartOfLine(line);
                    if (line.StartsWith("published:")) { if (!(line.Substring(line.IndexOf(" ") + 1) == "false")) published[groupID] = true; }
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
