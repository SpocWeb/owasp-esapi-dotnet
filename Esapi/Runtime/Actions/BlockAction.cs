using System;
using System.Web;

namespace Owasp.Esapi.Runtime.Actions
{
	/// <summary>
	///     Block current request action
	/// </summary>
	[Action(BuiltinActions.Block)]
	public class BlockAction : IAction
	{
		/// <summary>
		///     Block HTTP status code
		/// </summary>
		public int StatusCode { get; set; } = 403;

		#region IAction Members

		/// <summary>
		///     Block current request
		/// </summary>
		/// <param name="args"></param>
		/// <remarks>Will end the current request</remarks>
		public void Execute(ActionArgs args)
		{
			var response = HttpContext.Current != null ? HttpContext.Current.Response : null;

			if (null == response) throw new InvalidOperationException();

			response.ClearHeaders();
			response.ClearContent();

			response.StatusCode = StatusCode;
			response.End();
		}

		#endregion
	}
}