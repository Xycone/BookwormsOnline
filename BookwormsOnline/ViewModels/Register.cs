using BookwormsOnline.CustomValidation;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace BookwormsOnline.ViewModels
{
    public class Register
    {
        [Required]
		[DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg" })]
		public IFormFile Photo { get; set; }

		[Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit Card No consists of 16 digits")]
        public string CreditCardNo {  get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Mobile No can only contain numbers from 1-9")]
        public string MobileNo { get; set; }

        [Required]
        public string BillingAddress { get; set; }

        [Required]
        public string ShippingAddress {  get; set; }

        [Required]
		[UniqueEmail(ErrorMessage = "Email address is already in use.")]
		[DataType(DataType.EmailAddress)]
        [RegularExpression(@"^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$", ErrorMessage = "Passwords must be at least 12 characters long and contain at least an uppercase letter, lowercase letter, digit and a symbol")]

		public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
	}
}
