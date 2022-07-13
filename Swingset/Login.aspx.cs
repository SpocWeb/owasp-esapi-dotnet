using System;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Owasp.Esapi.Swingset
{
	public partial class Login : SwingsetPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//if (User.IsInRole("user"))
			//{
			//	Response.Redirect("Users/Default.aspx");
			//}
			//if (User.IsInRole("admin"))
			//{
			//	Response.Redirect("Administrators/Default.aspx");
			//}

			SiteMapPath smpSwingset;
			smpSwingset = (SiteMapPath) Master.FindControl("smpSwingset");
			if (smpSwingset != null) smpSwingset.Visible = false;
		}

		protected void EsapiLogin_LoggedIn(object sender, EventArgs e)
		{
			var userName = EsapiLogin.UserName;
			logger.Info(LogEventTypes.SECURITY, string.Format("User {0} has successfully logged in.", userName));
		}

		protected void EsapiLogin_LoginError(object sender, EventArgs e)
		{
			var userName = EsapiLogin.UserName;
			var user = Membership.GetUser(userName);
			if (user == null)
				logger.Warning(LogEventTypes.SECURITY,
					string.Format("The login attempt failed for user {0}.", userName));
			else if (user.IsLockedOut)
				logger.Warning(LogEventTypes.SECURITY,
					string.Format("The login attempt failed for user {0} because the user is locked out.", userName));
			else if (!user.IsApproved)
				logger.Warning(LogEventTypes.SECURITY,
					string.Format("The login attempt failed for user {0} because the user is not yet approved.",
						userName));
			else
				logger.Warning(LogEventTypes.SECURITY,
					string.Format(
						"The login attempt failed for user {0} because they did not supply the correct password.",
						userName));
		}

		protected void EsapiLogin_Authenticate(object sender, AuthenticateEventArgs e)
		{
			var userName = EsapiLogin.UserName;
			var password = EsapiLogin.Password;
			if (Membership.GetUser(userName) != null && Membership.GetUser(userName).IsOnline)
				// e.Authenticated = false;
				logger.Warning(LogEventTypes.SECURITY,
					string.Format(
						"User has attempted to log in with a user account {0} that already has an active session.",
						userName));
			e.Authenticated = Membership.ValidateUser(userName, password);
		}
	}
}