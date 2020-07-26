using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendenceManagementSystem.Models;

namespace AttendenceManagementSystem.Database
{
    public class dataContext : DbContext
    {
        public dataContext(DbContextOptions<dataContext> options)
              : base(options) { }

        public DbSet<Student> Student { get; set; }

        public DbSet<ClassName> ClassName { get; set; }

        public DbSet<ClassTime> ClassTime { get; set; }

        public DbSet<Guardian> Guardian { get; set; }

        public DbSet<Faculty> Faculty { get; set; }

        public DbSet<Course> Course { get; set; }

        public DbSet<ClassInfo> ClassInfo { get; set; }
        public DbSet<AttendanceList> AttendanceList { get; set; }
        public DbSet<ClasssStudentList> ClasssStudentList { get; set; }
        public DbSet<DateInfo> DateInfo { get; set; }
        public DbSet<MessageList> MessageList { get; set; }
    }
}
