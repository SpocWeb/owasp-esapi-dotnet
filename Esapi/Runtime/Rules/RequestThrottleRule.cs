using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.SessionState;
using Owasp.Esapi.Errors;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi.Runtime.Rules
{
	/// <summary>
	///     Throttle requests rule
	/// </summary>
	public class RequestThrottleRule : IRule
	{
		const string SessionKey = "Owasp.Esapi.IntrusionDetection.Rules.RequestThrottleRule";

		int _maxCount;
		TimeSpan _timespan;

		/// <summary>
		///     Initialize request throttle rule
		/// </summary>
		public RequestThrottleRule()
		{
			_maxCount = 5;
			_timespan = new TimeSpan(0, 0, 10);
		}

		/// <summary>
		///     Initialize request throttle rule
		/// </summary>
		/// <param name="maxCount">Maximum hit count</param>
		/// <param name="timeSpan">Time interval (in seconds)</param>
		public RequestThrottleRule(int maxCount, int timeSpan)
		{
			if (maxCount <= 0) throw new ArgumentOutOfRangeException("maxCount");
			if (timeSpan <= 0) throw new ArgumentOutOfRangeException("timeSpan");
		}

		/// <summary>
		///     Maximum hit count
		/// </summary>
		public int MaxCount
		{
			get => _maxCount;
			set
			{
				if (value < 0) throw new ArgumentException();
				_maxCount = value;
			}
		}

		/// <summary>
		///     Time interval
		/// </summary>
		public int TimeSpan
		{
			get => _timespan.Seconds;
			set
			{
				if (value <= 0) throw new ArgumentException();
				_timespan = new TimeSpan(0, 0, value);
			}
		}

		/// <summary>
		///     Get request history
		/// </summary>
		/// <returns>Request history</returns>
		List<DateTime> GetRequestHistory(HttpSessionState session)
		{
			Debug.Assert(session != null);

			var history = session[SessionKey] as List<DateTime>;
			if (history == null)
			{
				history = new List<DateTime>();
				session[SessionKey] = history;
			}

			return history;
		}

		/// <summary>
		///     Verify request rate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPreRequestHandlerExecute(object sender, RuntimeEventArgs e)
		{
			var session = HttpContext.Current != null ? HttpContext.Current.Session : null;

			// No session initialized yet
			if (session == null) return;
			// Get current and history requests
			var requestHistory = GetRequestHistory(session);
			Debug.Assert(requestHistory != null);

			var currentTimestamp = DateTime.Now;

			// Lookup first in timespan
			var pos = -1;
			for (var i = 0; i < requestHistory.Count; ++i)
			{
				var hit = requestHistory[i];
				if (currentTimestamp - hit <= _timespan)
				{
					pos = i;
					break;
				}
			}

			// Add current
			requestHistory.Add(currentTimestamp);

			// Check & cleanup
			if (pos != -1)
			{
				// Remove expired records
				for (var i = 0; i < pos; ++i) requestHistory.RemoveAt(0);
				// Check interval
				if (requestHistory.Count >= _maxCount)
					throw new IntrusionException(EM.RequestThrottleRule_MaximumExceeded,
						EM.RequestThrottleRule_MaximumExceeded);
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