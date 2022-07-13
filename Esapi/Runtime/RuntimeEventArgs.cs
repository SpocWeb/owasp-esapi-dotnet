using System;
using System.Collections.Generic;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     Runtime event arguments
	/// </summary>
	public class RuntimeEventArgs : EventArgs
	{
		/// <summary>
		///     Context stack
		/// </summary>
		readonly Stack<IContext> _contexts;

		/// <summary>
		///     Initialize runtime arguments
		/// </summary>
		public RuntimeEventArgs()
		{
			_contexts = new Stack<IContext>();
			MatchCache = new EvaluationCache<IContext, bool>();
			EvalCache = new EvaluationCache<ICondition, bool>();
		}

		/// <summary>
		///     Get context path
		/// </summary>
		public IEnumerable<IContext> ContextPath => _contexts;

		/// <summary>
		///     Get current context
		/// </summary>
		public IContext CurrentContext => _contexts.Count > 0 ? _contexts.Peek() : null;

		#region Internal

		/// <summary>
		///     Push context
		/// </summary>
		/// <param name="context">
		///     A <see cref="IContext" />
		/// </param>
		internal void PushContext(IContext context)
		{
			if (context == null) throw new ArgumentNullException();
			_contexts.Push(context);
		}

		/// <summary>
		///     Pop context
		/// </summary>
		/// <returns>
		///     A <see cref="IContext" />
		/// </returns>
		internal IContext PopContext()
		{
			return _contexts.Pop();
		}

		/// <summary>
		///     Context match cache
		/// </summary>
		internal EvaluationCache<IContext, bool> MatchCache { get; }

		/// <summary>
		///     Condition evaluation cache
		/// </summary>
		internal EvaluationCache<ICondition, bool> EvalCache { get; }

		#endregion
	}
}