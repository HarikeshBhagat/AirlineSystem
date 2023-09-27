using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAgencies.EntityFrameWork
{
    public class Subscriptions
    {
        [Key]
        public int agency_id { get; set; }
        public int origin_city_id { get; set; }
        public int destination_city_id { get; set; }
    }
}
