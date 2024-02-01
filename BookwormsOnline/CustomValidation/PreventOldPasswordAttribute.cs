using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookwormsOnline.CustomValidation
{
    public class PreventOldPasswordAttribute : ValidationAttribute
    {
        private readonly int passwordHistoryLimit = 2;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var userManager = (UserManager<BookwormsUser>)validationContext.GetService(typeof(UserManager<BookwormsUser>));
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var serviceProvider = (IServiceProvider)validationContext.GetService(typeof(IServiceProvider));

            var user = userManager.GetUserAsync(httpContextAccessor.HttpContext.User).Result;
            var context = (AuthDbContext)serviceProvider.GetService(typeof(AuthDbContext));

            var passwordHistory = context.PasswordHistory
                .Where(ph => ph.UserId == user.Id)
                .OrderByDescending(ph => ph.CreatedAt)
                .Take(passwordHistoryLimit)
                .Select(ph => ph.LoggedPasswordHash)
                .ToList();

            foreach (var hashedpassword in passwordHistory)
            {
                var verificationResult = userManager.PasswordHasher.VerifyHashedPassword(user, hashedpassword, value.ToString());

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    return new ValidationResult("Password has been used recently and cannot be reused.");
                }
            }
       
            return ValidationResult.Success;
        }

    }
}
