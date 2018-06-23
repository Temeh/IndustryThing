using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public class IndustryJob
    {
        public int job_id { get; set; }
        public int installer_id { get; set; }
        public string installer_name { get; set; } // nope
        public long facility_id { get; set; }
        public int soler_system_id { get; set; } // nope
        public string solar_sysetm_name { get; set; } // nope
        public long station_id { get; set; } // nope
        public long location_id { get; set; }
        public int activity_id { get; set; }
        public long blueprint_id { get; set; }
        public int blueprint_type_id { get; set; }
        public string blueprint_type_name { get; set; } // nope
        public long blueprint_location_id { get; set; }
        public long output_location_id { get; set; }
        public int runs { get; set; }
        public decimal? cost { get; set; }
        public int? licensed_runs { get; set; }
        public decimal? probability { get; set; }
        public int? product_type_id { get; set; }
        public string product_type_name { get; set; } // nope
        public string status { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public double time_in_seconds { get { return (end_date - start_date).TotalSeconds; } }
        public DateTime? pause_date { get; set; }
        public DateTime? completed_date { get; set; }
        public int? completed_character_id { get; set; }
        public int? succesful_runs { get; set; }
    }
}
