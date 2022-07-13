using System;

namespace Owasp.Esapi.Runtime.Conditions
{
	/// <summary>
	///     Verify parameters existence
	/// </summary>
	public class ParametersCondition : ICondition
	{
		#region ICondition Members

		/// <summary>
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public bool Evaluate(ConditionArgs args)
		{
			if (args == null) throw new ArgumentNullException("args");

			//TODO
			return false;
		}

		#endregion
	}
}