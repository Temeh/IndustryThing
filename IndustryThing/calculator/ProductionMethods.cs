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

        public ProductionMethods()
        {

        }

        /// <summary>
        /// Takes an array of 2d arrays containing materials bill for each bpo and compiles a single list of mats needed.
        /// </summary>
        /// <param name="moduleMats"></param>
        public int[,] TotalModuleMaterials(int[][,] moduleMats)
        {
            int[,] totalModuleMats = new int[256, 2];
            int count = 0;
            int i = 0; // variable showing what bpo we are adding
            while (i < moduleMats.Length)
            {
                int j = 0;
                while (j < moduleMats[i].Length / 2)
                {
                    if (CheckForExistingItem(moduleMats[i][j, 0], totalModuleMats))
                    {
                        totalModuleMats[FindItemLocation(moduleMats[i][j, 0],totalModuleMats), 1] += moduleMats[i][j, 1];
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
            return temp;
        }

        /// <summary>
        /// Checks if an item exists in the totalModuleMats array
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CheckForExistingItem(int id, int[,]totalModuleMats)
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
        private int FindItemLocation(int id, int[,] totalModuleMats)
        {
            int i = 0;
            while (i < (totalModuleMats.Length / 2))
            {
                if (totalModuleMats[i, 0] == id) return i;
                i++;
            }
            return -1;// this is probably bad code but should never be triggered!
        }
    }
}
