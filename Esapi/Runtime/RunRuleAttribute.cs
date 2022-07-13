using System;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     Request rule execution at runtime
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class RunRuleAttribute : Attribute
	{
		Type _ruleType;

		/// <summary>
		///     Initialize required rule to run
		/// </summary>
		/// <param name="ruleType">Rule type</param>
		public RunRuleAttribute(Type ruleType)
			: this(ruleType, null)
		{
		}

		/// <summary>
		///     Initialize required rule to run
		/// </summary>
		/// <param name="ruleType">Rule type</param>
		/// <param name="faultActions">Actions to run on rule failure</param>
		public RunRuleAttribute(Type ruleType, Type[] faultActions)
		{
			if (ruleType == null) throw new ArgumentNullException();
			_ruleType = ruleType;
			FaultActions = faultActions;
		}

		/// <summary>
		///     Type of rule to run
		/// </summary>
		public Type Rule
		{
			get => _ruleType;
			set
			{
				if (value == null) throw new ArgumentNullException();
				_ruleType = value;
			}
		}

		/// <summary>
		///     Actions to run if the rule fails
		/// </summary>
		public Type[] FaultActions { get; set; }
	}
}