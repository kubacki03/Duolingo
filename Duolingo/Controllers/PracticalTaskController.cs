using Duolingo.Areas.Identity.Data;
using Duolingo.Data;
using Duolingo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duolingo.Controllers
{
    public class PracticalTaskController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly SignInManager<DuolingoUser> _signInManager;
        private readonly UserManager<DuolingoUser> _userManager;
        private readonly DuolingoContext _context;
        private AiService _aiService;
        public PracticalTaskController(AiService ai, ILogger<HomeController> logger, SignInManager<DuolingoUser> signInManager, UserManager<DuolingoUser> userManager, DuolingoContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _aiService = ai;
        }


        public IActionResult GetTaskPage(int taskId)
        {

            var task = _context.Tasks.Include(s => s.Subject).FirstOrDefault(i => i.Id == taskId); ;


            return View(task);
        }


        public async Task<ActionResult> CheckTask(int taskId, string answer)
        {
            var taskToDo = _context.Tasks.Include(s => s.Subject).FirstOrDefault(p => p.Id == taskId);

            if (taskToDo == null)
            {
                TempData["response"] = "Nie znaleziono zadania.";
                return RedirectToAction("GetTaskPage"); 
            }

            ResponseModel response = await _aiService.CheckTaskCorrectnessAsync(taskToDo.TaskToDo, answer);

            taskToDo.isPassed = response.IsCorrect;
            if (response.IsCorrect)
            {
                var user = await _userManager.GetUserAsync(User);
                user.DoneTasks++;
            }
            TempData["response"] = response.Review;

            _context.Update(taskToDo);
            await _context.SaveChangesAsync(); 

            return View("GetTaskPage", taskToDo);
        }


    }
}