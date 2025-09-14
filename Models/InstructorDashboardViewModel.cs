namespace MyCoursera.Models
{
    public class InstructorDashboardViewModel
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public string Rank { get; set; } = "Beginner";
        public int ProgressPercent { get; set; } // For progress bar
    }
}
