using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace MyCoursera.Models
{
    public class MasterContext:IdentityDbContext
    {
        public MasterContext(DbContextOptions<MasterContext> options) : base(options)
        {
            
        }

        public DbSet<AppUser> users { get; set; }
        public DbSet<Course> courses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Course>().
                HasMany(x => x.EnrolledStudents).
                WithMany(y => y.EnrolledCourses).
                UsingEntity(j => j.ToTable("EnrolmentTable"));
        }
    }
}
