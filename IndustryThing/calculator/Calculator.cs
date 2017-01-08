using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.calculator
{
    class Calculator
    {
        db.Db dataBase = new db.Db();
        int[,] t1Modules;
        int[,] t1ships;
        int[,] planetaryComodities;
        int[,] tools;
        int[,] minerals;
        int[,] constructionComponents;

        public Calculator()
        {
            T2mods t2mods = new T2mods(dataBase);

            SortMaterials(t2mods.TotalModuleMats);

            StreamWriter sw = new StreamWriter("testoutput.txt");
            int i = 0;
            while (i < t2mods.TotalModuleMats.Length / 2)
            {
                sw.WriteLine(dataBase.types.TypeName(t2mods.TotalModuleMats[i, 0]) + "	" + t2mods.TotalModuleMats[i, 1]);
                i++;
            }
            sw.Close();





         
            

          //  int i=0;
            sw.WriteLine("t1modules");
            while (i < t1Modules.Length / 2)
            {
                sw.WriteLine(dataBase.types.TypeName(t1Modules[i, 0]) + "	" + t1Modules[i, 1]);
                i++;
            }
             i = 0;
             sw.WriteLine("t1Ships");
             while (i < t1ships.Length / 2)
            {
                sw.WriteLine(dataBase.types.TypeName(t1ships[i, 0]) + "	" + t1ships[i, 1]);
                i++;
            }
             i = 0;
             sw.WriteLine("planetaryComodities");
             while (i < planetaryComodities.Length / 2)
             {
                 sw.WriteLine(dataBase.types.TypeName(planetaryComodities[i, 0]) + "	" + planetaryComodities[i, 1]);
                 i++;
             }
             i = 0;
             sw.WriteLine("tools");
             while (i < tools.Length / 2)
             {
                 sw.WriteLine(dataBase.types.TypeName(tools[i, 0]) + "	" + tools[i, 1]);
                 i++;
             }
             i = 0;
             sw.WriteLine("minerals");
             while (i < minerals.Length / 2)
             {
                 sw.WriteLine(dataBase.types.TypeName(minerals[i, 0]) + "	" + minerals[i, 1]);
                 i++;
             }
             i = 0;
             sw.WriteLine("constructionComponents");
             while (i < constructionComponents.Length / 2)
             {
                 sw.WriteLine(dataBase.types.TypeName(constructionComponents[i, 0]) + "	" + constructionComponents[i, 1]);
                 i++;
             }
            sw.Close();
        
        }
        /// <summary>
        /// Sorts the materials needed for t2 into their respective groups
        /// </summary>
        void SortMaterials(int[,] totalMats)
        {

           t1Modules = new int[256, 2]; int t1ModulesCount = 0;// categoryid=7
          t1ships = new int[256, 2]; int t1shipsCount = 0;// categoryid=6
           planetaryComodities = new int[256, 2]; int planetaryComoditiesCount = 0;// categoryid=43
          tools = new int[256, 2]; int toolsCount = 0;// groupid=332
           minerals = new int[256, 2]; int mineralsCount = 0;// groupid=18
           constructionComponents = new int[256, 2]; int constructionComponentsCount = 0;// groupid=334
            int i = 0;
            while (i < totalMats.Length / 2)
            {
                int id = totalMats[i, 0];
                if (dataBase.groupIDs.CategoryID(dataBase.types.GroupID(id)) == 7)
                {
                    t1Modules[t1ModulesCount, 0] = totalMats[i, 0];
                    t1Modules[t1ModulesCount, 1] = totalMats[i, 1];
                    t1ModulesCount++;
                }
                if (dataBase.groupIDs.CategoryID(dataBase.types.GroupID(id)) == 6)
                {
                    t1ships[t1shipsCount, 0] = totalMats[i, 0];
                    t1ships[t1shipsCount, 1] = totalMats[i, 1];
                    t1shipsCount++;
                }
                if (dataBase.groupIDs.CategoryID(dataBase.types.GroupID(id)) == 43)
                {
                    planetaryComodities[planetaryComoditiesCount, 0] = totalMats[i, 0];
                    planetaryComodities[planetaryComoditiesCount, 1] = totalMats[i, 1];
                    planetaryComoditiesCount++;
                }
                if (dataBase.types.GroupID(id) == 332)
                {
                    tools[toolsCount, 0] = totalMats[i, 0];
                    tools[toolsCount, 1] = totalMats[i, 1];
                    toolsCount++;
                }
                if (dataBase.types.GroupID(id) == 18)
                {
                    minerals[mineralsCount, 0] = totalMats[i, 0];
                    minerals[mineralsCount, 1] = totalMats[i, 1];
                    mineralsCount++;
                }
                if (dataBase.types.GroupID(id) == 334)
                {
                    constructionComponents[constructionComponentsCount, 0] = totalMats[i, 0];
                    constructionComponents[constructionComponentsCount, 1] = totalMats[i, 1];
                    constructionComponentsCount++;
                }
                i++;
            }
            t1Modules = itemArrayCleanup(t1Modules, t1ModulesCount);
            t1ships = itemArrayCleanup(t1ships, t1shipsCount);
            planetaryComodities = itemArrayCleanup(planetaryComodities, planetaryComoditiesCount);
            tools = itemArrayCleanup(tools, toolsCount);
            minerals = itemArrayCleanup(minerals, mineralsCount);
            constructionComponents = itemArrayCleanup(constructionComponents, constructionComponentsCount);
        }

        /// <summary>
        /// Reduses the size of an array with empty space to a proper size
        /// </summary>
        /// <param name="mats"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int[,] itemArrayCleanup(int[,] mats, int count)
        {
            int[,] temp = new int[count, 2];
            int i = 0;
            while (i < count)
            {
                temp[i, 0] = mats[i, 0];
                temp[i, 1] = mats[i, 1];
                i++;
            }
            return temp;
        }
    }
}
