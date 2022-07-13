using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.ValidationRules
{
	/// <summary>
	///     This class performs double (decimal) validation.
	/// </summary>
	[ValidationRule(BuiltinValidationRules.Double)]
	public class DoubleValidationRule : IValidationRule
	{
		/// <summary>
		///     Minimum value
		/// </summary>
		public double MinValue { get; set; } = double.MinValue;

		/// <summary>
		///     Maximum value
		/// </summary>
		public double MaxValue { get; set; } = double.MaxValue;

		#region IValidationRule Members

		/// <summary>
		///     Checks whether the input is a valid double.
		/// </summary>
		/// <param name="input">The input to valdiate.</param>
		/// <returns>True, if the input is valid. False, otherwise.</returns>
		public bool IsValid(string input)
		{
			double value;

			if (!double.TryParse(input, out value)) return false;

			return !(double.IsInfinity(value) || double.IsNaN(value)) && value >= MinValue && value <= MaxValue;
		}

		#endregion
	}
}