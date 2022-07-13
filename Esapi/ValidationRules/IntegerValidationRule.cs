using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.ValidationRules
{
	/// <summary>
	///     This class performs integer validation.
	/// </summary>
	[ValidationRule(BuiltinValidationRules.Integer)]
	public class IntegerValidationRule : IValidationRule
	{
		/// <summary>
		///     Minimum value
		/// </summary>
		public int MinValue { get; set; } = int.MinValue;

		/// <summary>
		///     Maximum value
		/// </summary>
		public int MaxValue { get; set; } = int.MaxValue;

		#region IValidationRule Members

		/// <summary>
		///     Checks whether the input is a valid integer.
		/// </summary>
		/// <param name="input">The input to valdiate.</param>
		/// <returns>True, if the input is valid. False, otherwise.</returns>
		public bool IsValid(string input)
		{
			int value;
			if (!int.TryParse(input, out value)) return false;

			return value >= MinValue && value <= MaxValue;
		}

		#endregion
	}
}