using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Owasp.Esapi.Runtime.Conditions;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     ESAPI runtime module has to be re-designed:
	///     https://docs.microsoft.com/en-us/aspnet/core/migration/http-modules?view=aspnetcore-6.0
	/// </summary>
	public class EsapiRuntimeModule : IHttpModule, IRuntimeEventPublisher
	{
		readonly object _handlersLock;
		readonly HashSet<Type> _handlerTypes;
		readonly EsapiRuntime _runtime;

		public EsapiRuntimeModule()
		{
			_runtime = new EsapiRuntime();

			_handlersLock = new object();
			_handlerTypes = new HashSet<Type>();
		}

		/// <summary>
		///     Map request handler to context
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPostMapRequestHandler(object sender, EventArgs e)
		{
			var context = ((HttpApplication) sender).Context;
			var handler = context.CurrentHandler;

			if (handler != null)
				lock (_handlersLock)
				{
					// Get code behind type
					var handlerType = handler.GetType();

					// If handler not known map to context
					if (!_handlerTypes.Contains(handlerType))
					{
						MapHandlerContext(handlerType);
						_handlerTypes.Add(handlerType);
					}
				}
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			if (PreRequestHandlerExecute != null) PreRequestHandlerExecute(this, new RuntimeEventArgs());
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnPostRequestHandlerExecute(object sender, EventArgs e)
		{
			if (PostRequestHandlerExecute != null) PostRequestHandlerExecute(this, new RuntimeEventArgs());
		}

		#region Public

		/// <summary>
		///     Get current module instance (if registered)
		/// </summary>
		public EsapiRuntimeModule Current
		{
			get
			{
				EsapiRuntimeModule instance = null;

				// Lookup module in the current running application
				var currentApp = HttpContext.Current != null ? HttpContext.Current.ApplicationInstance : null;
				if (currentApp != null)
				{
					var modules = currentApp.Modules;

					// Lookup first instance
					foreach (string key in modules.Keys)
						if (null != (instance = modules[key] as EsapiRuntimeModule))
							break;
				}

				return instance;
			}
		}

		/// <summary>
		///     Get runtime instance
		/// </summary>
		public IRuntime RuntimeInstance => _runtime;

		#endregion

		#region Context mapping

		/// <summary>
		///     Mapp application to context
		/// </summary>
		/// <param name="applicationType"></param>
		void MapApplicationContext(Type applicationType)
		{
			Debug.Assert(applicationType != null);

			var runRules = applicationType.GetCustomAttributes(typeof(RunRuleAttribute), true);
			if (runRules != null && runRules.Length > 0)
			{
				// Create new context
				var appContext = _runtime.CreateContext();
				appContext.BindCondition(new ValueBoundCondition(true), true);

				// Add rules to context
				foreach (RunRuleAttribute runRule in runRules)
				{
					var rule = appContext.BindRule(ObjectBuilder.Build<IRule>(runRule.Rule));

					// Add actions
					if (runRule.FaultActions != null && runRule.FaultActions.Length > 0)
						foreach (var action in runRule.FaultActions)
							rule.FaultActions.Add(ObjectBuilder.Build<IAction>(action));
				}
			}
		}

		/// <summary>
		///     Map handler to context
		/// </summary>
		/// <param name="handlerTpe"></param>
		void MapHandlerContext(Type handlerType)
		{
			Debug.Assert(handlerType != null);

			var runRules = handlerType.GetCustomAttributes(typeof(RunRuleAttribute), true);
			if (runRules != null && runRules.Length > 0)
			{
				// Create new context
				var handlerContext = _runtime.CreateContext();
				handlerContext.BindCondition(new HandlerCondition(handlerType), true);

				// Add rules to context
				foreach (RunRuleAttribute runRule in runRules)
				{
					var rule = handlerContext.BindRule(ObjectBuilder.Build<IRule>(runRule.Rule));

					// Add actions
					if (runRule.FaultActions != null && runRule.FaultActions.Length > 0)
						foreach (var action in runRule.FaultActions)
							rule.FaultActions.Add(ObjectBuilder.Build<IAction>(action));
				}
			}
		}

		#endregion

		#region IRuntimeEventPublisher Members

		public event EventHandler<RuntimeEventArgs> PreRequestHandlerExecute;
		public event EventHandler<RuntimeEventArgs> PostRequestHandlerExecute;

		#endregion

		#region IHttpModule Members

		/// <summary>
		///     Release runtime resources
		/// </summary>
		public void Dispose()
		{
			// Disconnect runtime
			_runtime.Unsubscribe(this);
		}

		/// <summary>
		///     Register for events
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
			context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
			context.PostMapRequestHandler += OnPostMapRequestHandler;

			// Connect runtime
			_runtime.Subscribe(this);

			// Map application context
			MapApplicationContext(context.GetType());
		}

		#endregion
	}
}