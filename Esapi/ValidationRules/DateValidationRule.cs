using System;
using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.ValidationRules
{
	/// <summary>
	///     This class performs date validation.
	/// </summary>
	[ValidationRule(BuiltinValidationRules.Date)]
	public class DateValidationRule : IValidationRule
	{
		/// <summary>
		///     Date min value
		/// </summary>
		public DateTime MinValue { get; set; } = DateTime.MinValue;

		/// <summary>
		///     Date maximum value
		/// </summary>
		public DateTime MaxValue { get; set; } = DateTime.MaxValue;

		#region IValidationRule Members

		/// <summary>
		///     Checks whether the input is a valid date.
		/// </summary>
		/// <param name="input">The input to valdiate.</param>
		/// <returns>True, if the input is valid. False, otherwise.</returns>
		public bool IsValid(string input)
		{
			DateTime value;
			if (!DateTime.TryParse(input, out value)) return false;

			return value >= MinValue && value <= MaxValue;
		}

		#endregion
	}
}