using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceManagementSystem.Models
{
    public class MessageList
    {
        public int Id { get; set; }
        public string MessageTitle { get; set; }        
        public string MessageDetail { get; set; }

        public int TeacherId { get; set; }
        public int GuardianId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string SentBy { get; set; }
    }
}
