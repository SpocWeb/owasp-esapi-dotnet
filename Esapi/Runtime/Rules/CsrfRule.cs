﻿using System;
using System.Web;
using System.Web.UI;

namespace Owasp.Esapi.Runtime.Rules
{
	/// <summary>
	///     Intrusion detection CSRF rule
	/// </summary>
	public class CsrfRule : IRule
	{
		/// <summary>
		///     Verify CSRF guard before page executes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPreRequestHandlerExecute(object sender, RuntimeEventArgs e)
		{
			// Get current page
			var currentPage = HttpContext.Current != null ? HttpContext.Current.CurrentHandler as Page : null;

			if (currentPage != null) // Add CSRF guard when page initializes
				currentPage.Init += (p, a) => Esapi.HttpUtilities.AddCsrfToken();
		}

		#region IRule Members

		/// <summary>
		///     Subscribe to events
		/// </summary>
		/// <param name="publisher"></param>
		public void Subscribe(IRuntimeEventPublisher publisher)
		{
			if (publisher == null) throw new ArgumentNullException();
			publisher.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
		}

		/// <summary>
		///     Disconnect from events
		/// </summary>
		/// <param name="publisher"></param>
		public void Unsubscribe(IRuntimeEventPublisher publisher)
		{
			if (publisher == null) throw new ArgumentNullException();

			publisher.PreRequestHandlerExecute -= OnPreRequestHandlerExecute;
		}

		#endregion
	}
}