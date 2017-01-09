using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.calculator
{
    class IntermediateBuilds:ProductionMethods
    {
        private db.Db dataBase;
        private int[][,] moduleMats;
        private int[,] totalModuleMats;
        public int[,] TotalModuleMats { get { return totalModuleMats; } }
        private int[] moduleAmounts;
        private decimal materialModifier;
        private string rigTypeUsed;

        public IntermediateBuilds(int[,] buildlist, db.Db data, string rigGroup)
        {
            rigTypeUsed = rigGroup;
            dataBase = data;
            int i = 0;
            moduleMats = new int[buildlist.Length/2][,];
            while (i < (buildlist.Length / 2))
            {
                buildlist[i, 0] = FindBpoTypeIdForItem(buildlist[i, 0]);
                i++;
            }
            moduleMats = GetMaterialBill(buildlist);
            totalModuleMats = TotalModuleMaterials(moduleMats);
        }

        /// <summary>
        /// takes a 2d int with bpoID's and amount of units needed and produces a list of materials needed
        /// </summary>
        /// <returns></returns>
        protected int[][,] GetMaterialBill(int[,] buildList)
        {
            int i = 0;
            int[] moduleAmounts = new int[buildList.Length / 2];
            int[][,] moduleMats = new int[buildList.Length / 2][,];
            while (i < buildList.Length / 2) // adds amounts
            {
                int bpoid = buildList[i, 0];
                moduleAmounts[i] = buildList[i, 1];
                moduleMats[i] = dataBase.bpo.ManufacturingMats(bpoid);
                int j = 0;
                while (j < (moduleMats[i].Length / 2))
                {
                    if (moduleMats[i][j, 1] == 1) moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * 1.0));

                    else moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * MaterialModifier()));
                    j++;
                }
                i++;
            }
            return moduleMats;
        }

        decimal MaterialModifier()
        {
            return dataBase.settings.FacilityMaterialModifier * dataBase.settings.RigMaterialModifier(rigTypeUsed) * dataBase.settings.MaterialEfficiencyModifier;
        }

        /// <summary>
        /// Takes an item's TypeID and returns the TypeID of the bpo that can build this item.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        protected int FindBpoTypeIdForItem(int itemID)
        {
            int i = 0;
            while (true)
            {
                int[,] temp = dataBase.bpo.ManufacturingOutput(i);
                if (!(temp == null)) if (itemID == temp[0, 0])
                    {
                        return i;
                    }
                i++;
            }
        }

    }
}
