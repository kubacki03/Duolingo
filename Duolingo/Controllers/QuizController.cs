using Duolingo.Areas.Identity.Data;
using Duolingo.Data;
using Duolingo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Duolingo.Controllers
{
    public class QuizController : Controller
    {
        public readonly SignInManager<DuolingoUser> _signInManager;
        private readonly UserManager<DuolingoUser> _userManager;
        private readonly DuolingoContext _context;
        private AiService _aiService;
        public QuizController(AiService ai, SignInManager<DuolingoUser> signInManager, UserManager<DuolingoUser> userManager, DuolingoContext context)
        {
           
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _aiService = ai;
        }

        [HttpPost]
        public async Task<IActionResult> CheckQuiz([FromBody] QuizRequestModel request)
        {

            var quiz = _context.Questions.FirstOrDefault(p => p.Id == request.QuestionId);
            if (quiz == null)
            {
                return Json(new { isCorrect = false, review = "Nie znaleziono zadania." });
            }


            ResponseModel response = await _aiService.CheckTaskCorrectnessAsync(quiz.Questions, request.Answer);

            quiz.isPassed = response.IsCorrect;
            if (quiz.isPassed)
            {
                var user = await _userManager.GetUserAsync(User);
                user.DoneTasks++;
            }

            _context.Update(quiz);
            await _context.SaveChangesAsync();


            return Json(new { success = response.IsCorrect });
        }


      

        public class QuizRequestModel
        {
            public int QuestionId { get; set; }
            public string Answer { get; set; }
        }
    }
}
public class QuizRequestModel
{
    public int QuestionId { get; set; }
    public string Answer { get; set; }
}
