using System.Web.Security;

namespace Owasp.Esapi.Runtime.Actions
{
	/// <summary>
	///     FormsAuthentication Logout action
	/// </summary>
	[Action(BuiltinActions.FormsAuthenticationLogout)]
	public class FomsAuthenticationLogoutAction : IAction
	{
		#region IAction Members

		/// <summary>
		///     Logout user using FormsAuthentication
		/// </summary>
		/// <param name="args"></param>
		public void Execute(ActionArgs args)
		{
			FormsAuthentication.SignOut();
		}

		#endregion
	}
}