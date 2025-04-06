using Duolingo.Areas.Identity.Data;
using Duolingo.Data;
using Microsoft.AspNetCore.Identity;

namespace Duolingo.Services
{
    public class GamificationService
    {
        public readonly SignInManager<DuolingoUser> _signInManager;
        private readonly UserManager<DuolingoUser> _userManager;
        private readonly DuolingoContext _context;
        private AiService _aiService;
        public GamificationService(AiService ai, SignInManager<DuolingoUser> signInManager, UserManager<DuolingoUser> userManager, DuolingoContext context)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _aiService = ai;
        }

        public List<String> GetBestUsers()
        {
            var topUsers = _context.Users
    .OrderByDescending(p => p.DoneTasks)
    .Take(3).Select(a=>a.UserName)
    .ToList();


            return topUsers;
        }

    }
}
