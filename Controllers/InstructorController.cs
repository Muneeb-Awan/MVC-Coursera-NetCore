using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCoursera.Models;

namespace MyCoursera.Controllers
{
    [Authorize(Roles ="Instructor")]
    public class InstructorController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MasterContext _context; // <-- Your EF DbContext

        public InstructorController(UserManager<AppUser> userManager, MasterContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var courses = _context.courses
            .Include(c => c.EnrolledStudents)
            .Where(c => c.AuthorId == currentUser.Id)
            .ToList();

            int totalCourses = courses.Count;
            int totalStudents = courses.Sum(c => c.EnrolledStudents?.Count ?? 0);

            string rank;
            int progress;
            if (totalStudents < 10)
            {
                rank = "Beginner Instructor";
                progress = (int)((totalStudents / 10.0) * 100);
            }
            else if (totalStudents < 50)
            {
                rank = "Intermediate Instructor";
                progress = (int)((totalStudents / 50.0) * 100);
            }
            else if (totalStudents < 200)
            {
                rank = "Pro Instructor";
                progress = (int)((totalStudents / 200.0) * 100);
            }
            else
            {
                rank = "Master Instructor";
                progress = 100;
            }

            var vm = new InstructorDashboardViewModel
            {
                TotalCourses = totalCourses,
                TotalStudents = totalStudents,
                Rank = rank,
                ProgressPercent = progress
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult NewCourse()
        {
            return View(new Course());
        }
        public async Task<IActionResult> MyCourses()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            List<Course> courses = _context.courses.Where(x=> x.AuthorId==currentUser.Id).Include(c => c.EnrolledStudents).ToList();
            return View(courses);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                course.status = CourseState.Pending;

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    course.Author = currentUser.FullName ?? "Unknown";
                    course.AuthorId = currentUser.Id;
                }

                _context.courses.Add(course);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyCourses");
            }

            return View("NewCourse", course);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _context.courses.FirstOrDefaultAsync(x => x.Id == id);
            return View(contact);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Author,AuthorId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyCourses");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.courses.FirstOrDefaultAsync(x => x.Id == id);
            return View(contact);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var contact = await _context.courses.FindAsync(id);
            if (contact != null)
            {
                _context.courses.Remove(contact);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public IActionResult ListStudents(int id)
        {
            var course = _context.courses
            .Include(c => c.EnrolledStudents)   // load navigation
            .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var students = course.EnrolledStudents ?? new List<AppUser>();
            return View((course,students));
        }
    }
}
