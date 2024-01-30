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
		private readonly AuthDbContext activityLogsDbContext;
		public LogoutModel(SignInManager<BookwormsUser> signInManager, AuthDbContext activityLogsDbContext)
		{
			this.signInManager = signInManager;
			this.activityLogsDbContext = activityLogsDbContext;
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
				activityLogsDbContext.LogEntries.Add(logLogout);
				activityLogsDbContext.SaveChanges();

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
