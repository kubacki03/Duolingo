

using Duolingo.Areas.Identity.Data;
using Duolingo.Data;
using Duolingo.Models;
using Duolingo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Duolingo.Controllers
{
    public class CourseController : Controller
    {
       
        public readonly SignInManager<DuolingoUser> _signInManager;
        private readonly UserManager<DuolingoUser> _userManager;
        private readonly DuolingoContext _context;
        private AiService _aiService;
        public CourseController(AiService ai,  SignInManager<DuolingoUser> signInManager, UserManager<DuolingoUser> userManager, DuolingoContext context)
        {
            
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _aiService = ai;
        }


        [HttpGet]
        public async Task<ActionResult> JoinToCourse(string level, string language)
        {
            var user = await _userManager.GetUserAsync(User);
            
            var existCourse = _context.Courses
                   .Include(s => s.CourseUsers)
                   .Include(x => x.subjectToLearns)
                       .ThenInclude(s => s.Question)
                   .Include(x => x.subjectToLearns)  
                       .ThenInclude(s => s.PracticalTask)  
                   .FirstOrDefault(o => o.Language == language && o.Level == level && o.UserId == user.Id);


            if (existCourse != null)
            {
                int done = 0;
                int all = 0;
                List<SubjectModel> oldSubject = new List<SubjectModel>();
                foreach (var subject in existCourse.subjectToLearns)
                {
                   
                   
                    var allTasks= subject.PracticalTask.Count();
                    var allQuestions = subject.Question.Count();
                    var doneTasks= subject.PracticalTask.Where(s => s.isPassed == true).Count();
                    var doneQuestions= subject.Question.Where(s => s.isPassed == true).Count();


                    all += allQuestions + allTasks;
                    done += doneQuestions + doneTasks;

                    oldSubject.Add(new SubjectModel { description = subject.Description, subject = subject.Subject, CourseId = existCourse.Id, PassedChalenges=doneQuestions+doneTasks, ChallengeToDo=allQuestions+allTasks });


                }
                TempData["courseName"] = existCourse.Name;
                TempData["progres"] = done + "/" + all;
                return View(oldSubject);
            }




            Course course = new Course();
            course.Name = language + " " + level;
            course.UserId = user.Id;
            course.CourseUsers = user;
            course.Level = level;
            course.Language = language;


            _context.Courses.Add(course);

            _context.SaveChanges();

            List<SubjectModel> subjects = await _aiService.GenerateSubjects((LevelEnum)System.Enum.Parse(typeof(LevelEnum), level), (LanguagesEnum)System.Enum.Parse(typeof(LanguagesEnum), language));

            var existCours = _context.Courses.Include(m => m.subjectToLearns).FirstOrDefault(z => z.Name == course.Name && z.UserId == user.Id);

            foreach (var subject in subjects)
            {
                SubjectToLearn s = new SubjectToLearn { Description = subject.description, Subject = subject.subject, Course = existCours, CourseId = existCours.Id };
                existCours.subjectToLearns.Add(s);
            }

            subjects.ForEach(n => n.CourseId = existCours.Id);

            _context.SaveChanges();

            TempData["courseName"] =course.Name;
            return View(subjects);



        }

        public async Task<IActionResult> GetSubjectContent(string subject, string language, int id)
        {
            var course = _context.Courses
 .Include(s => s.subjectToLearns)
     .ThenInclude(x => x.ContentToLearn)
 .Include(s => s.subjectToLearns)
     .ThenInclude(x => x.Question)
 .Include(s => s.subjectToLearns)
     .ThenInclude(x => x.PracticalTask)
 .FirstOrDefault(z => z.Id == id);

            //pobiera dobry subject
            var currentSubject = course.subjectToLearns.FirstOrDefault(z => z.Subject == subject && z.CourseId == id);

            var level = course.Level;

            //nie pobiera konentu wraz z subject
            var opt = currentSubject.ContentToLearn.IsNullOrEmpty();

            if (!opt)
            {
                var contMod = new ContentModel { ContentToLearn = (List<ContentToLearn>)currentSubject.ContentToLearn, PracticalTasks = (List<PracticalTask>)currentSubject.PracticalTask, Quiz = (List<QuizQuestion>)currentSubject.Question };
                return View(contMod);
            }

            var user = await _userManager.GetUserAsync(User);
            var contentModel = await _aiService.GenerateContent(subject, language, level);


            var rand = new Random();

            var temp = rand.Next(1000);

            currentSubject.TempId = temp;

            _context.SaveChanges();
            var s = _context.Subjects.Include(v => v.ContentToLearn).Include(m => m.PracticalTask).Include(i => i.Question).FirstOrDefault(x => x.TempId == temp);

            s.ContentToLearn = contentModel.Value.ContentToLearn
                .Select(d =>
                {
                    d.Subject = s;
                    d.SubjectId = s.Id;
                    return d;
                })
                .ToList();


            s.PracticalTask = contentModel.Value.PracticalTasks
               .Select(d =>
               {
                   d.Subject = s;
                   d.SubjectId = s.Id;
                   return d;
               })
               .ToList();

            s.Question = contentModel.Value.Quiz
               .Select(d =>
               {
                   d.Subject = s;
                   d.SubjectId = s.Id;
                   return d;
               })
               .ToList();

            // var newSubject = new SubjectToLearn { ContentToLearn = contentModel.Value.ContentToLearn, PracticalTask = contentModel.Value.PracticalTasks, Question = contentModel.Value.Quiz, Subject = subject, Description = "nuak" };
            //course.subjectToLearns.Add(newSubject);
            _context.SaveChanges();
            return View(contentModel.Value);
        }




    }
}