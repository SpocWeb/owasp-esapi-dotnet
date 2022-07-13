using System;
using System.Web.Security;

namespace Owasp.Esapi.Swingset
{
	public partial class PasswordReset : SwingsetPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var userName = Request.QueryString.Get("username");
			var resetGuid = Request.QueryString.Get("token");
			var user = Membership.GetUser(userName);
			if (user == null)
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("Non-existent User {0} unsuccessfully attempted to reset password.", userName));
			}
			else if (!user.IsApproved)
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("Non-active User {0} unsuccessfully attempted to reset password.", userName));
			}
			else if (!(user.Comment == resetGuid))
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("User {0} unsuccessfully attempted to reset password (bad token).", userName));
			}
			else
			{
				logger.Info(LogEventTypes.SECURITY,
					string.Format("User {0} successfully accessed reset password form.", userName));
				lblSecretQuestion.Text = user.PasswordQuestion;
				Context.Items["user"] = user;
				return;
			}
			Response.Redirect("Error.aspx");
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				var user = (MembershipUser) Context.Items["user"];
				var secretAnswer = txtSecretAnswer.Text;
				string tempPassword = null;
				try
				{
					tempPassword = user.ResetPassword(secretAnswer);
					logger.Info(LogEventTypes.SECURITY,
						string.Format("User {0} supplied the correct answer to the secret question.", user.UserName));
				}
				catch (MembershipPasswordException mpe)
				{
					lblError.Text = "The answer to the secret question was not correct.";
					logger.Warning(LogEventTypes.SECURITY,
						string.Format("User {0} supplied the wrong answer to the secret question.", user.UserName),
						mpe);
				}
				if (tempPassword != null)
				{
					var newPassword = txtNewPassword.Text;
					user.ChangePassword(tempPassword, txtNewPassword.Text);
					user.Comment = null;
					Membership.UpdateUser(user);
					logger.Info(LogEventTypes.SECURITY,
						string.Format("User {0} successfully changed their password.", user.UserName));
					Response.Redirect("Message.aspx?message=2");
				}
			}
		}
	}
}