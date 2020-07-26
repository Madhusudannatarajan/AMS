using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.ViewModel
{
    public class AttendancePersantageVM
    {
        public int StuidVM { get; set; }
        public string StuNameVM { get; set; }
        public int TotallClassVM { get; set; }
        public int PresentVM { get; set; }
        public int AbsentVM { get; set; }
        public double PersantageVM { get; set; }
    }
}
