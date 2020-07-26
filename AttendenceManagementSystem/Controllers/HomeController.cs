using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendenceManagementSystem.Models;
using AttendenceManagementSystem.Database;

namespace AttendenceManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public readonly dataContext s;
        public HomeController(dataContext a)
        {
            s = a;
        }
        public IActionResult Index()
        {
            var i = s.ClassName.ToList();
            return View(i);
        }
    }
}
