using System;
using System.Web;

namespace Owasp.Esapi.Runtime.Rules
{
	/// <summary>
	///     Clickjack detection rule
	/// </summary>
	public class ClickjackRule : IRule
	{
		/// <summary>
		///     Framing mode
		/// </summary>
		public enum FramingModeType
		{
			/// <summary>
			///     Deny framing
			/// </summary>
			Deny,

			/// <summary>
			///     Allow only same domain
			/// </summary>
			Sameorigin
		}

		const string HeaderName = "X-FRAME-OPTIONS";
		const string DenyValue = "DENY";
		const string SameoriginValue = "SAMEORIGIN";

		/// <summary>
		///     Initialize clickjack rule
		/// </summary>
		public ClickjackRule()
		{
			FramingMode = FramingModeType.Deny;
		}

		/// <summary>
		///     Initialize clickjack rule
		/// </summary>
		/// <param name="mode">Framing mode type</param>
		public ClickjackRule(FramingModeType mode)
		{
			FramingMode = mode;
		}

		/// <summary>
		///     Framing mode type
		/// </summary>
		public FramingModeType FramingMode { get; set; }

		/// <summary>
		///     Add clickjack headers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPostRequestHandlerExecute(object sender, RuntimeEventArgs e)
		{
			// Get response
			var response = HttpContext.Current != null ? HttpContext.Current.Response : null;
			if (response == null) throw new InvalidOperationException();

			// Add clickjack protection
			switch (FramingMode)
			{
				case FramingModeType.Deny:
					response.AddHeader(HeaderName, DenyValue);
					break;
				case FramingModeType.Sameorigin:
					response.AddHeader(HeaderName, SameoriginValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#region IRule Members

		/// <summary>
		///     Subscribe to events
		/// </summary>
		/// <param name="publisher"></param>
		public void Subscribe(IRuntimeEventPublisher publisher)
		{
			if (publisher == null) throw new ArgumentNullException();
			publisher.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
		}

		/// <summary>
		///     Disconnect from events
		/// </summary>
		/// <param name="publisher"></param>
		public void Unsubscribe(IRuntimeEventPublisher publisher)
		{
			if (publisher == null) throw new ArgumentNullException();
			publisher.PostRequestHandlerExecute -= OnPostRequestHandlerExecute;
		}

		#endregion
	}
}