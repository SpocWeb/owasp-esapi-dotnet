namespace Owasp.Esapi.Runtime.Conditions
{
	/// <summary>
	///     Value bound condition
	/// </summary>
	public class ValueBoundCondition : ICondition
	{
		/// <summary>
		///     Initialize condition
		/// </summary>
		/// <param name="value">Bounded value</param>
		public ValueBoundCondition(bool value)
		{
			Value = value;
		}

		/// <summary>
		///     Bound value
		/// </summary>
		public bool Value { get; set; }

		#region ICondition Members

		/// <summary>
		///     Eval
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public bool Evaluate(ConditionArgs args)
		{
			return Value;
		}

		#endregion
	}
}