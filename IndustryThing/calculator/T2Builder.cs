using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; // debugger

namespace IndustryThing.calculator
{
    class T2Builder : ProductionMethods
    {
        db.Db dataBase;
        Market.Market market;
        private int[][,] moduleMats;
        private int[,] totalModuleMats;
        public int[,] TotalModuleMats { get { return totalModuleMats; } }
        private int[] moduleAmounts;
        private decimal materialModifier;

        /// <summary>
        /// Second dimension: 0=typeID,1=output amount
        /// </summary>
        private int[,] output;
        public int[,] Output { get { return output; } }
        private string[] outputName;
        public string[] OutputName { get { return outputName; } }
        private decimal[] outputTotalValue;
        public decimal[] OutputTotalValue { get { return outputTotalValue; } }
        private decimal[] outputTotalCost;
        public decimal[] OutputTotalCost { get { return outputTotalCost; } }



        public T2Builder(db.Db dataBase_, Market.Market market)
            : base(market, dataBase_)
        {
            this.market = market;
            t1Modules = new long[256, 2]; // categoryid=7
            t1ships = new long[256, 2]; // categoryid=6
            planetaryComodities = new long[256, 2]; // categoryid=43
            tools = new long[256, 2]; // groupid=332
            minerals = new long[256, 2]; // groupid=18
            constructionComponents = new long[256, 2]; // groupid=334
            compopsites = new long[256, 2]; // groupip=429

            dataBase = dataBase_;
            moduleMats = new int[dataBase.t2bpoOwned.Index()][,];
            moduleAmounts = new int[dataBase.t2bpoOwned.Index()];

            output = new int[dataBase.t2bpoOwned.Index(), 2];
            outputName = new string[dataBase.t2bpoOwned.Index()];
            outputTotalValue = new decimal[dataBase.t2bpoOwned.Index()];
            outputTotalCost = new decimal[dataBase.t2bpoOwned.Index()];

            int i = 0;
            while (i < dataBase.t2bpoOwned.Index()) // adds amounts
            {
                int bpoid = dataBase.t2bpoOwned.Bpo(i);
                moduleAmounts[i] = GetBuildAmount(bpoid);

                moduleMats[i] = dataBase.bpo.ManufacturingMats(bpoid);
                materialModifier = MaterialModifier();

                int[,] bpoOutput = dataBase.bpo.ManufacturingOutput(bpoid);
                output[i, 0] = bpoOutput[0, 0]; output[i, 1] = bpoOutput[0, 1] * moduleAmounts[i];
                outputName[i] = dataBase.types.TypeName(output[i, 0]);
                outputTotalValue[i] = output[i, 1] * market.FindPrice(dataBase.settings.MarketRegion, "sell", output[i, 0]);
                int j = 0;
                while (j < (moduleMats[i].Length / 2))
                {
                    if (moduleMats[i][j, 1] == 1) moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * 1.0));
                    else moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * materialModifier));
                    j++;
                }
                //test variable
                decimal installcost = FindInstallCost(bpoid) * moduleAmounts[i];
                outputTotalCost[i] = SortMaterials(moduleMats[i]) + installcost;
                i++;
            }
            TotalModuleMaterials();
        }

        /// <summary>
        /// Takes the typeID of a bpo and returns the amount that can be built in a spesific timeframe with available bonuses
        /// </summary>
        /// <param name="i">item TypeID</param>
        int GetBuildAmount(int i)
        {
            // Works out the amount of items that can be built in the current timeframe when skills and facility bonuses are applied
            decimal industry = 1 - Convert.ToDecimal(0.04) * dataBase.settings.IndustryLevel;
            decimal advIndustry = 1 - Convert.ToDecimal(0.03) * dataBase.settings.AdvancedIndustryLevel;
            decimal scienceSkillOne = 1 - Convert.ToDecimal(0.01) * dataBase.settings.ScienceSkillOneLevel;
            decimal scienceSkillTwo = 1 - Convert.ToDecimal(0.01) * dataBase.settings.ScienceSkillTwoLevel;
            decimal rigModifier = dataBase.settings.RigSpeedModifier("module");
            decimal facilityBonus = dataBase.settings.FacilitySpeedModifier;
            decimal timeEfficiency = dataBase.settings.TimeEfficiencyModifier;

            decimal time = dataBase.bpo.ManufacturingTime(i);
            time = time * industry * advIndustry * scienceSkillOne * scienceSkillTwo * rigModifier * facilityBonus * timeEfficiency; //applies bonuses to build time

            decimal timeframe = dataBase.settings.BuildCycle;
            int monthlyAmount = Convert.ToInt32(Math.Floor(timeframe / time));
            return monthlyAmount;
        }

        decimal MaterialModifier()
        {
            return dataBase.settings.FacilityMaterialModifier * dataBase.settings.RigMaterialModifier("module") * dataBase.settings.MaterialEfficiencyModifier;
        }

        void TotalModuleMaterials()
        {
            totalModuleMats = new int[256, 2];
            int count = 0;
            int i = 0; // variable showing what bpo we are adding
            while (i < moduleMats.Length)
            {
                int j = 0;
                while (j < moduleMats[i].Length / 2)
                {
                    if (CheckForExistingItem(moduleMats[i][j, 0])) { totalModuleMats[FindItemLocation(moduleMats[i][j, 0]), 1] += moduleMats[i][j, 1]; }
                    else { totalModuleMats[count, 0] = moduleMats[i][j, 0]; totalModuleMats[count, 1] = moduleMats[i][j, 1]; count++; }
                    j++;
                }
                i++;
            }
            int[,] temp = new int[count, 2];
            i = 0;
            while (i < count)
            {
                temp[i, 0] = totalModuleMats[i, 0];
                temp[i, 1] = totalModuleMats[i, 1];
                i++;
            }
            totalModuleMats = temp;
        }
        /// <summary>
        /// Checks if an item exists in the totalModuleMats array
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckForExistingItem(int id)
        {
            int i = 0;
            while (i < (totalModuleMats.Length / 2))
            {
                if (totalModuleMats[i, 0] == id)
                { return true; }
                i++;
            }
            return false;
        }

        /// <summary>
        /// Finds an items location in the totalModuleMats array
        /// </summary>
        /// <returns></returns>
        int FindItemLocation(int id)
        {
            int i = 0;
            while (i < (totalModuleMats.Length / 2))
            {
                if (totalModuleMats[i, 0] == id) return i;
                i++;
            }
            return -1;// this is probably bad code but should never be triggered!
        }

        /// <summary>
        /// Takes a 2D array that contains all the items needed for building a t2 item, and distributes them over several 2d arrays indicating what group they belong to.
        /// It also works out the buld needs of all the intermediates and then returns the total cost of the raw materials needed to build the t2 item that spawned the initial 2d array
        /// </summary>
        /// <param name="totalMats">first dimension is the item, second dimension is: 0=ItemID, 1=amount</param>
        decimal SortMaterials(int[,] totalMats)
        {
            decimal cost = 0;
            long[,] t1Modules = new long[256, 2]; int t1ModulesCount = 0;// categoryid=7(mods), categoryid=8(charges), categoryid=18(drones), categoryid=22(deployables)
            long[,] t1ships = new long[256, 2]; int t1shipsCount = 0;// categoryid=6
            long[,] planetaryComodities = new long[256, 2]; int planetaryComoditiesCount = 0;// categoryid=43
            long[,] tools = new long[256, 2]; int toolsCount = 0;// groupid=332
            long[,] minerals = new long[256, 2]; int mineralsCount = 0;// groupid=18
            long[,] constructionComponents = new long[256, 2]; int constructionComponentsCount = 0;// groupid=334
            long[,] compopsites = new long[256, 2]; int compopsitesCount = 0; // groupip=429 (advanced materials)
            int i = 0;
            while (i < totalMats.Length / 2)
            {
                int id = totalMats[i, 0];
                int categoryID = dataBase.groupIDs.CategoryID(dataBase.types.GroupID(id));
                if ((categoryID == 7) || (categoryID == 8) || (categoryID == 18) || (categoryID == 22))
                {
                    t1Modules[t1ModulesCount, 0] = totalMats[i, 0]; t1Modules[t1ModulesCount, 1] = totalMats[i, 1]; t1ModulesCount++;
                }
                if (dataBase.groupIDs.CategoryID(dataBase.types.GroupID(id)) == 6)
                {
                    t1ships[t1shipsCount, 0] = totalMats[i, 0]; t1ships[t1shipsCount, 1] = totalMats[i, 1]; t1shipsCount++;
                }
                if (dataBase.groupIDs.CategoryID(dataBase.types.GroupID(id)) == 43)
                {
                    planetaryComodities[planetaryComoditiesCount, 0] = totalMats[i, 0]; planetaryComodities[planetaryComoditiesCount, 1] = totalMats[i, 1]; planetaryComoditiesCount++;
                }
                if (dataBase.types.GroupID(id) == 332)
                {
                    tools[toolsCount, 0] = totalMats[i, 0]; tools[toolsCount, 1] = totalMats[i, 1]; toolsCount++;
                }
                if (dataBase.types.GroupID(id) == 18)
                {
                    minerals[mineralsCount, 0] = totalMats[i, 0]; minerals[mineralsCount, 1] = totalMats[i, 1]; mineralsCount++;
                }
                if (dataBase.types.GroupID(id) == 334)
                {
                    constructionComponents[constructionComponentsCount, 0] = totalMats[i, 0]; constructionComponents[constructionComponentsCount, 1] = totalMats[i, 1]; constructionComponentsCount++;
                }
                if (dataBase.types.GroupID(id) == 429)
                {
                    compopsites[compopsitesCount, 0] = totalMats[i, 0]; compopsites[compopsitesCount, 1] = totalMats[i, 1]; compopsitesCount++;
                }
                i++;
            }
            //cleans up the size of the arrays so they dont have redundant space
            t1Modules = itemArrayCleanup(t1Modules, t1ModulesCount); t1ships = itemArrayCleanup(t1ships, t1shipsCount);
            planetaryComodities = itemArrayCleanup(planetaryComodities, planetaryComoditiesCount); tools = itemArrayCleanup(tools, toolsCount);
            minerals = itemArrayCleanup(minerals, mineralsCount); constructionComponents = itemArrayCleanup(constructionComponents, constructionComponentsCount);
            compopsites = itemArrayCleanup(compopsites, compopsitesCount);

            IntermediateBuilder t2Components = new IntermediateBuilder(constructionComponents, dataBase, "component", market);
            cost = t2Components.CostOfMats; cost += t2Components.InstallCost;
            IntermediateBuilder t1Mods = new IntermediateBuilder(t1Modules, dataBase, "module", market);
            cost += t1Mods.CostOfMats; cost += t1Mods.InstallCost;
            IntermediateBuilder t1Shipsthing = new IntermediateBuilder(t1ships, dataBase, "ship", market);
            cost += t1Shipsthing.CostOfMats; cost += t1Shipsthing.InstallCost;
            IntermediateBuilder toolmaker = new IntermediateBuilder(tools, dataBase, "component", market);
            cost += toolmaker.CostOfMats; cost += toolmaker.InstallCost;
            cost = cost + FindCosts(planetaryComodities) + FindCosts(minerals) + FindCosts(compopsites);

            compopsites = TotalModuleMaterials(new long[2][,] { compopsites, t2Components.TotalMats });
            minerals = TotalModuleMaterials(new long[2][,] { minerals, t1Mods.TotalMats });
            minerals = TotalModuleMaterials(new long[2][,] { minerals, t1Shipsthing.TotalMats });
            minerals = TotalModuleMaterials(new long[2][,] { minerals, toolmaker.TotalMats });

            this.t1Modules =TotalModuleMaterials(new long[2][,] { this.t1Modules, t1Modules });
            this.t1ships = TotalModuleMaterials(new long[2][,] { this.t1ships, t1ships });
            this.planetaryComodities = TotalModuleMaterials(new long[2][,] { this.planetaryComodities, planetaryComodities });
            this.tools = TotalModuleMaterials(new long[2][,] { this.tools, tools });
            this.minerals = TotalModuleMaterials(new long[2][,] { this.minerals, minerals });
            this.constructionComponents = TotalModuleMaterials(new long[2][,] { this.constructionComponents, constructionComponents });
            this.compopsites = TotalModuleMaterials(new long[2][,] { this.compopsites, compopsites });

            return cost;
        }

        /// <summary>
        /// Reduses the size of an array with empty space to a proper size
        /// </summary>
        /// <param name="mats">2d int array containing TypeID and amount</param>
        /// <param name="count">int to spesify size of new array</param>
        /// <returns></returns>
        long[,] itemArrayCleanup(long[,] mats, int count)
        {
            long[,] temp = new long[count, 2];
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
