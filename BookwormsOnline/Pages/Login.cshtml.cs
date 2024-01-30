using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using BookwormsOnline.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace BookwormsOnline.Pages
{
	public class MyObject
	{
		public bool success { get; set; }
		public float score { get; set; }
		public List<string> errorMessage { get; set; }


	}

	public class LoginModel : PageModel
    {
		[BindProperty]
        public Login LModel { get; set; }

		[BindProperty]
		public string RecaptchaToken { get; set; }

		private readonly SignInManager<BookwormsUser> signInManager;
		private readonly AuthDbContext activityLogsDbContext;

		public LoginModel(SignInManager<BookwormsUser> signInManager, AuthDbContext activityLogsDbContext)
		{
			this.signInManager = signInManager;
			this.activityLogsDbContext = activityLogsDbContext;
        }

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ValidateCaptcha())
			{
				ModelState.AddModelError("", "Please complete the reCAPTCHA verification.");
				return Page();
			}

			if (ModelState.IsValid)
			{
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
				LModel.RememberMe, lockoutOnFailure: true);

                if (identityResult.Succeeded)
				{
					var logSuccessfulEntry = new LogEntry
					{
						Email = LModel.Email,
						Activity = "Login Attempted - Successful",
						Time = DateTime.UtcNow
					};

					activityLogsDbContext.LogEntries.Add(logSuccessfulEntry);
					activityLogsDbContext.SaveChanges();

					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Email,  LModel.Email)
					};
					var i = new ClaimsIdentity(claims, "MyCookieAuth");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
					await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

					return RedirectToPage("UserInfo");
				}

				if (identityResult.IsLockedOut)
				{
					ModelState.AddModelError("", "Account is locked due to too many failed login attempts. Please try again later.");
					return Page();
				}
				else
				{
					var logFailedEntry = new LogEntry
					{
						Email = LModel.Email,
						Activity = "Login Attempted - Failed",
						Time = DateTime.UtcNow
					};

					activityLogsDbContext.LogEntries.Add(logFailedEntry);
					activityLogsDbContext.SaveChanges();

					ModelState.AddModelError("", "Invalid Login Attempt, Username or Password incorrect");
				}
			}

			return Page();
		}

		private bool ValidateCaptcha()
		{

			string secretKey = "6LezN2EpAAAAAKo_nT0CBhPNbybyFYljGM30n5VX";
			string captchaResponse = RecaptchaToken;

			string apiUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaResponse}";

			using (var client = new WebClient())
			{
				string jsonResponse = client.DownloadString(apiUrl);
				var jsonObject = JsonSerializer.Deserialize<MyObject>(jsonResponse);

				if (jsonObject != null && jsonObject.success)
				{
					// I wanted to check the score :)
					System.Diagnostics.Debug.WriteLine(jsonObject.score);
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
