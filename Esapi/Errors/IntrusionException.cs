using System;
using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.Errors
{
	/// <summary>
	///     An IntrusionException should be thrown anytime an error condition arises that is likely to be the result of an
	///     attack
	///     in progress. IntrusionExceptions are handled specially by the IntrusionDetector, which is equipped to respond by
	///     either specially logging the event, logging out the current user, or invalidating the current user's account.
	/// </summary>
	[Serializable]
	public class IntrusionException : Exception
	{
		/// <summary>The logger. </summary>
		static readonly ILogger logger;

		/// <summary>
		///     The message for the log
		/// </summary>
		string _logMessage;

		static IntrusionException()
		{
			logger = Esapi.Logger;
		}

		/// <summary>
		///     Internal classes may throw an IntrusionException to the IntrusionDetector, which generates the appropriate log
		///     message.
		/// </summary>
		public IntrusionException()
		{
		}

		/// <summary>
		///     Creates a new instance of IntrusionException.
		/// </summary>
		/// <param name="userMessage">
		///     The message for the user.
		/// </param>
		/// <param name="logMessage">
		///     The message for the log.
		/// </param>
		public IntrusionException(string userMessage, string logMessage)
			: base(userMessage)
		{
			_logMessage = logMessage;
			logger.Error(LogEventTypes.SECURITY, "INTRUSION - " + logMessage);
		}

		/// <summary>
		///     Instantiates a new intrusion exception.
		/// </summary>
		/// <param name="userMessage">
		///     The message for the user.
		/// </param>
		/// <param name="logMessage">
		///     The message for the log.
		/// </param>
		/// <param name="cause">
		///     The cause of the exception.
		/// </param>
		public IntrusionException(string userMessage, string logMessage, Exception cause)
			: base(userMessage, cause)
		{
			_logMessage = logMessage;
			logger.Error(LogEventTypes.SECURITY, "INTRUSION - " + logMessage, cause);
		}

		/// <summary>
		///     The message for the user
		/// </summary>
		public virtual string UserMessage => Message;

		/// <summary>
		///     The message for the log
		/// </summary>
		public virtual string LogMessage => _logMessage;
	}
}