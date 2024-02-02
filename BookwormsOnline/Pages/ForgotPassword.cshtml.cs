using BookwormsOnline.EmailSender;
using BookwormsOnline.Extensions;
using BookwormsOnline.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Encodings.Web;

namespace BookwormsOnline.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public ForgotPassword FPModel { get; set; }

		private readonly UserManager<BookwormsUser> userManager;
		private readonly EmailSender.EmailSender emailSender;

		public ForgotPasswordModel(UserManager<BookwormsUser> userManager, EmailSender.EmailSender emailSender)
		{
			this.userManager = userManager;
			this.emailSender = emailSender;
		}

		public async Task<IActionResult> OnPost()
        {
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await userManager.FindByEmailAsync(FPModel.Email);

			if (user != null)
			{
				// Generate a token and send a reset link to the user's email
				var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

				var resetLink = Url.Page("/ResetPassword", pageHandler: null, values: new { area = "Identity", userId = user.Id, code = resetToken, email = user.Email }, protocol: Request.Scheme);
				var emailSubject = "Password Reset";
				var htmlLink = $"Click the following link to reset your password: <a href='{HtmlEncoder.Default.Encode(resetLink)}'>Reset Password Link</a>";

				// Try to send the email
				if (await emailSender.SendEmailAsync(user.Email, emailSubject, htmlLink))
				{
					// Email sent successfully, set model error
					ModelState.AddModelError("", "Reset link has been sent to the email");
				}
				else
				{
					ModelState.AddModelError("", "Something went wrong");
				}


			}
			else { 
				ModelState.AddModelError("", "User dont exist");
			}

			return Page();
		}
    }
}
