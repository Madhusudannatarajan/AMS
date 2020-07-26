using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.ViewModel
{
    public class DateWiseAttendanceListVM
    {
        public DateTime DateVM { get; set; }
        public string TimeVM { get; set; }
        public string StatusVM { get; set; }
        public string EditStatusVM { get; set; }
    }
}
