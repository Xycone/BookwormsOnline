using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BookwormsOnline.EmailSender
{
	public class EmailSender
	{
		private readonly string _sendGridApiKey;

		public EmailSender(IConfiguration configuration)
		{
			_sendGridApiKey = configuration["SendGrid:ApiKey"];
		}

		public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
		{
			var client = new SendGridClient(_sendGridApiKey);
			var from = new EmailAddress("seanang123456@gmail.com", "Sean");
			var to = new EmailAddress(toEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);

			try
			{
				await client.SendEmailAsync(msg);

				// Email sent successfully, display success message
				return true;
			}
			catch (Exception ex)
			{
				// Log or handle the exception appropriately
				Console.WriteLine($"Error sending email: {ex.Message}");

				// Display an error message
				return false;
			}
		}
	}
}
