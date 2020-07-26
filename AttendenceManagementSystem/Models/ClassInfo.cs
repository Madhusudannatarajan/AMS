using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.Models
{
    public class ClassInfo
    {
        public int id { get; set; }
        public int ClassNameId { get; set; }
        public int TeacherId { get; set; }
        public int CourseNameId { get; set; }
        public int ClassTimeId { get; set; }

    }
}
