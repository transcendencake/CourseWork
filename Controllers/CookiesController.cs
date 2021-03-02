using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;

namespace CourseWork.Controllers
{
    public class CookiesController : Controller
    {
        [HttpPost]
        public IActionResult SetTheme(string returnUrl, string theme)
        {
            HttpContext.Response.Cookies.Delete("theme");
            HttpContext.Response.Cookies.Append("theme", theme);

            return Redirect(returnUrl);
        }

        [HttpPost]
        public IActionResult SetLanguage(string returnUrl, string language)
        {
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)));

            return Redirect(returnUrl);
        }
    }
}
