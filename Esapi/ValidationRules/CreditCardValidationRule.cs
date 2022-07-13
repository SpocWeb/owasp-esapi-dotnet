using System.Text;
using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.ValidationRules
{
	/// <summary>
	///     This class performs credit card number validation, including Luhn algorithm checking.
	/// </summary>
	[ValidationRule(BuiltinValidationRules.CreditCard)]
	public class CreditCardValidationRule : IValidationRule
	{
		#region IValidationRule Members

		/// <summary>
		///     Checks whether the input is a valid credit card number.
		/// </summary>
		/// <param name="input">The input to valdiate.</param>
		/// <returns>True, if the input is valid. False, otherwise.</returns>
		public bool IsValid(string input)
		{
			if (string.IsNullOrEmpty(input)) return false;

			// perform Luhn algorithm checking
			var digitsOnly = new StringBuilder();
			char c;
			for (var i = 0; i < input.Length; i++)
			{
				c = input[i];
				if (char.IsDigit(c)) digitsOnly.Append(c);
			}

			if (digitsOnly.Length > 18 || digitsOnly.Length < 15) return false;
			var sum = 0;
			var digit = 0;
			var addend = 0;
			var timesTwo = false;

			for (var i = digitsOnly.Length - 1; i >= 0; i--)
			{
				digit = int.Parse(digitsOnly.ToString(i, 1));
				if (timesTwo)
				{
					addend = digit * 2;
					if (addend > 9) addend -= 9;
				}
				else
				{
					addend = digit;
				}
				sum += addend;
				timesTwo = !timesTwo;
			}

			var modulus = sum % 10;
			return modulus == 0;
		}

		#endregion
	}
}