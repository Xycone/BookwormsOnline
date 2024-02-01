using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using BookwormsOnline.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookwormsOnline.Pages
{
    public class ResetPasswordModel : PageModel
    {
		[BindProperty]
		public ResetPassword RPModel { get; set; }

		private readonly UserManager<BookwormsUser> userManager;
		private readonly AuthDbContext context;

		public ResetPasswordModel(UserManager<BookwormsUser> userManager, AuthDbContext context)
		{
			this.userManager = userManager;
			this.context = context;
		}

		// Retrieve info from url
		public async Task<IActionResult> OnGetAsync(string userId, string code, string email)
		{
			if (userId == null || code == null || email == null)
			{
				// Handle the case where the required parameters are missing
				return RedirectToAction("Error");
			}

			// Store the parameters in TempData to be used in the POST action
			TempData["ResetEmail"] = email;
			TempData["ResetCode"] = code;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var code = TempData["ResetCode"]?.ToString();
			var email = TempData["ResetEmail"]?.ToString();

			if (code == null || email == null)
			{
				return RedirectToAction("Register");
			}


			var user = await userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return RedirectToPage("/Register");
			}

			// Use the token and user ID to reset the user's password
			var result = await userManager.ResetPasswordAsync(user, code, RPModel.NewPassword);

			if (result.Succeeded)
			{
				// Logs hashed password associated to user for password history later on
				var newPasswordHash = userManager.PasswordHasher.HashPassword(user, RPModel.NewPassword);

				var passwordHistoryEntry = new PasswordHistory
				{
					UserId = user.Id,
					LoggedPasswordHash = newPasswordHash,
					CreatedAt = DateTime.UtcNow
				};

				context.PasswordHistory.Add(passwordHistoryEntry);
				context.SaveChanges();

				// Password reset successful, you can redirect to a confirmation page.
				return RedirectToPage("/Login");
			}

			return Page();
		}

	}
}
