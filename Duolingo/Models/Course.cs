using Duolingo.Areas.Identity.Data;

namespace Duolingo.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }

        public string UserId { get; set; }
        public DuolingoUser CourseUsers { get; set; }

        public ICollection<SubjectToLearn> subjectToLearns { get; set; } = new List<SubjectToLearn>();
    }

}