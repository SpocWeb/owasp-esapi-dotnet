using System;
using System.Web.Security;

namespace Owasp.Esapi.Swingset
{
	public partial class Activate : SwingsetPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var userName = Request.QueryString["username"];
			var activationGuid = Request.QueryString["token"];
			var user = Membership.GetUser(userName);
			if (user == null)
			{
				logger.Warning(LogEventTypes.SECURITY,
					string.Format("Non-existent user {0} unsuccessfully attempted to activate account.", userName));
			}
			else if (user.IsApproved)
			{
				logger.Warning(LogEventTypes.SECURITY,
					string.Format("Non-active user {0} unsuccessfully attempted to activate account.", userName));
			}
			else if (!(user.Comment == activationGuid))
			{
				logger.Warning(LogEventTypes.SECURITY,
					string.Format("User {0} unsuccessfully attempted to activate account (bad token).", userName));
			}
			else
			{
				user.IsApproved = true;
				user.Comment = null;
				Membership.UpdateUser(user);
				logger.Info(LogEventTypes.SECURITY,
					string.Format("User {0} successfully activated account.", userName));
				Response.Redirect("Message.aspx?message=1");
			}
			Response.Redirect("Error.aspx");
		}
	}
}