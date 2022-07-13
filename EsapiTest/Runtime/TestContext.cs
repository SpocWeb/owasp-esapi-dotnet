using System;
using NUnit.Framework;
using Owasp.Esapi.Runtime;
using Rhino.Mocks;

namespace EsapiTest.Runtime
{
	internal delegate void RuntimeSubscribe(IRuntimeEventPublisher pub);

	internal class RuntimeEventSource : IRuntimeEventPublisher
	{
		public void FirePreRequestHandlerExecute()
		{
			if (PreRequestHandlerExecute != null) PreRequestHandlerExecute(this, new RuntimeEventArgs());
		}

		public void FirePostRequestHandlerExecute()
		{
			if (PostRequestHandlerExecute != null) PostRequestHandlerExecute(this, new RuntimeEventArgs());
		}

		#region IRuntimeEventPublisher Members

		public event EventHandler<RuntimeEventArgs> PreRequestHandlerExecute;
		public event EventHandler<RuntimeEventArgs> PostRequestHandlerExecute;

		#endregion
	}

	/// <summary>
	///     Summary description for TestContext
	/// </summary>
	public class TestContext
	{
		readonly string AID = Guid.NewGuid().ToString();

		readonly string CID = Guid.NewGuid().ToString();
		readonly string RID = Guid.NewGuid().ToString();
		MockRepository _mocks;
		EsapiRuntime _runtime;

		[Test]
		public void Initialize()
		{
			_mocks = new MockRepository();
			_runtime = new EsapiRuntime();

			InitializeRuntime();
		}

		[TearDown]
		public void TearDown()
		{
		}

		public void InitializeRuntime()
		{
			Assert.IsNotNull(_runtime);

			_runtime.Conditions.Register(CID, _mocks.StrictMock<ICondition>());
			_runtime.Actions.Register(AID, _mocks.StrictMock<IAction>());
			_runtime.Rules.Register(RID, _mocks.StrictMock<IRule>());
		}

		[Test]
		public void TestAddDuplicateContext()
		{
			var contextId = Guid.NewGuid().ToString();

			Assert.IsNotNull(_runtime);

			_runtime.CreateContext(contextId);
			Assert.Throws<ArgumentException>(() =>
				_runtime.CreateContext(contextId));
		}

		[Test]
		public void TestContextFailInit()
		{
			try
			{
				new Context(null);
				Assert.Fail("Null id");
			}
			catch (ArgumentException)
			{
			}

			try
			{
				new Context(string.Empty);
				Assert.Fail("Empty id");
			}
			catch (ArgumentException)
			{
			}
		}

		[Test]
		public void TestAddDuplicateSubContext()
		{
			var contextId = Guid.NewGuid().ToString();
			var subcontextId = Guid.NewGuid().ToString();

			Assert.IsNotNull(_runtime);

			var parent = _runtime.CreateContext(contextId);
			parent.CreateSubContext(subcontextId);
			Assert.Throws<ArgumentNullException>(() =>
				parent.CreateSubContext(subcontextId));
		}

		[Test]
		public void TestContextMatchTrueCondition()
		{
			Assert.IsNotNull(_runtime);

			var contextId = Guid.NewGuid().ToString();
			var eventId = Guid.NewGuid().ToString();

			// Setup conditions
			var context = _runtime.CreateContext(contextId);
			context.BindCondition(_runtime.Conditions.Get(CID), true);
			Assert.AreEqual(context.MatchConditions.Count, 1);

			// Setup rule
			var rule = _runtime.Rules.Get(RID);
			Expect.Call(delegate { rule.Subscribe(null); }).IgnoreArguments()
				.Do((RuntimeSubscribe)
					// Register to throw exceptions for each published event
					delegate(IRuntimeEventPublisher pub)
					{
						pub.PreRequestHandlerExecute += delegate { throw new InvalidOperationException(); };
						pub.PostRequestHandlerExecute += delegate { throw new AccessViolationException(); };
					});

			// Set expectations for prerequest
			Expect.Call(_runtime.Conditions.Get(CID).Evaluate(null))
				.IgnoreArguments().Return(true);
			Expect.Call(delegate { _runtime.Actions.Get(AID).Execute(null); }).IgnoreArguments();
			// Set expectations for postrequest
			Expect.Call(_runtime.Conditions.Get(CID).Evaluate(null))
				.IgnoreArguments().Return(true);
			Expect.Call(delegate { _runtime.Actions.Get(AID).Execute(null); }).IgnoreArguments();
			_mocks.ReplayAll();


			// Verify            
			context.BindRule(rule).FaultActions.Add(_runtime.Actions.Get(AID));
			Assert.AreEqual(context.ExecuteRules.Count, 1);

			// Verify event handlers
			var source = new RuntimeEventSource();
			_runtime.Subscribe(source);
			source.FirePreRequestHandlerExecute();
			source.FirePostRequestHandlerExecute();

			_mocks.VerifyAll();
		}
	}
}