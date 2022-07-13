using System;
using System.Web;

namespace Owasp.Esapi.Runtime.Actions
{
	/// <summary>
	///     Transfer request
	/// </summary>
	[Action(BuiltinActions.Transfer, AutoLoad = false)]
	internal class TransferAction : IAction
	{
		string _url;

		/// <summary>
		///     Initialize transfer action
		/// </summary>
		/// <param name="url">Url to transfer to</param>
		public TransferAction(string url)
		{
			if (string.IsNullOrEmpty(url)) throw new ArgumentException();

			_url = url;
		}

		/// <summary>
		///     Transfer URL
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
		public void Execute(ActionArgs args)
		{
			var context = HttpContext.Current;
			if (context == null) throw new InvalidOperationException();

			context.Server.TransferRequest(_url);
		}

		#endregion
	}
}