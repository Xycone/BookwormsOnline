using BookwormsOnline.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookwormsOnline.Pages
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class UserInfoModel : PageModel
    {
		private readonly UserManager<BookwormsUser> userManager;

		public UserInfoModel(UserManager<BookwormsUser> userManager)
		{
			this.userManager = userManager;
		}

		public UserInfo UserInfo { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			// Decrypt
			var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
			var protector = dataProtectionProvider.CreateProtector("MySecretKey");
			var decryptedCreditCardNo = protector.Unprotect(user.CreditCardNo);

			UserInfo = new UserInfo
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				CreditCardNo = decryptedCreditCardNo,
				MobileNo = user.MobileNo,
				BillingAddress = user.BillingAddress,
				ShippingAddress = user.ShippingAddress,
				Photo = user.Photo
			};

			return Page();
		}
    }
}
