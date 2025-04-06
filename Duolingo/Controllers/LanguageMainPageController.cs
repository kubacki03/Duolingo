using Duolingo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Duolingo.Controllers
{
    public class LanguageMainPageController : Controller
    {
        public LanguageMainPageController() { }


        public IActionResult GetMainPage(LanguagesEnum language)
        {



            return View(language);
        }
        public IActionResult GetLevelPage(LevelEnum level, LanguagesEnum language)
        {
            // Convert the enums to strings before passing them to the view
            var model = KeyValuePair.Create(language.ToString(), level.ToString());
            TempData["SelectedCourse"] = "Kurs: " + language + " " + level;
            return View(model);
        }

    }
}