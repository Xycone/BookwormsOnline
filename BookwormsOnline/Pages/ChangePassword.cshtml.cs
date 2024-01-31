using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using BookwormsOnline.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BookwormsOnline.Pages
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public ChangePassword CPModel { get; set; }

        private UserManager<BookwormsUser> userManager { get; }

        public ChangePasswordModel(UserManager<BookwormsUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    var changePasswordResult = await userManager.ChangePasswordAsync(user, CPModel.CurrentPassword, CPModel.NewPassword);

                    if (changePasswordResult.Succeeded)
                    {
                        return RedirectToPage("UserInfo");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password change failed. Please try again");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Unable to load user for password change");
                }
            }

            return Page();
        }

    }
}
