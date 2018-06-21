using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public class MarketOrder
    {
        public long order_id { get; set; }
        public int character_id { get; set; } // nope
        public int station_id { get; set; } // nope
        public long location_id { get; set; }
        public int volume_total { get; set; }
        public int volume_remain { get; set; }
        public int state { get; set; } // nope
        public int type_id { get; set; }
        public string range { get; set; }
        public string account_key { get; set; } // nope
        public int wallet_division { get; set; }
        public int duration { get; set; }
        public decimal? escrow { get; set; }
        public decimal price { get; set; }
        public bool? is_buy_order { get; set; }
        public DateTime issued { get; set; }
    }
}
