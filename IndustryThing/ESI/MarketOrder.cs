using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public class CharacterMarketORder : PrivateMarketOrder
    {
        public bool is_corporation { get; set; }
    }

    public class CorporationMarketOrder : PrivateMarketOrder
    {
        public int wallet_division { get; set; }
    }

    public abstract class PrivateMarketOrder : PublicMarketOrder
    {
        public decimal? escrow { get; set; }
        public int region_id { get; set; }
    }

    public class RegionMarketOrder : PublicMarketOrder
    {
        public int system_id { get; set; }
    }

    public abstract class PublicMarketOrder
    {
        public int duration { get; set; }
        public bool? is_buy_order { get; set; }
        public DateTime issued { get; set; }
        public long location_id { get; set; }
        public int min_volume { get; set; }
        public long order_id { get; set; }
        public decimal price { get; set; }
        public string range { get; set; }
        public int type_id { get; set; }
        public int volume_remain { get; set; }
        public int volume_total { get; set; }
    }

    public class RegionMarketHistory
    {
        public decimal average { get; set; }
        public DateTime date { get; set; }
        public decimal highest { get; set; }
        public decimal lowest { get; set; }
        public long order_count { get; set; }
        public long volume { get; set; }
    }
}
