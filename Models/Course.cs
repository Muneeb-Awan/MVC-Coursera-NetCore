using System.ComponentModel.DataAnnotations;

namespace MyCoursera.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public CourseState status { get; set; } = CourseState.Pending;
        public ICollection<AppUser>? EnrolledStudents;
    }
    public enum CourseState
    {
        Pending,
        Approved,
        Rejected
    }
}
