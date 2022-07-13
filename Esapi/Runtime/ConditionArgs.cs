using System;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     ESAPI condition arguments
	/// </summary>
	[Serializable]
	public class ConditionArgs
	{
		/// <summary>
		///     Empty condition arguments
		/// </summary>
		public static readonly ConditionArgs Emtpy = new ConditionArgs();

		RuntimeEventArgs _runtimeEventArgs;

		/// <summary>
		///     Runtime event arguments
		/// </summary>
		public RuntimeEventArgs RuntimeArgs
		{
			get => _runtimeEventArgs;
			internal set => _runtimeEventArgs = value;
		}
	}
}