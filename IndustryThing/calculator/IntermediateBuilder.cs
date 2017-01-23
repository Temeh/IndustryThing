using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.calculator
{
    class IntermediateBuilder : ProductionMethods
    {
        private db.Db dataBase;
        private Market.Market market;
        private long[][,] moduleMats;
        private long[,] totalMats;
        public long[,] TotalMats { get { return totalMats; } }
        private int[] moduleAmounts;
        private decimal materialModifier;
        private string rigTypeUsed;
        private decimal costOfMats;
        public decimal CostOfMats { get { return costOfMats; } }
        private decimal installCost;
        public decimal InstallCost { get { return installCost; } }

        public IntermediateBuilder(long[,] buildlist, db.Db data, string rigGroup, Market.Market market)
            : base(market, data)
        {
            this.market = market;
            rigTypeUsed = rigGroup;
            dataBase = data;
            int i = 0;
            moduleMats = new long[buildlist.Length / 2][,];
            while (i < (buildlist.Length / 2))
            {
              //  buildlist[i, 0] = FindBpoTypeIdForItem(Convert.ToInt32(buildlist[i, 0]));
                i++;
            }
            moduleMats = GetMaterialBill(buildlist);
            totalMats = TotalModuleMaterials(moduleMats);
            costOfMats = FindCosts(totalMats);
        }

        /// <summary>
        /// takes a 2d int with bpoID's and amount of units needed and produces a list of materials needed
        /// </summary>
        /// <returns></returns>
        protected long[][,] GetMaterialBill(long[,] buildList)
        {
            int i = 0;
            int[] moduleAmounts = new int[buildList.Length / 2];
            long[][,] moduleMats = new long[buildList.Length / 2][,];
            while (i < buildList.Length / 2) // adds amounts
            {
                int bpoid =dataBase.bpo. FindBpoTypeIdForItem(Convert.ToInt32(buildList[i, 0]));
                moduleAmounts[i] = Convert.ToInt32(buildList[i, 1]);
                int[,] temp = dataBase.bpo.ManufacturingMats(bpoid);
                moduleMats[i] = new long[temp.Length / 2, 2];
                for (int k = 0; k < temp.Length/2; k++)
                {
                    moduleMats[i][k, 0] = temp[k, 0];
                    moduleMats[i][k, 1] = temp[k, 1];
                }
                int j = 0;
                while (j < (moduleMats[i].Length / 2))
                {
                    if (moduleMats[i][j, 1] == 1) moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * 1.0));
                    else moduleMats[i][j, 1] = Convert.ToInt32(Math.Ceiling(moduleMats[i][j, 1] * moduleAmounts[i] * MaterialModifier()));
                    j++;
                }
                installCost += FindInstallCost(bpoid) * moduleAmounts[i];
                i++;
            }
            return moduleMats;
        }

        decimal MaterialModifier()
        {
            return dataBase.settings.FacilityMaterialModifier * dataBase.settings.RigMaterialModifier(rigTypeUsed) * dataBase.settings.MaterialEfficiencyModifier;
        }
    }
}
