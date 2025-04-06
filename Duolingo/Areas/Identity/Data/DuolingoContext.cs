using Duolingo.Areas.Identity.Data;
using Duolingo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Duolingo.Data;

public class DuolingoContext : IdentityDbContext<DuolingoUser>
{
    public DuolingoContext(DbContextOptions<DuolingoContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }



    public DbSet<ExperienceReward> ExperienceReward { get; set; }
    public DbSet<SubjectToLearn> Subjects { get; set; }
    public DbSet<ContentToLearn> Contents { get; set; }
    public DbSet<QuizQuestion> Questions { get; set; }
    public DbSet<PracticalTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<DuolingoUser>()
     .HasMany(u => u.Courses)
     .WithOne(c => c.CourseUsers)
     .HasForeignKey(fk => fk.UserId)
     ;  // Change to Restrict or NoAction to prevent cascade delete


        builder.Entity<ContentToLearn>()
           .HasOne(c => c.Subject)
           .WithMany(s => s.ContentToLearn)
           .HasForeignKey(c => c.SubjectId);

        builder.Entity<QuizQuestion>()
            .HasOne(q => q.Subject)
            .WithMany(s => s.Question)
            .HasForeignKey(q => q.SubjectId);

        builder.Entity<PracticalTask>()
            .HasOne(p => p.Subject)
            .WithMany(s => s.PracticalTask)
            .HasForeignKey(p => p.SubjectId);



        builder.Entity<SubjectToLearn>()
            .HasOne(p => p.Course)
            .WithMany(s => s.subjectToLearns)
            .HasForeignKey(k => k.CourseId);




        builder.Entity<ExperienceReward>().HasData(
            new ExperienceReward {Id=1, DoneTasks = 0, RewardName = "Amator" },
            new ExperienceReward { Id = 2, DoneTasks = 1, RewardName = "Początki są najtrudniejsze" },
            new ExperienceReward { Id = 3, DoneTasks = 10, RewardName = "Pierwsze podchody zrobione" },
            new ExperienceReward { Id = 4, DoneTasks = 50, RewardName = "Doświadczony zawodnik" },
            new ExperienceReward { Id = 5, DoneTasks = 100, RewardName = "Ekspert języków obcych" }

            );

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
