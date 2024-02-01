using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BookwormsOnline.Pages
{
    public class LogoutModel : PageModel
    {

		private readonly SignInManager<BookwormsUser> signInManager;
		private readonly AuthDbContext context;
		public LogoutModel(SignInManager<BookwormsUser> signInManager, AuthDbContext context)
		{
			this.signInManager = signInManager;
			this.context = context;
		}


		public async Task<IActionResult> OnPostLogoutAsync()
		{
			// retrieve user email from claims
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var logLogout = new LogEntry
			{
				Email = userEmail,
				Activity = "Logout",
				Time = DateTime.UtcNow
			};

			// Signs user out and logs the sign out in the db
			await signInManager.SignOutAsync();
			try
			{
				HttpContext.Session.Clear();
				context.LogEntries.Add(logLogout);
				context.SaveChanges();

                return RedirectToPage("Login");
            }
			catch (Exception ex)
			{
                Console.Error.WriteLine($"Error logging sign-out: {ex.Message}");
                return RedirectToPage("Login");
            }
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("UserInfo");
		}
	}
}
