using System.Net.Mail;

namespace Owasp.Esapi.Swingset
{
	public class MailUtil
	{
		public static void SendMail(string EmailAddress, string Subject, string Body)
		{
			var mailMessage = new MailMessage();
			mailMessage.To.Add(EmailAddress);
			mailMessage.Subject = Subject;
			mailMessage.Body = Body;
			mailMessage.IsBodyHtml = false;
			var smtp = new SmtpClient();
			smtp.Send(mailMessage);
		}
	}
}