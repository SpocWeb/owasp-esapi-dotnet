using Owasp.Esapi.Errors;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi.Runtime.Actions
{
	/// <summary>
	///     Log threshold exceeded action
	/// </summary>
	[Action(BuiltinActions.Log)]
	public class LogAction : IAction
	{
		#region IAction Members

		/// <summary>
		///     Execute action
		/// </summary>
		/// <param name="args">Arguments</param>
		public void Execute(ActionArgs args)
		{
			if (args == null) return;

			var intrusionException = args.FaultException as IntrusionException;
			if (intrusionException != null) Esapi.Logger.Fatal(LogEventTypes.SECURITY, intrusionException.LogMessage);
		}

		#endregion
	}
}