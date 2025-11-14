using ChatApplication.API.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace ChatApplication.API.Services.EmailService;

public class EmailService : IEmailSender
{
	private readonly EmailSettings _emailSettings;

	public EmailService(IOptions<EmailSettings> emailSettings)
	{
		_emailSettings = emailSettings.Value;
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var message = new MimeMessage
		{
			Sender = MailboxAddress.Parse(_emailSettings.Email),//من مين
			Subject = subject
		};

		message.To.Add(MailboxAddress.Parse(email));//لمين

		var builder = new BodyBuilder
		{
			HtmlBody = htmlMessage //Template
		};

		message.Body = builder.ToMessageBody();
		using var smtp = new SmtpClient();// المسؤل عن إرسال الإميل

		smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
		smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
		await smtp.SendAsync(message);
		smtp.Disconnect(true);
	}
}