using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.ViewModel
{
    public class AttendanceStudentListVM
    {        
        public int StudentIdVM { get; set; }
        public string StudentNameVM { get; set; }
        public int Status { get; set; }
        public int classIdVM { get; set; }
        public int courseIdVM { get; set; }
        public int  timeIdVM { get; set; }
        public DateTime dateVM { get; set; }
        
    }
}
