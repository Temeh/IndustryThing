using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public class Asset
    {
        public long item_id { get; set; }
        public long location_id { get; set; }
        public string location_flag { get; set; }
        public string location_type { get; set; }
        public int type_id { get; set; }
        public int quantity { get; set; }
        public bool is_singleton { get; set; }
    }
}
