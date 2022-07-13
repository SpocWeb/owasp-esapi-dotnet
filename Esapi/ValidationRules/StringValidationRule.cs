using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Owasp.Esapi.Interfaces;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi.ValidationRules
{
	/// <summary>
	/// </summary>
	[ValidationRule(BuiltinValidationRules.String, AutoLoad = false)]
	public class StringValidationRule : IValidationRule
	{
		readonly List<Regex> _blacklist;

		int _minLength;
		readonly List<Regex> _whitelist;

		/// <summary>
		///     Initialize string validation rule
		/// </summary>
		public StringValidationRule()
		{
			_whitelist = new List<Regex>();
			_blacklist = new List<Regex>();
		}

		/// <summary>
		///     Allow null or empty values
		/// </summary>
		public bool AllowNullOrEmpty { get; set; } = false;

		/// <summary>
		///     Minimum length value
		/// </summary>
		public int MinLength
		{
			get => _minLength;
			set
			{
				if (value < 0) throw new ArgumentException(EM.InvalidArgument);
				_minLength = value;
			}
		}

		/// <summary>
		///     Maximum length value
		/// </summary>
		public int MaxLength { get; set; } = int.MaxValue;

		#region IValidationRule Members

		/// <summary>
		///     Validate string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public bool IsValid(string input)
		{
			if (string.IsNullOrEmpty(input)) return AllowNullOrEmpty;

			// Check length
			if (input.Length < _minLength || input.Length > MaxLength) return false;

			// Check whitelist patterns
			foreach (var r in _whitelist)
				if (!r.IsMatch(input))
					return false;

			// Check blacklist patterns
			foreach (var r in _blacklist)
				if (r.IsMatch(input))
					return false;

			return true;
		}

		#endregion

		/// <summary>
		///     Add pattern to whitelist
		/// </summary>
		/// <param name="pattern">String pattern</param>
		public void AddWhitelistPattern(string pattern)
		{
			try
			{
				_whitelist.Add(new Regex(pattern));
			}
			catch (Exception exp)
			{
				throw new ArgumentException(EM.InvalidArgument, exp);
			}
		}

		/// <summary>
		///     Add pattern to blacklist
		/// </summary>
		/// <param name="pattern">String pattern</param>
		public void AddBlacklistPattern(string pattern)
		{
			try
			{
				_blacklist.Add(new Regex(pattern));
			}
			catch (Exception exp)
			{
				throw new ArgumentException(EM.InvalidArgument, exp);
			}
		}
	}
}