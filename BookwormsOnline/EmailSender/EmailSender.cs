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

		public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
		{
			var client = new SendGridClient(_sendGridApiKey);
			var from = new EmailAddress("seanang123456@gmail.com", "Sean");
			var to = new EmailAddress(toEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);

			await client.SendEmailAsync(msg);
		}
	}
}
