using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustryThing.calculator
{
    /// <summary>
    /// input1 is the database, input 2 is the rig type thats applied to this group of items built
    /// a class to be used as a base for classes does the calculation for material requirements for various groups of items
    /// </summary>
    class ProductionMethods
    {
        Market.Market market;
        db.Db dataBase;

      protected long[,] t1Modules;
      protected long[,] t1ships;
      protected long[,] planetaryComodities;
      protected long[,] tools;
      protected long[,] minerals;
      protected long[,] constructionComponents;
      protected long[,] compopsites; //advanced materials

        public ProductionMethods(Market.Market market, db.Db dataBase)
        {
            this.dataBase = dataBase;
            this.market = market;
        }

        /// <summary>
        /// Takes an array of 2d arrays containing materials bill for each bpo and compiles a single list of mats needed.
        /// </summary>
        /// <param name="moduleMats"></param>
        public long[,] TotalModuleMaterials(long[][,] moduleMats)
        {
            long[,] totalModuleMats = new long[256, 2];
            int count = 0;
            int i = 0; // variable showing what bpo we are adding
            while (i < moduleMats.Length)
            {
                int j = 0;
                while (j < moduleMats[i].Length / 2)
                {
                    if (CheckForExistingItem(Convert.ToInt32(moduleMats[i][j, 0]), totalModuleMats))
                    {
                        totalModuleMats[FindItemLocation(Convert.ToInt32(moduleMats[i][j, 0]), totalModuleMats), 1] += moduleMats[i][j, 1];
                    }
                    else { totalModuleMats[count, 0] = moduleMats[i][j, 0]; totalModuleMats[count, 1] = moduleMats[i][j, 1]; count++; }
                    j++;
                }
                i++;
            }
            long[,] temp = new long[count, 2];
            i = 0;
            while (i < count)
            {
                temp[i, 0] = totalModuleMats[i, 0];
                temp[i, 1] = totalModuleMats[i, 1];
                i++;
            }
            return temp;
        }

        /// <summary>
        /// Checks if an item exists in the totalModuleMats array
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckForExistingItem(int id, long[,] totalModuleMats)
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
        private int FindItemLocation(int id, long[,] totalModuleMats)
        {
            int i = 0;
            while (i < (totalModuleMats.Length / 2))
            {
                if (totalModuleMats[i, 0] == id) return i;
                i++;
            }
            return -1;// this is probably bad code but should never be triggered!
        }

        protected decimal FindCosts(long[,] materials)
        {
            int i = 0; decimal cost = 0;
            while (i < materials.Length / 2)
            {
                decimal temp = market.FindPrice(dataBase.settings.MarketRegion, "buy", materials[i, 0]) * materials[i, 1];
                cost += market.FindPrice(dataBase.settings.MarketRegion, "buy", materials[i, 0]) * materials[i, 1];
                i++;
            }
            return cost;
        }

        /// <summary>
        /// Takes a bpoID and finds the install cost for one run.
        /// </summary>
        /// <param name="mats">the bpoID</param>
        /// <returns>The install cost</returns>
        protected decimal FindInstallCost(int bpoID)
        {
            //finds baseCost of an item
            decimal baseCost = 0;
            int[,] mats = dataBase.bpo.ManufacturingMats(bpoID);
            int i = 0;
            while (i < mats.Length / 2)
            {
                baseCost += market.FindAdjustedPrice(mats[i, 0]) * mats[i, 1];
                i++;
            }
            // .95 modifier comes from sotiyo bonus
            decimal installCost = baseCost * Convert.ToDecimal(0.95)*market.FindSystemIndexManufacturing();
            installCost = installCost * ((100 + dataBase.settings.FacilityTax) / 100);

            return installCost;
        }

        /// <summary>
        /// Returns an array containing a spesific group in materials
        /// </summary>
        /// <param name="i">0=t1modules, 1=t1ships, 2=planetaryComodities, 3=tools, 4=minerals, 5=constructionComponents, 6=composites</param>
        /// <returns></returns>
        public long[,] GetGroup(long i)
        {
            if (i == 0) return t1Modules;
            else if (i == 1) return t1ships;
            else if (i == 2) return planetaryComodities;
            else if (i == 3) return tools;
            else if (i == 4) return minerals;
            else if (i == 5) return constructionComponents;
            else return compopsites;
        }
    }
}
