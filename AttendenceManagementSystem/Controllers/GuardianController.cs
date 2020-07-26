using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendenceManagementSystem.Database;
using AttendenceManagementSystem.Models;
using AttendenceManagementSystem.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttendenceManagementSystem.Controllers
{
    public class GuardianController : Controller
    {
        public readonly dataContext db;
        public GuardianController(dataContext a)
        {
            db = a;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Guardian Model)
        {
            var i = db.Guardian.Where(s => s.UserName == Model.UserName && s.Password == Model.Password).FirstOrDefault();
            if (i != null)
            {
                ViewBag.S = "Succeecful";
                HttpContext.Session.SetString("Guardian", Model.UserName);             
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.S = "Username/Password is Incorrect.";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("Guardian", "Expired");
            return RedirectToAction("Login");
        }
        public IActionResult Dashboard()
        {
            //ViewBag.GuardianName= HttpContext.Session.GetString("Guardian");
            if (HttpContext.Session.GetString("Guardian") != null &&
                HttpContext.Session.GetString("Guardian") != "Expired")
            {
                var guardian = HttpContext.Session.GetString("Guardian");
                var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
                ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

                ViewBag.GdnInfo = db.Guardian.Where(a => a.id == guardianId.id).FirstOrDefault();
                ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();

                var clsStuList = db.ClasssStudentList.ToList();
                var stu = db.Student.ToList();
                var clsName = db.ClassName.ToList();
                var querry = from st in stu
                             where st.GuardianId == guardianId.id
                             join csl in clsStuList on st.Id equals csl.StudentId
                             join cn in clsName on csl.ClassId equals cn.Id
                             select new
                             {
                                 stuname = st.FirstName + " " + st.LastName,
                                 clsname = cn.Classname,
                                 stuId = st.Id
                             };

                List<GdStuListVM> ob = new List<GdStuListVM>();
                foreach (var item in querry)
                {
                    GdStuListVM obj = new GdStuListVM();
                    obj.StudentName = item.stuname;
                    obj.ClassName = item.clsname;
                    obj.StuId = item.stuId;
                    ob.Add(obj);
                }

                return View(ob);
            }
            return RedirectToAction("Login");
        }
        public IActionResult StudentInfo(int StuId)
        {
            if (HttpContext.Session.GetString("Guardian") != null &&
                HttpContext.Session.GetString("Guardian") != "Expired")
            {
            var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.name = db.Student.Where(a => a.Id == StuId).FirstOrDefault();
            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();
            var clsStuList = db.ClasssStudentList.ToList();
            var ClsName = db.ClassName.ToList();
            var FindClassName = from ClsStuList in clsStuList
                                where ClsStuList.StudentId == StuId
                                join clsname in ClsName on ClsStuList.ClassId equals clsname.Id
                                select clsname;
            var classId = FindClassName.FirstOrDefault().Id;
            ViewBag.CLassName = db.ClassName.Where(a => a.Id == classId).FirstOrDefault();

            var clsInfo = db.ClassInfo.ToList();
            var TName = db.Faculty.ToList();
            var Cname = db.Course.ToList();
            var TeacherCourses = (from ClsInfo in clsInfo
                                 where ClsInfo.ClassNameId == 1
                                 //join ClsInfo in clsInfo on ClassStuList.ClassId equals ClsInfo.ClassNameId
                                 join tname in TName on ClsInfo.TeacherId equals tname.id
                                 join cname in Cname on ClsInfo.CourseNameId equals cname.Id
                                 select new
                                 {
                                     CrsName = cname.CourseName,
                                     TechName = tname.FirstName + " " + tname.LastName,
                                     TechId=tname.id
                                 }).ToList();
            List<TeacherCourseVM> obj = new List<TeacherCourseVM>();
            foreach (var item in TeacherCourses)
            {
                TeacherCourseVM ob = new TeacherCourseVM();
                ob.CourseNameVM = item.CrsName;
                ob.TeacherNameVM = item.TechName;
                ob.TeacherIdVM = item.TechId;
                obj.Add(ob);
            }            
            
            return View(obj);
            }
            return RedirectToAction("Login");
        }       
        public IActionResult SentMessage(int TechId)
        {
            if (HttpContext.Session.GetString("Guardian") != null &&
            HttpContext.Session.GetString("Guardian") != "Expired")
            {
            var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();
            //ViewBag.TecherName= db.Faculty.Where(a => a.id == TechId).FirstOrDefault();
            ViewBag.TeacherId = TechId;
            return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult SentMessage(int TeachId, MessageList m)
        {
            if (HttpContext.Session.GetString("Guardian") != null &&
                HttpContext.Session.GetString("Guardian") != "Expired")
            {
            var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.FacultyName = db.Faculty.Where(b => b.id == TeachId).FirstOrDefault();
            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();
                
            var i = TeachId;
            if (m.MessageTitle != null && m.MessageDetail != null)
            {
                MessageList msg = new MessageList()
                {
                    MessageTitle = m.MessageTitle,
                    MessageDetail = m.MessageDetail,
                    TeacherId = m.TeacherId = TeachId,
                    GuardianId = m.GuardianId = guardianId.id,
                    Date = m.Date = DateTime.Now.ToString("M/d/yyyy"),
                    Time = m.Time = DateTime.Now.ToString("hh:mm tt"),
                    SentBy = m.SentBy = "gd",

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
            if (HttpContext.Session.GetString("Guardian") != null &&
            HttpContext.Session.GetString("Guardian") != "Expired")
            {
                    var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();

            var msglist = db.MessageList.ToList();
            var querry = from msg in msglist
                         where msg.GuardianId == guardianId.id & msg.SentBy == "gd"
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
        public IActionResult ReadMessage(int MessageId)
        {
            if (HttpContext.Session.GetString("Guardian") != null &&
            HttpContext.Session.GetString("Guardian") != "Expired")
            {
            var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();
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
            public IActionResult Inbox()
        {
            if (HttpContext.Session.GetString("Guardian") != null &&
            HttpContext.Session.GetString("Guardian") != "Expired")
           {
            var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();
            var msglist = db.MessageList.ToList();
            var querry = from msg in msglist
                         where msg.GuardianId == guardianId.id & msg.SentBy == "tc"
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
            if (HttpContext.Session.GetString("Guardian") != null &&
            HttpContext.Session.GetString("Guardian") != "Expired")
           {
            var guardian = HttpContext.Session.GetString("Guardian");
            var guardianId = db.Guardian.Where(a => a.UserName == guardian).FirstOrDefault();
            ViewBag.GuardianName = guardianId.FirstName + " " + guardianId.LastName;

            ViewBag.StuName = db.Student.Where(b => b.GuardianId == guardianId.id).ToList();
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
