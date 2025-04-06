using System.Diagnostics;
using Duolingo.Areas.Identity.Data;
using Duolingo.Data;
using Duolingo.Models;
using Duolingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Duolingo.Controllers
{
    public class HomeController : Controller
    {
        public readonly SignInManager<DuolingoUser> _signInManager;
        private readonly UserManager<DuolingoUser> _userManager;
        private readonly DuolingoContext _context;
        private AiService _aiService;
        private GamificationService _gamificationService;
        public HomeController(AiService ai, SignInManager<DuolingoUser> signInManager, UserManager<DuolingoUser> userManager, DuolingoContext context, GamificationService gamificationService)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _aiService = ai;
            _gamificationService = gamificationService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var courses=_context.Courses.Where(p=>p.UserId==user.Id).ToList();

            

            TempData["BestUsers"] = _gamificationService.GetBestUsers();

            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
