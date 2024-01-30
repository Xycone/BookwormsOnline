using BookwormsOnline.Extensions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookwormsOnline.CustomValidation
{
	public class UniqueEmailAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var userManager = (UserManager<BookwormsUser>)validationContext
				.GetService(typeof(UserManager<BookwormsUser>));

			if (value == null)
			{
				return ValidationResult.Success;
			}
			else {
				var existingUser = userManager.FindByEmailAsync(value.ToString()).Result;

				if (existingUser != null)
				{
					return new ValidationResult(ErrorMessage ?? "Email address is already in use.");
				}

				return ValidationResult.Success;
			}
		}
	}
}
