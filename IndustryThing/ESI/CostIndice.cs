using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public class CostIndice
    {
        public int solar_system_id { get; set; }
        public CostIndex[] cost_indices { get; set; }

        #region Easy access
        public decimal copying { get { return getIndex(nameof(copying)); } }
        public decimal duplicating { get { return getIndex(nameof(duplicating)); } }
        public decimal invention { get { return getIndex(nameof(invention)); } }
        public decimal manufacturing { get { return getIndex(nameof(manufacturing)); } }
        public decimal none { get { return getIndex(nameof(none)); } }
        public decimal reaction { get { return getIndex(nameof(reaction)); } }
        public decimal researching_material_efficiency { get { return getIndex(nameof(researching_material_efficiency)); } }
        public decimal researching_technology { get { return getIndex(nameof(researching_technology)); } }
        public decimal researching_time_efficiency { get { return getIndex(nameof(researching_time_efficiency)); } }
        public decimal reverse_engineering { get { return getIndex(nameof(reverse_engineering)); } }

        private decimal getIndex(string activity)
        {
            foreach (var index in cost_indices)
                if (index.activity == activity)
                    return index.cost_index;

            return 0;
        }
        #endregion
    }

    public class CostIndex
    {
        public string activity { get; set; }
        public decimal cost_index { get; set; }
    }
}
