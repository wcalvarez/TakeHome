using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class BusinessHour
    {
        public int BusinessHourID { get; set; }
        public int LocationID { get; set; }
        public bool Open { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CloseTime { get; set; }
        public string WeekDay { get; set; }
        public int WeekDayNumber { get; set; }
        public int PickupLeadTime { get; set; }
        public int LastOrderLeadTime { get; set; }
    }
}
