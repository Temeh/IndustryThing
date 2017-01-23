using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IndustryThing.calculator
{
    class Calculator
    {
        db.Db dataBase;
        Market.Market market;
        ApiImport.MainImport apiImports;
        int[,] t1Modules;
        int[,] t1ships;
        int[,] planetaryComodities;
        int[,] tools;
        int[,] minerals;
        int[,] constructionComponents;
        int[,] compopsites; //advanced materials

        public Calculator()
        {
            dataBase = new db.Db();
            apiImports = new ApiImport.MainImport(dataBase);
            market = new Market.Market(dataBase);
            T2Builder t2mods = new T2Builder(dataBase, market);

            Output.Output output = new Output.Output(t2mods, dataBase, market,apiImports);
           
        }
        /// <summary>//this is on the way to removal?
        /// Takes a 2D array that contains all the items needed for building t2 items, and distributes them over several 2d arrays indicating 
        /// </summary>
        /// <param name="totalMats">first dimension is the item, second dimension is 0=ItemID, 1=amount</param>
        void SortMaterials(int[,] totalMats)
        {
            t1Modules = new int[256, 2]; int t1ModulesCount = 0;// categoryid=7
            t1ships = new int[256, 2]; int t1shipsCount = 0;// categoryid=6
            planetaryComodities = new int[256, 2]; int planetaryComoditiesCount = 0;// categoryid=43
            tools = new int[256, 2]; int toolsCount = 0;// groupid=332
            minerals = new int[256, 2]; int mineralsCount = 0;// groupid=18
            constructionComponents = new int[256, 2]; int constructionComponentsCount = 0;// groupid=334
            compopsites = new int[256, 2]; int compopsitesCount = 0; // groupip=429
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
                if (dataBase.types.GroupID(id) == 429)
                {
                    compopsites[compopsitesCount, 0] = totalMats[i, 0];
                    compopsites[compopsitesCount, 1] = totalMats[i, 1];
                    compopsitesCount++;
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

        /// <summary>//also on the way to removal?
        /// Reduses the size of an array with empty space to a proper size
        /// </summary>
        /// <param name="mats">2d int array containing TypeID and amount</param>
        /// <param name="count">int to spesify size of new array</param>
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
