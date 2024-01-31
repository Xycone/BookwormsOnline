using BookwormsOnline.Extensions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookwormsOnline.CustomValidation
{
    public class ValidateCurrentPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var userManager = (UserManager<BookwormsUser>)validationContext.GetService(typeof(UserManager<BookwormsUser>));
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var user = userManager.GetUserAsync(httpContextAccessor.HttpContext.User).Result;

            if (userManager.CheckPasswordAsync(user, value.ToString()).Result)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The current password is incorrect.");
        }
    }
}
