using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCoursera.Models;

namespace MyCoursera.Controllers
{
    [Authorize(Roles ="Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MasterContext _context; // <-- Your EF DbContext

        public StudentController(UserManager<AppUser> userManager, MasterContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Get enrolled courses
            var enrolledCourses = await _context.courses
                .Include(c => c.EnrolledStudents)
                .Where(c => c.EnrolledStudents.Any(s => s.Id == currentUser.Id))
                .ToListAsync();

            // TODO: Replace with actual logic once you add progress tracking
            double progressPercent = enrolledCourses.Count > 0 ? 60 : 0;

            // TODO: Replace with actual certificate logic
            int certificatesCount = 0;

            // Recommended = courses student is NOT enrolled in
            var recommendedCourses = await _context.courses
                .Include(c => c.EnrolledStudents)
                .Where(c => c.status == CourseState.Approved && !c.EnrolledStudents.Any(s => s.Id == currentUser.Id))
                .Take(3)
                .ToListAsync();

            var vm = new StudentDashboardViewModel
            {
                EnrolledCoursesCount = enrolledCourses.Count,
                ProgressPercent = progressPercent,
                CertificatesCount = certificatesCount,
                RecommendedCourses = recommendedCourses
            };

            return View(vm);
        }

        public async Task<IActionResult> Explore()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var courses = await _context.courses
                .Include(c => c.EnrolledStudents)
                .Where(c => c.status == CourseState.Approved && !c.EnrolledStudents.Any(s => s.Id == currentUser.Id))
                .ToListAsync();

            return View(courses);
        }
        [HttpPost]
        public async Task<IActionResult> Enroll(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var course = await _context.courses
                .Include(c => c.EnrolledStudents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            if (!course.EnrolledStudents.Any(s => s.Id == currentUser.Id))
            {
                course.EnrolledStudents.Add(currentUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Explore");
        }
        public async Task<IActionResult> MyCourses()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var enrolledCourses = await _context.courses
                .Include(c => c.EnrolledStudents)
                .Where(c => c.status == CourseState.Approved && c.EnrolledStudents.Any(s => s.Id == currentUser.Id))
                .ToListAsync();

            return View(enrolledCourses);
        }
        [HttpPost]
        public async Task<IActionResult> LeaveCourse(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var course = await _context.courses
                .Include(c => c.EnrolledStudents)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            if (course.EnrolledStudents.Contains(user))
            {
                course.EnrolledStudents.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyCourses");
        }
    }
}
