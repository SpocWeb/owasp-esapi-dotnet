using System;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     ESAPI action arguments
	/// </summary>
	[Serializable]
	public class ActionArgs
	{
		/// <summary>
		///     Emtpy action arguments
		/// </summary>
		public static readonly ActionArgs Empty = new ActionArgs();

		Exception _faultException;
		IRule _faultingRule;

		RuntimeEventArgs _runtimeEventArgs;

		/// <summary>
		///     Faulting rule
		/// </summary>
		public IRule FaultingRule
		{
			get => _faultingRule;
			internal set => _faultingRule = value;
		}

		/// <summary>
		///     Fault exception
		/// </summary>
		public Exception FaultException
		{
			get => _faultException;
			internal set => _faultException = value;
		}

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