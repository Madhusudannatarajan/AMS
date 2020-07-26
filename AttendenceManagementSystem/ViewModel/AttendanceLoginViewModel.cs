using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.ViewModel
{
    public class AttendanceLoginViewModel
    {
       [Required(ErrorMessage = "Date is required.")]
        public DateTime dateVM { get; set; }
        [Required(ErrorMessage = "Time is required.")]
        public int timeIdVM { get; set; }
        [Required(ErrorMessage = "Please select any class.")]
        public int classIdVM { get; set; }
        [Required(ErrorMessage = "Please select any course.")]
        public int courseIdVM { get; set; }
    }
}
