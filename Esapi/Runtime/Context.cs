using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     Context implementation
	/// </summary>
	internal class Context : RuntimeEventBridge, IContext, IDisposable
	{
		readonly List<IContextCondition> _conditions;
		readonly List<IContextRule> _rules;
		readonly NamedObjectRepository<IContext> _subcontexts;

		internal Context(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentException();

			Name = name;

			_conditions = new List<IContextCondition>();
			_rules = new List<IContextRule>();
			_subcontexts = new NamedObjectRepository<IContext>();
		}

		#region IDisposable implementation

		public void Dispose()
		{
			foreach (ContextRule rule in _rules) rule.Dispose();
			foreach (var subcontext in _subcontexts.Objects)
			{
				var disp = subcontext as IDisposable;
				if (disp != null) disp.Dispose();
			}
		}

		#endregion

		public override void Unsubscribe(IRuntimeEventPublisher publisher)
		{
			base.Unsubscribe(publisher);

			foreach (ContextRule rule in _rules) rule.Unsubscribe(this);
			foreach (var subcontext in _subcontexts.Objects)
			{
				var rteListener = subcontext as IRuntimeEventListener;
				if (rteListener != null) rteListener.Unsubscribe(this);
			}
		}

		/// <summary>
		///     Check if the context is matched
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		bool IsMatch(RuntimeEventArgs args)
		{
			var isMatch = true;

			// Check context match cache first
			if (args.MatchCache.TryGetValue(this, out isMatch)) return isMatch;

			// Initialize condition arguments
			isMatch = true;
			var conditionArgs = new ConditionArgs {RuntimeArgs = args};

			// Evaluate each condition
			foreach (var contextCondition in _conditions)
			{
				var result = true;

				// Check condition eval cache first
				if (!args.EvalCache.TryGetValue(contextCondition.Condition, out result))
				{
					// Eval
					result = contextCondition.Condition.Evaluate(conditionArgs) == contextCondition.Result;
					args.EvalCache.SetValue(contextCondition.Condition, result);
				}

				// Shortcut match if false
				if (!result)
				{
					isMatch = false;
					break;
				}
			}

			// Cache
			args.MatchCache.SetValue(this, isMatch);

			// Return
			return isMatch;
		}

		#region EventBridge

		protected override bool ForwardEventFault(EventHandler<RuntimeEventArgs> handler, object sender,
			RuntimeEventArgs args, Exception exp)
		{
			var top = args.PopContext();
			Debug.Assert(top == this);

			return base.ForwardEventFault(handler, sender, args, exp);
		}


		protected override bool AfterForwardEvent(EventHandler<RuntimeEventArgs> handler, object sender,
			RuntimeEventArgs args)
		{
			var top = args.PopContext();
			Debug.Assert(top == this);

			return base.AfterForwardEvent(handler, sender, args);
		}


		protected override bool BeforeForwardEvent(EventHandler<RuntimeEventArgs> handler, object sender,
			RuntimeEventArgs args)
		{
			var shouldContinue = !IsMatch(args);
			if (!shouldContinue) args.PushContext(this);
			return shouldContinue;
		}

		#endregion EventBridge

		#region IContext implementation

		/// <summary>
		///     Get context name
		/// </summary>
		public string Name { get; }

		/// <summary>
		///     Get context conditions
		/// </summary>
		public ICollection<IContextCondition> MatchConditions => _conditions;

		/// <summary>
		///     Get context rules
		/// </summary>
		public ICollection<IContextRule> ExecuteRules => _rules;

		public ICollection<IContext> SubContexts => _subcontexts.Objects;

		/// <summary>
		///     Add condition
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="value">Value to match</param>
		/// <returns></returns>
		public IContextCondition BindCondition(ICondition condition, bool value)
		{
			var contextCondition = new ContextCondition(condition, value);
			_conditions.Add(contextCondition);

			return contextCondition;
		}

		/// <summary>
		///     Bind rule
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		public IContextRule BindRule(IRule rule)
		{
			var contextRule = new ContextRule(rule);
			contextRule.Subscribe(this);

			_rules.Add(contextRule);
			return contextRule;
		}

		/// <summary>
		///     Register context
		/// </summary>
		/// <returns>New subcontext</returns>
		/// <remarks>Context name is automatically generated</remarks>
		public IContext CreateSubContext()
		{
			return CreateSubContext(Guid.NewGuid().ToString());
		}

		/// <summary>
		///     Register named context
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IContext CreateSubContext(string name)
		{
			IContext prevContext;
			if (_subcontexts.Lookup(name, out prevContext)) throw new ArgumentException();

			var context = new Context(name);
			context.Subscribe(this);

			_subcontexts.Register(name, context);
			return context;
		}

		/// <summary>
		///     Lookup subcontext by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IContext LookupSubContext(string name)
		{
			IContext context;
			_subcontexts.Lookup(name, out context);
			return context;
		}

		/// <summary>
		///     Register subcontext
		/// </summary>
		/// <param name="name"></param>
		/// <param name="context"></param>
		public void RegisterSubContext(string name, IContext context)
		{
			_subcontexts.Register(name, context);
		}

		#endregion
	}
}