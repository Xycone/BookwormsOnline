using Microsoft.AspNetCore.Identity;

namespace BookwormsOnline.Extensions
{
	public class UserInfo
	{
		public byte[] Photo { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string CreditCardNo { get; set; }

		public string MobileNo { get; set; }

		public string BillingAddress { get; set; }

		public string ShippingAddress { get; set; }

		public string Email { get; set; }
	}
}

