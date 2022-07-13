﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Owasp.Esapi.Runtime.Conditions
{
	/// <summary>
	///     User test condition
	/// </summary>
	public class UserCondition : ICondition
	{
		/// <summary>
		///     Any user name pattern
		/// </summary>
		public const string AnyNamePattern = "*";

		List<string> _roles;

		Regex _userName;

		/// <summary>
		///     Initialize user test condition
		/// </summary>
		/// <param name="namePattern">User name pattern</param>
		public UserCondition(string namePattern)
			: this(namePattern, null)
		{
		}

		/// <summary>
		///     Initialize user test condition
		/// </summary>
		/// <param name="namePattern">User name pattern</param>
		/// <param name="roles">User roles</param>
		public UserCondition(string namePattern, IEnumerable<string> roles)
		{
			NamePattern = namePattern;
			Roles = roles;
		}

		/// <summary>
		///     Name pattern
		/// </summary>
		public string NamePattern
		{
			get => _userName.ToString();
			set
			{
				if (string.IsNullOrEmpty(value))
					_userName = new Regex("^$");
				else
					_userName = new Regex(value);
			}
		}

		/// <summary>
		///     User roles
		/// </summary>
		public IEnumerable<string> Roles
		{
			get => _roles;
			set => _roles = value == null ? new List<string>() : new List<string>(value);
		}

		#region ICondition Members

		/// <summary>
		///     Test if the current user identity and roles matches
		/// </summary>
		/// <returns></returns>
		public bool Evaluate(ConditionArgs args)
		{
			if (args == null) throw new ArgumentNullException("args");

			// Get user identity 
			var userPrincipal = Esapi.SecurityConfiguration.CurrentUser;
			var userIdentity = userPrincipal != null ? userPrincipal.Identity : null;

			if (userIdentity == null) return false;

			// Get user name
			var userName = userIdentity.Name;
			if (string.IsNullOrEmpty(userName)) return false;

			// Match user name
			if (!_userName.IsMatch(userName)) return false;

			// Match roles
			foreach (var role in _roles)
				if (!userPrincipal.IsInRole(role))
					return false;

			// Roles match
			return true;
		}

		#endregion
	}
}