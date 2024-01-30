using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using BookwormsOnline.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BookwormsOnline.Pages
{
    public class RegisterModel : PageModel
    {

        private UserManager<BookwormsUser> userManager { get; }
        private SignInManager<BookwormsUser> signInManager { get; }
		private readonly AuthDbContext activityLogsDbContext;

		[BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<BookwormsUser> userManager, SignInManager<BookwormsUser> signInManager, AuthDbContext activityLogsDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.activityLogsDbContext = activityLogsDbContext;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");


			var user = new BookwormsUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    CreditCardNo = protector.Protect(RModel.CreditCardNo),
					MobileNo = RModel.MobileNo,
                    BillingAddress = RModel.BillingAddress,
                    ShippingAddress = RModel.ShippingAddress,
					Photo = await ConvertFormFileToByteArray(RModel.Photo)
				};

				var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
					var logSuccessfulEntry = new LogEntry
					{
						Email = RModel.Email,
						Activity = "Login Attempted - Successful",
						Time = DateTime.UtcNow
					};

					activityLogsDbContext.LogEntries.Add(logSuccessfulEntry);
					activityLogsDbContext.SaveChanges();

					return RedirectToPage("UserInfo");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

		private async Task<byte[]> ConvertFormFileToByteArray(IFormFile file)
		{
			using (var memoryStream = new MemoryStream())
			{
				await file.CopyToAsync(memoryStream);
				return memoryStream.ToArray();
			}
		}

	}
}
