namespace MyCoursera.Models
{
    public class StudentDashboardViewModel
    {
        public int EnrolledCoursesCount { get; set; }
        public double ProgressPercent { get; set; } // later you can calculate based on course completion logic
        public int CertificatesCount { get; set; } // if you implement certificates
        public List<Course> RecommendedCourses { get; set; } = new();
    }
}
