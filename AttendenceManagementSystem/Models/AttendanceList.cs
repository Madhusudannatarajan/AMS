using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.Models
{
    public class AttendanceList
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int ClassTimeId { get; set; }
        public int TeacherId { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }
    }
}
