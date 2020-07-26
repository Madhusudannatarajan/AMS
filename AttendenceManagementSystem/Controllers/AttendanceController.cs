using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendenceManagementSystem.Database;
using AttendenceManagementSystem.Models;
using AttendenceManagementSystem.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AttendenceManagementSystem.Controllers
{
    public class AttendanceController : Controller
    {
        public readonly dataContext db;
        public AttendanceController(dataContext x)
        {
            db = x;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("Faculty", "Expired");
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Faculty Model)
        {
            var i = db.Faculty.Where(s => s.UserName == Model.UserName && s.Password == Model.Password).FirstOrDefault();
            if (i != null)
            {
                ViewBag.S = "Succeecful";
                HttpContext.Session.SetString("Faculty", Model.UserName);
                return RedirectToAction("AttendancePanel");
            }
            else
            {
                ViewBag.S = "Username/Password is Incorrect.";
                return View();
            }
        }
        
        public IActionResult AttendanceLogin()
        {     
            if (HttpContext.Session.GetString("Faculty") != null &&
               HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;
                              
                var ClsName = db.ClassName.ToList();
                var CN = new SelectList(ClsName, "Id", "Classname");
                ViewBag.ClassName = CN;
                var time = db.ClassTime.ToList();
                var tm = new SelectList(time, "Id", "Time");
                ViewBag.Time = tm;
                var course = db.Course.ToList();
                var crse = new SelectList(course, "Id", "CourseName");
                ViewBag.CourseName = crse;

                return View();
            }
            return RedirectToAction("Login");
        }  

        [HttpPost]
        public IActionResult AttendanceList(AttendanceLoginViewModel attendanceLoginViewModel)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
               HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;
                
            ViewBag.ClassName=db.ClassName.Where(a => a.Id == attendanceLoginViewModel.classIdVM).FirstOrDefault();
            var date = attendanceLoginViewModel.dateVM.Date;
            ViewBag.Date = date.ToString("dd-MM-yyyy");
            ViewBag.CourseName = db.Course.Where(a => a.Id == attendanceLoginViewModel.courseIdVM).FirstOrDefault();
            ViewBag.ClassTime = db.ClassTime.Where(a => a.Id == attendanceLoginViewModel.timeIdVM).FirstOrDefault();

            var clsstulist = db.ClasssStudentList;
            var stu = db.Student;
            var querry = from stulist in clsstulist
                         where stulist.ClassId == attendanceLoginViewModel.classIdVM
                         join stdnt in stu on stulist.StudentId equals stdnt.Id
                         select new
                         {
                             stuId=stdnt.Id,
                             stuName=stdnt.FirstName +" "+ stdnt.LastName
                         };
            List<AttendanceStudentListVM> obj = new List<AttendanceStudentListVM>();
            foreach (var item in querry)
            {
                AttendanceStudentListVM ob = new AttendanceStudentListVM();
                ob.StudentIdVM = item.stuId;
                ob.StudentNameVM = item.stuName;
                ob.classIdVM = ViewBag.ClassName.Id;
                ob.courseIdVM = ViewBag.CourseName.Id;
                ob.timeIdVM = ViewBag.ClassTime.Id;
                ob.dateVM = date;                 
                obj.Add(ob);
            }
            
            return View(obj);
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult AttendanceTaken(List<AttendanceStudentListVM> attendanceStudentListVM)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
               HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

                for (int i = 0; i < attendanceStudentListVM.Count; i++)
                {                   
                    AttendanceList attendanceList = new AttendanceList();
                    attendanceList.ClassId = attendanceStudentListVM[i].classIdVM;
                    attendanceList.CourseId = attendanceStudentListVM[i].courseIdVM;
                    attendanceList.Date = attendanceStudentListVM[i].dateVM;
                    attendanceList.StudentId = attendanceStudentListVM[i].StudentIdVM;
                    attendanceList.TeacherId = facultyId.id;
                    attendanceList.ClassTimeId = attendanceStudentListVM[i].timeIdVM;
                    attendanceList.Status = attendanceStudentListVM[i].Status;

                    if(attendanceStudentListVM[i].Status==0)
                    {
                        MessageList messageList = new MessageList();
                        {
                            messageList.MessageTitle = "Absent alert.";
                            
                            messageList.TeacherId = facultyId.id;
                            messageList.Date = DateTime.Now.ToString("M/d/yyyy");
                            messageList.Time = DateTime.Now.ToString("hh:mm tt");
                            var courseId = db.Course.Where(a => a.Id == attendanceStudentListVM[i].courseIdVM).FirstOrDefault();
                            var sid = db.Student.Where(e => e.Id == attendanceList.StudentId).FirstOrDefault();
                            var sname = sid.FirstName + " " + sid.LastName;
                            messageList.MessageDetail = "Your son/daughter Mr/Mrs " + sname + " was absent in " + courseId.CourseName + " class at "+messageList.Time+" on "+messageList.Date+".";
                            messageList.SentBy = "tc";
                            var gId = db.Student.Where(a => a.Id == attendanceList.StudentId).FirstOrDefault();
                            messageList.GuardianId = gId.GuardianId;
                        };
                        db.MessageList.Add(messageList);
                        db.SaveChanges();
                    }
                    db.AttendanceList.Add(attendanceList);
                    db.SaveChanges();
                }
                return RedirectToAction("AttendancePanel");
            }
            return RedirectToAction("Login");
        }

        public IActionResult AttendancePanel()
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
               HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

                var j = db.ClassName.ToList();
                var l = db.ClassInfo.ToList();
                var k = db.Course.ToList();
                var m = from x in l
                        where x.TeacherId == facultyId.id
                        join a in j on x.ClassNameId equals a.Id
                        join b in k on x.CourseNameId equals b.Id
                        select new
                        {
                            cn = a.Classname,
                            cid=a.Id,
                            crn = b.CourseName,
                            crsid=b.Id
                        };
                List<FTcourses> obj = new List<FTcourses>();
                foreach (var item in m)
                {
                    FTcourses ob = new FTcourses();
                    ob.classname = item.cn;
                    ob.ClassNameId = item.cid;
                    ob.coursename = item.crn;
                    ob.CoursNameId = item.crsid;
                    obj.Add(ob);
                }
                return View(obj);
            }
            return RedirectToAction("Login");
        }
        public IActionResult StudentList(int classId, int courseId)
        {          
             if (HttpContext.Session.GetString("Faculty") != null &&
                 HttpContext.Session.GetString("Faculty") != "Expired")
              {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

                ViewBag.ClassName= db.ClassName.Where(a => a.Id ==classId ).FirstOrDefault();
                ViewBag.CourseName = db.Course.Where(a => a.Id == courseId).FirstOrDefault();

                var studentList = db.ClasssStudentList.ToList();
                var querry = from a in studentList
                             where a.ClassId == classId                      
                             select new
                             {
                                 stuid = a.StudentId                         
                             };
                List<EditAttendanceStudentListVM> obj = new List<EditAttendanceStudentListVM>();
                foreach(var item in querry)
                {
                    EditAttendanceStudentListVM ob = new EditAttendanceStudentListVM();
                    ob.StuIdVM = item.stuid;
                    var StuId= db.Student.Where(a => a.Id == item.stuid).FirstOrDefault();
                    ob.StuNameVM = StuId.FirstName + " " + StuId.LastName;
                    obj.Add(ob);
                }
                return View(obj);
            }
            return RedirectToAction("Login");
        }
        public IActionResult DateWiseAttendanceList(int Studentid, int Courseid, int Classid)
        {            
          if (HttpContext.Session.GetString("Faculty") != null &&
               HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

                var i = db.ClassName.ToList();
                var b = new SelectList(i, "Id", "Classname");
                ViewBag.c = i;
                ViewBag.ClassName = db.ClassName.Where(a => a.Id == Classid).FirstOrDefault();
                ViewBag.CourseName = db.Course.Where(a => a.Id == Courseid).FirstOrDefault();
                ViewBag.StudentName = db.Student.Where(a => a.Id == Studentid).FirstOrDefault();

                var attendancelist = db.AttendanceList.ToList();
                var querry = from a in attendancelist
                             where a.ClassId == Classid &&
                             a.CourseId == Courseid &&
                             a.StudentId == Studentid &&
                             a.TeacherId == facultyId.id
                             select new
                             {
                                 date=a.Date,
                                 time=a.ClassTimeId,
                                 status=a.Status
                             };
                List<DateWiseAttendanceListVM> obj = new List<DateWiseAttendanceListVM>();
                foreach (var item in querry)
                {
                    DateWiseAttendanceListVM ob = new DateWiseAttendanceListVM();
                    ob.DateVM = item.date;
                    var timeid = db.ClassTime.Where(a => a.Id == item.time).FirstOrDefault();
                    ob.TimeVM = timeid.Time;
                    if(item.status==0)
                    {
                        ob.StatusVM = "Absent";
                        ob.EditStatusVM = "Present";
                    }
                    else
                    {
                        ob.StatusVM = "Present";
                        ob.EditStatusVM = "Absent";
                    }
                    obj.Add(ob);
                }
                return View(obj);               
            }
            return RedirectToAction("Login");
        }
        public IActionResult EditAttendance(int stuid, DateTime date, int classid, int crseid,string time)
        {
            //var e = stuid;
            //var f = date;
            //var g = classid;
            //var h = crseid;
         if (HttpContext.Session.GetString("Faculty") != null &&
             HttpContext.Session.GetString("Faculty") != "Expired")
           {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            var b = new SelectList(i, "Id", "Classname");
            ViewBag.c = i;
            var timeid= db.ClassTime.Where(a => a.Time == time).FirstOrDefault();

            var attendancelist = db.AttendanceList.ToList();
                var querry = from a in attendancelist
                             where a.ClassId == classid &&
                             a.CourseId == crseid &&
                             a.StudentId == stuid &&
                             a.Date == date &&
                             a.ClassTimeId == timeid.Id &&
                             a.TeacherId == facultyId.id
                             select a;
                var x = querry.FirstOrDefault().Id;
                var c = db.AttendanceList.Where(a => a.Id == x).FirstOrDefault();
                c.Status = c.Status == 0 ? 1 : 0;
                db.AttendanceList.Update(c);
                db.SaveChanges();
                return RedirectToAction("DateWiseAttendanceList",new { Studentid= c.StudentId, Courseid =c.CourseId, Classid =c.ClassId});
            }
            return RedirectToAction("Login");
        }
        public IActionResult AttendancePercentage(int classId, int courseId)
        {
         if (HttpContext.Session.GetString("Faculty") != null &&
             HttpContext.Session.GetString("Faculty") != "Expired")
           {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            var b = new SelectList(i, "Id", "Classname");
            ViewBag.c = i;
               
            ViewBag.ClassName = db.ClassName.Where(a => a.Id == classId).FirstOrDefault().Classname;
            ViewBag.CourseName = db.Course.Where(a => a.Id == courseId).FirstOrDefault().CourseName;

                List<AttendancePersantageVM> obj = new List<AttendancePersantageVM>();
                var attendancelist = db.AttendanceList.ToList();
                var stu = db.Student.ToList();
                var q = db.ClasssStudentList.ToList();
                var e = from t in q
                        where t.ClassId == classId
                        join c in stu on t.StudentId equals c.Id
                        select t;
               
                foreach (var item in e)
                {
                    AttendancePersantageVM ob = new AttendancePersantageVM();                  
                    ob.StuidVM = item.StudentId;
                    var sname = db.Student.Where(u => u.Id == item.StudentId).FirstOrDefault();
                    ob.StuNameVM = sname.FirstName + " " + sname.LastName;
                    var querry = from a in attendancelist
                                 where a.ClassId == classId &&
                                 a.CourseId == courseId 
                                 where a.StudentId == item.StudentId
                                 select a;
                    var x = 0;
                    var y = 0;
                    var z = 0;
                    foreach (var Item in querry)
                    {                        
                        ob.TotallClassVM = x+1;
                        x++;
                        if (Item.Status == 1)
                        {
                            ob.PresentVM = y + 1;
                            y++;
                        }
                        if (Item.Status == 0)
                        {
                            ob.AbsentVM = z + 1;
                            z++;
                        }                                              
                    }
                    var ap=(double) (ob.AbsentVM * 100) / ob.TotallClassVM;
                    ob.PersantageVM = Math.Round(ap,2);
                    obj.Add(ob);
                }
                return View(obj);
            }
            return RedirectToAction("Login");
        }
    }
}