using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.calculator
{
    class T2mods
    {
        db.Db dataBase;
        private int[][,] moduleMats;
        private int[,] totalModuleMats;
        public int[,] TotalModuleMats { get { return totalModuleMats; } }
        private int[] moduleAmounts;
        private decimal materialModifier;

        public T2mods(db.Db dataBase_)
        {
            dataBase = dataBase_;
            moduleMats = new int[dataBase.t2bpoOwned.Index()][,];
            moduleAmounts = new int[dataBase.t2bpoOwned.Index()];
            int i = 0;
            while (i < dataBase.t2bpoOwned.Index()) // adds amounts
            {
                if (i == 20)// test code
                {
                    string test = "hjh";
                }
                int bpoid = dataBase.t2bpoOwned.Bpo(i);
                moduleAmounts[i] = GetBuildAmount(bpoid);
                moduleMats[i] = dataBase.bpo.ManufacturingMats(bpoid);
                materialModifier = MaterialModifier();
                int j = 0;
                while (j < (moduleMats[i].Length / 2))
                {
                    if (moduleMats[i][j, 1] == 1) moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * 1.0));

                    else moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * materialModifier));
                    j++;
                }
                i++;
            }
            TotalModuleMaterials();
        }
        /// <summary>
        /// Takes the item ID of a type and returns the amount that can be built in a spesific timeframe with available bonuses
        /// </summary>
        /// <param name="i"></param>
        int GetBuildAmount(int i)
        {
            // Works out the amount of items that can be built in the current timeframe when skills and facility bonuses are applied
            decimal industry = 1 - Convert.ToDecimal(0.04) * dataBase.settings.IndustryLevel;
            decimal advIndustry = 1 - Convert.ToDecimal(0.03) * dataBase.settings.AdvancedIndustryLevel;
            decimal scienceSkillOne = 1 - Convert.ToDecimal(0.01) * dataBase.settings.ScienceSkillOneLevel;
            decimal scienceSkillTwo = 1 - Convert.ToDecimal(0.01) * dataBase.settings.ScienceSkillTwoLevel;
            decimal rigModifier = dataBase.settings.ModuleRigSpeedModifier;
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
            return dataBase.settings.FacilityMaterialModifier * dataBase.settings.ModuleRigMaterialModifier * dataBase.settings.MaterialEfficiencyModifier;
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
                    if (CheckForExistingItem(moduleMats[i][j, 0])) { 
                        if(moduleMats[i][j, 0]==11399)// test code please delete
                        {
                            int test = moduleMats[i][j, 1];
                        }
                        totalModuleMats[FindItemLocation(moduleMats[i][j, 0]), 1] += moduleMats[i][j, 1]; 
                    }
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
        /// only used for testing things
        /// </summary>
        /// <param name="i"></param>
        void FindCategoryForBlueprintsOutputItem(int i)
        {
           // int outputItemId = dataBase.bpo.manufacturingOutput[i][0, 0];
           // int outputsGroup = dataBase.types.GroupID(outputItemId);
          //  string categoryname = dataBase.categoryIDs.Name(dataBase.groupIDs.CategoryID(dataBase.types.GroupID(outputItemId)), 0);
        }
    }
}
