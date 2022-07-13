using System;
using System.Web;

namespace Owasp.Esapi.Runtime.Actions
{
	/// <summary>
	///     Redirect request
	/// </summary>
	[Action(BuiltinActions.Redirect, AutoLoad = false)]
	internal class RedirectAction : IAction
	{
		string _url;

		/// <summary>
		///     Initialize redirect action
		/// </summary>
		/// <param name="url">Url to redirect to</param>
		public RedirectAction(string url)
		{
			if (string.IsNullOrEmpty(url)) throw new ArgumentException();

			_url = url;
		}

		/// <summary>
		///     Redirect URL
		/// </summary>
		public string Url
		{
			get => _url;
			set
			{
				if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
				_url = value;
			}
		}

		#region IAction Members

		/// <summary>
		///     Execute redirect action
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>Will terminate the current request</remarks>
		public void Execute(ActionArgs args)
		{
			var response = HttpContext.Current != null ? HttpContext.Current.Response : null;
			if (response == null) throw new InvalidOperationException();


			response.Redirect(_url, true);
		}

		#endregion
	}
}