using System;
using NUnit.Framework;
using Owasp.Esapi.Runtime;
using Rhino.Mocks;

namespace EsapiTest.Runtime
{
	/// <summary>
	/// </summary>
	public class TestRuntimeConditions
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
		public void TestFluentAddConditions()
		{
			Assert.IsNotNull(_runtime);

			// Create and add conditions
			var conditions = ObjectRepositoryMock.MockNamedObjects<ICondition>(_mocks, 10);

			ObjectRepositoryMock.AddNamedObjects(conditions, _runtime.Conditions);
			ObjectRepositoryMock.AssertContains(conditions, _runtime.Conditions);

			// Call conditions
			ObjectRepositoryMock.ForEach(_runtime.Conditions,
				delegate(ICondition condition) { Expect.Call(condition.Evaluate(ConditionArgs.Emtpy)).Return(false); });
			_mocks.ReplayAll();

			ObjectRepositoryMock.ForEach(_runtime.Conditions,
				delegate(ICondition condition) { Assert.IsFalse(condition.Evaluate(ConditionArgs.Emtpy)); });
			_mocks.VerifyAll();
		}

		[Test]
		public void TestFluentAddInvalidConditionParams()
		{
			Assert.IsNotNull(_runtime);

			try
			{
				_runtime.Conditions.Register(null, _mocks.StrictMock<ICondition>());
				Assert.Fail("Null condition name");
			}
			catch (ArgumentException)
			{
			}

			try
			{
				_runtime.Conditions.Register(string.Empty, _mocks.StrictMock<ICondition>());
				Assert.Fail("Empty condition name");
			}
			catch (ArgumentException)
			{
			}

			try
			{
				_runtime.Conditions.Register(Guid.NewGuid().ToString(), null);
				Assert.Fail("Null condition");
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void TestRemoveCondition()
		{
			Assert.IsNotNull(_runtime);

			ObjectRepositoryMock.AssertMockAddRemove(_mocks, _runtime.Conditions);
		}
	}
}