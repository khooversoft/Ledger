using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LedgerMenu.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public IActionResult OnGetAsync()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Main");
            }

            return Page();
        }
    }
}
