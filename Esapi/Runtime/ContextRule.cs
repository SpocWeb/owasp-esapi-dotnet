using System;
using System.Collections.Generic;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     Context rule implementation
	/// </summary>
	internal class ContextRule : RuntimeEventBridge, IContextRule, IDisposable
	{
		readonly List<IAction> _faultActions;

		/// <summary>
		///     Initialize context rule
		/// </summary>
		/// <param name="rule">
		///     A <see cref="IRule" />
		/// </param>
		internal ContextRule(IRule rule)
		{
			if (rule == null) throw new ArgumentNullException();
			Rule = rule;
			_faultActions = new List<IAction>();

			// Subscribe rule to events
			Rule.Subscribe(this);
		}

		#region IDisposable implementation

		public void Dispose()
		{
			Rule.Unsubscribe(this);
		}

		#endregion

		/// <summary>
		///     Unsubscribe from publisher's events
		/// </summary>
		/// <param name="publisher">
		///     A <see cref="IRuntimeEventPublisher" />
		/// </param>
		public override void Unsubscribe(IRuntimeEventPublisher publisher)
		{
			base.Unsubscribe(publisher);
			Rule.Unsubscribe(this);
		}

		/// <summary>
		///     Handle rule execution fault
		/// </summary>
		/// <param name="handler">
		///     A <see cref="EventHandler<RuntimeEventArgs>"/>
		/// </param>
		/// <param name="sender">
		///     A <see cref="System.Object" />
		/// </param>
		/// <param name="args">
		///     A <see cref="RuntimeEventArgs" />
		/// </param>
		/// <param name="exp">
		///     A <see cref="Exception" />
		/// </param>
		/// <returns>
		///     A <see cref="System.Boolean" />
		/// </returns>
		protected override bool ForwardEventFault(EventHandler<RuntimeEventArgs> handler, object sender,
			RuntimeEventArgs args, Exception exp)
		{
			// Init action args
			var actionArgs = new ActionArgs
			{
				FaultingRule = Rule,
				FaultException = exp,
				RuntimeArgs = args
			};

			// Run each action
			foreach (var action in _faultActions) action.Execute(actionArgs);

			return true;
		}


		#region IContextRule implementation

		public IRule Rule { get; }

		public ICollection<IAction> FaultActions => _faultActions;

		#endregion
	}
}