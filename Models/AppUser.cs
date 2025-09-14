
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyCoursera.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public ICollection<Course>? EnrolledCourses { get; set; }
    }
}
