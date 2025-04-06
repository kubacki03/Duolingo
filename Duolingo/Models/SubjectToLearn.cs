using Duolingo.Areas.Identity.Data;

namespace Duolingo.Models
{
    public class SubjectToLearn
    {
        public int Id { get; set; }

        public int TempId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }


        public int CourseId { get; set; }
        public Course Course { get; set; }

        public ICollection<ContentToLearn> ContentToLearn { get; set; } = new List<ContentToLearn>();
        public ICollection<QuizQuestion> Question { get; set; } = new List<QuizQuestion>();
        public ICollection<PracticalTask> PracticalTask { get; set; } = new List<PracticalTask>();
    }

}