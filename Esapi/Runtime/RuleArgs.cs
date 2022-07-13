using System;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     ESAPI ruule arguments
	/// </summary>
	[Serializable]
	public class RuleArgs
	{
		/// <summary>
		///     Empty rule arguments
		/// </summary>
		public static readonly RuleArgs Empty;

		static RuleArgs()
		{
			Empty = new RuleArgs(string.Empty);
		}

		/// <summary>
		///     Initialize rule arguments
		/// </summary>
		/// <param name="eventName"></param>
		public RuleArgs(string eventName)
		{
			Event = eventName;
		}

		/// <summary>
		///     Get event name
		/// </summary>
		public string Event { get; }
	}
}