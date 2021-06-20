using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdessoRideShare.model.entities
{
    public class TravelPlan
    {
        [Key]
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int NumberOfSeats { get; set; }
        public DateTime TravelDate { get; set; }
        public string Explanation { get; set; }
        public bool IsOnAir { get; set; }
    }
}
