using System;
using System.Web;

namespace Owasp.Esapi.Runtime.Conditions
{
	/// <summary>
	///     Handler match condition
	/// </summary>
	public class HandlerCondition : ICondition
	{
		/// <summary>
		///     Initialize handler condition
		/// </summary>
		public HandlerCondition()
		{
			HandlerType = null;
		}

		/// <summary>
		///     Initialize handler condition
		/// </summary>
		/// <param name="handlerType">Handler type to match</param>
		public HandlerCondition(Type handlerType)
		{
			if (handlerType == null) throw new ArgumentNullException();
			HandlerType = handlerType;
		}

		/// <summary>
		///     Handler type to match
		/// </summary>
		public Type HandlerType { get; set; }

		#region ICondition Members

		/// <summary>
		///     Evaluate handler condition
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public bool Evaluate(ConditionArgs args)
		{
			var isMatch = false;

			var handler = HttpContext.Current != null ? HttpContext.Current.CurrentHandler : null;

			if (handler != null && HandlerType != null) isMatch = handler.GetType().Equals(HandlerType);

			return isMatch;
		}

		#endregion
	}
}