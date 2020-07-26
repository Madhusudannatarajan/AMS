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
    public class FacultyController : Controller
    {
        public readonly dataContext db;
        public FacultyController(dataContext a)
        {
            db = a;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("Faculty", "Expired");
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult Login(Faculty Model)
        {
            var i = db.Faculty.Where(s => s.UserName == Model.UserName && s.Password == Model.Password).FirstOrDefault();
            if (i != null)
            {
                ViewBag.S = "Succeecful";
                HttpContext.Session.SetString("Faculty", Model.UserName);
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.S = "Username/Password is Incorrect.";
                return View();
            }
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
                HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
                var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
                ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

                var i = db.ClassName.ToList();
                ViewBag.c = i;
                ViewBag.j = db.Faculty.Where(a => a.id == facultyId.id).FirstOrDefault();
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
                            crn = b.CourseName
                        };
                List<FTcourses> obj = new List<FTcourses>();
                foreach (var item in m)
                {
                    FTcourses ob = new FTcourses();
                    ob.classname = item.cn;
                    ob.coursename = item.crn;
                    obj.Add(ob);
                }
                return View(obj);
            }
            return RedirectToAction("Login");
        }
       
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ClassAndCourse()
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
                HttpContext.Session.GetString("Faculty") != "Expired")
            {
                var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            //var b = new SelectList(i, "Id", "Classname");
            ViewBag.c = i;
            return View();
            }
            return RedirectToAction("Login");
        }
      
        public IActionResult StudentInfo(int Id)
        {
                if (HttpContext.Session.GetString("Faculty") != null &&
                HttpContext.Session.GetString("Faculty") != "Expired")
                {
                    var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            ViewBag.stuInfo = db.Student.Where(a => a.Id == Id).FirstOrDefault();
            var stu = db.Student.ToList();
            var grdn = db.Guardian.ToList();
            var querry = from st in stu
                         where st.Id == Id
                         join gd in grdn on st.GuardianId equals gd.id
                         select new
                         {
                             Gid = gd.id,
                             GName = gd.FirstName + " " + gd.LastName,
                             Gphn = gd.Phone,
                             Gmail=gd.Email
                         };
            List<GuardianInfoVM> ob = new List<GuardianInfoVM>();
            foreach (var item in querry)
            {
                GuardianInfoVM obj = new GuardianInfoVM();
                obj.GidVM = item.Gid;
                obj.GNameVM = item.GName;
                obj.GPhoneVM = item.Gphn;
                obj.GMailVM = item.Gmail;
                ob.Add(obj);
            }
            return View(ob);
            }
            return RedirectToAction("Login");
        }
        public IActionResult StudentList(int Id)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
            HttpContext.Session.GetString("Faculty") != "Expired")
            {
                        var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            ViewBag.className = db.ClassName.Where(a => a.Id == Id).FirstOrDefault();
            var stu = db.Student.ToList();
            var ClsStuList = db.ClasssStudentList.ToList();
            var querry = from a in ClsStuList
                         where a.ClassId == Id
                         join b in stu on a.StudentId equals b.Id

                         select new
                         {
                             stuId = b.Id,
                             stuFirstname = b.FirstName,
                             stuLastname = b.LastName
                         };
            List<StudentVM> ob = new List<StudentVM>();
            foreach (var item in querry)
            {
                StudentVM obj = new StudentVM();
                obj.Sid = item.stuId;
                obj.SFirstName = item.stuFirstname;
                obj.SLastName = item.stuLastname;
                ob.Add(obj);
            }
            return View(ob);
            }
            return RedirectToAction("Login");
        }
        public IActionResult SentMessage(int SId)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
            HttpContext.Session.GetString("Faculty") != "Expired")
           {
                            var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            var sid = SId;
            var stu = db.Student.ToList();
            var grdn = db.Guardian.ToList();
            var querry = from st in stu
                         where st.Id == sid
                         join gd in grdn on st.GuardianId equals gd.id
                         select new
                         {
                             Gname = gd.FirstName + " " + gd.LastName,
                             Gid=gd.id
                         };
           // ViewBag.GName= querry;
            foreach (var item in querry)
            {
                ViewBag.GName = item.Gname;
                ViewBag.GId = item.Gid;
            }           
            return View();
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult SentMessage(int GdnId, MessageList m)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
                HttpContext.Session.GetString("Faculty") != "Expired")
            {
            var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            var g = GdnId;
            if (m.MessageTitle != null && m.MessageDetail != null)
            {
                MessageList msg = new MessageList()
                {
                    MessageTitle = m.MessageTitle,
                    MessageDetail = m.MessageDetail,
                    TeacherId = m.TeacherId = facultyId.id,
                    GuardianId = m.GuardianId = GdnId,
                    Date = m.Date = DateTime.Now.ToString("M/d/yyyy"),
                    Time = m.Time = DateTime.Now.ToString("hh:mm tt"),
                    SentBy = m.SentBy = "tc",

                };
                db.MessageList.Add(msg);
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("ShowSentMessage");
            }
            else
            {
                ViewBag.ErrorMessage = "Fill up all the field";
                return View();
            }
            }
            return RedirectToAction("Login");
        }

        public IActionResult ShowSentMessage()
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
            HttpContext.Session.GetString("Faculty") != "Expired")
           {
            var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            var msglist = db.MessageList.ToList();
            var querry = from msg in msglist
                         where msg.TeacherId == facultyId.id & msg.SentBy == "tc"
                         select new
                         {
                             id=msg.Id,
                             title=msg.MessageTitle,
                             date=msg.Date,
                             time=msg.Time
                         };
            //MessageList m = new MessageList();
            List<MessageVM> m = new List<MessageVM>();
            foreach (var item in querry)
            {
                MessageVM ml = new MessageVM();
                ml.MId = item.id;
                ml.MTitle = item.title;
                ml.MDate = item.date;
                ml.MTime = item.time;
                m.Add(ml);
            }
            return View(m);
            }
            return RedirectToAction("Login");
        }
        public IActionResult ReadMessage(int MessageId)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
            HttpContext.Session.GetString("Faculty") != "Expired")
            {
            var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            var msglist = db.MessageList.ToList();
            var grdn = db.Guardian.ToList();
            var fclty = db.Faculty.ToList();
            var querry = from msg in msglist
                         where msg.Id == MessageId
                         join gdn in grdn on msg.GuardianId equals gdn.id
                         join flty in fclty on msg.TeacherId equals flty.id
                         select new
                         {
                           tcname=flty.FirstName + " " + flty.LastName,
                           grdname=gdn.FirstName + " " + gdn.LastName,
                           msgdate=msg.Date,
                           msgtime=msg.Time,
                           msgtitle=msg.MessageTitle,
                           msgdetail=msg.MessageDetail
                         };
            foreach (var item in querry)
            {
                ViewBag.facultyname = item.tcname;
                ViewBag.grdname = item.grdname;
                ViewBag.msgdate = item.msgdate;
                ViewBag.msgtime = item.msgtime;
                ViewBag.msgtitle = item.msgtitle;
                ViewBag.msgdetail = item.msgdetail;
            }
            return View();
            }
            return RedirectToAction("Login");
        }
        public IActionResult Inbox()
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
            HttpContext.Session.GetString("Faculty") != "Expired")
            {
            var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            var msglist = db.MessageList.ToList();
            var querry = from msg in msglist
                         where msg.TeacherId == facultyId.id & msg.SentBy == "gd"
                         select new
                         {
                             id = msg.Id,
                             title = msg.MessageTitle,
                             date = msg.Date,
                             time = msg.Time
                         };
            //MessageList m = new MessageList();
            List<MessageVM> m = new List<MessageVM>();
            foreach (var item in querry)
            {
                MessageVM ml = new MessageVM();
                ml.MId = item.id;
                ml.MTitle = item.title;
                ml.MDate = item.date;
                ml.MTime = item.time;
                m.Add(ml);
            }
            return View(m);
            }
            return RedirectToAction("Login");
        }
        public IActionResult InboxReadMessage(int MessageId)
        {
            if (HttpContext.Session.GetString("Faculty") != null &&
            HttpContext.Session.GetString("Faculty") != "Expired")
           {
            var faculty = HttpContext.Session.GetString("Faculty");
            var facultyId = db.Faculty.Where(a => a.UserName == faculty).FirstOrDefault();
            ViewBag.FacultyName = facultyId.FirstName + " " + facultyId.LastName;

            var i = db.ClassName.ToList();
            ViewBag.c = i;
            var msglist = db.MessageList.ToList();
            var grdn = db.Guardian.ToList();
            var fclty = db.Faculty.ToList();
            var querry = from msg in msglist
                         where msg.Id == MessageId
                         join gdn in grdn on msg.GuardianId equals gdn.id
                         join flty in fclty on msg.TeacherId equals flty.id
                         select new
                         {
                             tcname = flty.FirstName + " " + flty.LastName,
                             grdname = gdn.FirstName + " " + gdn.LastName,
                             msgdate = msg.Date,
                             msgtime = msg.Time,
                             msgtitle = msg.MessageTitle,
                             msgdetail = msg.MessageDetail
                         };
            foreach (var item in querry)
            {
                ViewBag.facultyname = item.tcname;
                ViewBag.grdname = item.grdname;
                ViewBag.msgdate = item.msgdate;
                ViewBag.msgtime = item.msgtime;
                ViewBag.msgtitle = item.msgtitle;
                ViewBag.msgdetail = item.msgdetail;
            }
            return View();
            }
            return RedirectToAction("Login");
        }
    }
}
