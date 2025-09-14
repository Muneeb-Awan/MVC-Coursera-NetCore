using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCoursera.Models;

namespace MyCoursera.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly MasterContext _context;

        public AdminController(MasterContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var stats = new
            {
                TotalCourses = _context.courses.Count(),
                PendingCourses = _context.courses.Count(c => c.status == CourseState.Pending),
                ApprovedCourses = _context.courses.Count(c => c.status == CourseState.Approved),
                TotalStudents = _context.Users.Count(u => _context.UserRoles.Any(r => r.UserId == u.Id && r.RoleId == "3f664ade-11cc-4559-9aee-118e39228112")),
                TotalInstructors = _context.Users.Count(u => _context.UserRoles.Any(r => r.UserId == u.Id && r.RoleId == "0bf99408-0d64-40d7-b51f-0899d70b1717"))
            };

            return View(stats);
        }
        // ✅ List all pending courses
        public IActionResult PendingCourses()
        {
            var pendingCourses = _context.courses
                                         .Where(c => c.status == CourseState.Pending)
                                         .ToList();

            return View(pendingCourses);
        }

        // ✅ Approve a course
        [HttpPost]
        public IActionResult ApproveCourse(int id)
        {
            var course = _context.courses.Find(id);
            if (course != null)
            {
                course.status = CourseState.Approved;
                _context.SaveChanges();
            }
            return RedirectToAction("PendingCourses");
        }

        // ✅ Reject a course
        [HttpPost]
        public IActionResult RejectCourse(int id)
        {
            var course = _context.courses.Find(id);
            if (course != null)
            {
                course.status = CourseState.Rejected;
                _context.SaveChanges();
            }
            return RedirectToAction("PendingCourses");
        }
        public IActionResult AllCourses()
        {
            var courses = _context.courses.ToList();
            return View(courses);
        }

        [HttpPost]
        public IActionResult ChangeCourseState(int id, CourseState newState)
        {
            var course = _context.courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
            {
                course.status = newState;
                _context.SaveChanges();
            }

            return RedirectToAction("AllCourses");
        }
    }
}
