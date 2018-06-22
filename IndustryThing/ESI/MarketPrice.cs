using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public class MarketPrice
    {
        public int type_id { get; set; }
        public decimal? adjusted_price { get; set; }
        public decimal? average_price { get; set; }
    }
}
