using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.db
{
    class T2ModBpoOwned
    {
        private int[] bpo;
        public int Bpo(int index) { return bpo[index]; }
        private string[] name;
        public string Name(int i) { return name[i]; }
        public int Index() { return bpo.Length; }

        public T2ModBpoOwned()
        {
            StreamReader sr = new StreamReader(StaticInfo.installDir+"t2ModBpoOwned.txt");
            int i = 0;
            while (!sr.EndOfStream)
            {
                i++;
                string line = sr.ReadLine();
            }
            bpo = new int[i];
            name = new string[i];
            i = 0;
            sr = new StreamReader(StaticInfo.installDir+"t2ModBpoOwned.txt");
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                bpo[i] = Convert.ToInt32(line.Substring(0, line.IndexOf("	")));
                name[i] = line.Substring(line.IndexOf("	") + 1);
                i++;
            }
        }
    }
}
