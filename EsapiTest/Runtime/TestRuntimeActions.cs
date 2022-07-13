using System;
using NUnit.Framework;
using Owasp.Esapi.Runtime;
using Rhino.Mocks;

namespace EsapiTest.Runtime
{
	/// <summary>
	///     Summary description for TestRuntime
	/// </summary>
	public class TestRuntimeActions
	{
		MockRepository _mocks;
		EsapiRuntime _runtime;

		[Test]
		public void Initialize()
		{
			_mocks = new MockRepository();
			_runtime = new EsapiRuntime();
		}


		[Test]
		public void TestGetRuntime()
		{
			Assert.IsNotNull(_runtime);
		}

		[Test]
		public void TestFluentAddActions()
		{
			Assert.IsNotNull(_runtime);

			// Create and add actions
			var actions = ObjectRepositoryMock.MockNamedObjects<IAction>(_mocks, 10);

			ObjectRepositoryMock.AddNamedObjects(actions, _runtime.Actions);
			ObjectRepositoryMock.AssertContains(actions, _runtime.Actions);

			// Call actions
			ObjectRepositoryMock.ForEach(_runtime.Actions,
				delegate(IAction action) { Expect.Call(delegate { action.Execute(ActionArgs.Empty); }); });
			_mocks.ReplayAll();

			ObjectRepositoryMock.ForEach(_runtime.Actions,
				delegate(IAction action) { action.Execute(ActionArgs.Empty); });
			_mocks.VerifyAll();
		}

		[Test]
		public void TestFluentAddInvalidActionParams()
		{
			Assert.IsNotNull(_runtime);

			try
			{
				_runtime.Actions.Register(null, _mocks.StrictMock<IAction>());
				Assert.Fail("Null action name");
			}
			catch (ArgumentException)
			{
			}

			try
			{
				_runtime.Actions.Register(string.Empty, _mocks.StrictMock<IAction>());
				Assert.Fail("Empty action name");
			}
			catch (ArgumentException)
			{
			}

			try
			{
				_runtime.Actions.Register(Guid.NewGuid().ToString(), null);
				Assert.Fail("Null action");
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void TestRemoveAction()
		{
			Assert.IsNotNull(_runtime);

			ObjectRepositoryMock.AssertMockAddRemove(_mocks, _runtime.Actions);
		}
	}
}