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
        private readonly AuthDbContext context;

        public ChangePasswordModel(UserManager<BookwormsUser> userManager, AuthDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Ensures that the user does not change password too quickly
                    var lastPasswordChange = context.PasswordHistory
                        .Where(ph => ph.UserId == user.Id)
                        .OrderByDescending(ph => ph.CreatedAt)
                        .FirstOrDefault();

                    if (lastPasswordChange != null)
                    {
                        // password minimum duration
                        var minimumDuration = TimeSpan.FromMinutes(2);

                        var timeDifference = DateTime.UtcNow - lastPasswordChange.CreatedAt;

                        if (timeDifference < minimumDuration)
                        {
                            ModelState.AddModelError("", $"You cannot change your password again so soon. Please wait for at least {minimumDuration.TotalMinutes} minute(s).");
                            return Page();
                        }
                    }


                    var changePasswordResult = await userManager.ChangePasswordAsync(user, CPModel.CurrentPassword, CPModel.NewPassword);

                    if (changePasswordResult.Succeeded)
                    {
                        // Logs hashed password associated to user for password history later on
                        var newPasswordHash = userManager.PasswordHasher.HashPassword(user, CPModel.NewPassword);

                        var passwordHistoryEntry = new PasswordHistory
                        {
                            UserId = user.Id,
                            LoggedPasswordHash = newPasswordHash,
                            CreatedAt = DateTime.UtcNow
                        };

                        context.PasswordHistory.Add(passwordHistoryEntry);
                        context.SaveChanges();

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
