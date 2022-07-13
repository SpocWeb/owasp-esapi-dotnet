using System;
using System.Web.Security;

namespace Owasp.Esapi.Swingset
{
	public partial class ForgotPassword : SwingsetPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			var userName = txtUserName.Text;
			var user = Membership.GetUser(userName);
			if (user == null)
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("Non-existent user {0} unsuccessfully requested password reset email.", userName));
			}
			else if (!user.IsApproved)
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("Non-active user {0} unsuccessfully requested password reset email.", userName));
			}
			else
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("User {0} requested password reset email.", userName));
				user.Comment = Esapi.Randomizer.GetRandomGUID().ToString();
				Membership.UpdateUser(user);
				var resetUrl = Request.Url.ToString().Replace("ForgotPassword.aspx",
					string.Format("PasswordReset.aspx?username={0}&token={1}", userName, user.Comment));
				var body = FileUtil.RetrieveFileBody("ForgotPasswordBody.txt").Replace("@ResetUrl", resetUrl);
				MailUtil.SendMail(user.Email, "Forgot Password Email", body);
			}
			Response.Redirect("Message.aspx?message=4");
		}
	}
}