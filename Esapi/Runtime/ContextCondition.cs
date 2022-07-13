using System;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     Runtime context condition
	/// </summary>
	internal class ContextCondition : IContextCondition
	{
		public ContextCondition(ICondition condition)
			: this(condition, true)
		{
		}

		/// <summary>
		///     Initialize condition
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="result"></param>
		public ContextCondition(ICondition condition, bool result)
		{
			if (condition == null) throw new ArgumentNullException();
			Condition = condition;
			Result = result;
		}

		#region IContextCondition implementation

		public ICondition Condition { get; }

		public bool Result { get; set; }

		#endregion
	}
}