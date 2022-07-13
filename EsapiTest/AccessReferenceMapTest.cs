using System;
using System.Collections;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Errors;

namespace EsapiTest
{
	/// <summary>
	///     Summary description for AccessReferenceMapTest
	/// </summary>
	public class AccessReferenceMapTest
	{
		Account account1;
		Account account2;
		Account account3;

		ArrayList accounts;
		AccessReferenceMap arm;

		/// <summary>
		///     Gets or sets the test context which provides
		///     information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		/// <summary>
		///     Set up an access reference map
		/// </summary>
		[SetUp]
		public void MyTestInitialize()
		{
			account1 = new Account(1000, "test1");
			account2 = new Account(2000, "test2");
			account3 = new Account(3000, "test3");
			accounts = new ArrayList();
			accounts.Add(account1);
			accounts.Add(account2);
			accounts.Add(account3);
			arm = new AccessReferenceMap(accounts);
		}

		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//

		#endregion

		[Test]
		public void Test_Update()
		{
			Console.Out.WriteLine("Update");
			// test to make sure update returns something
			arm.Update(accounts);
			Assert.IsNotNull(arm.GetIndirectReference(account1));

			// test to make sure update removes items that are no longer in the list
			accounts.Remove(account3);
			arm.Update(accounts);
			var indirect = arm.GetIndirectReference(account3);
			Assert.IsNull(indirect);

			// test to make sure old indirect reference is maintained after an update
			arm.Update(accounts);
			var newIndirect = arm.GetIndirectReference(account3);
			Assert.AreEqual(indirect, newIndirect);
		}

		[Test]
		public void Test_UpdateNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				arm.Update(null));
		}


		/// <summary> Test of GetDirectReferences method, of class Owasp.Esapi.AccessReferenceMap.</summary>
		[Test]
		public void Test_GetDirectReferences()
		{
			Console.Out.WriteLine("GetDirectReferences");
			var enumerator = arm.GetDirectReferences().GetEnumerator();
			var index = 0;
			while (enumerator.MoveNext())
			{
				var account = (Account) enumerator.Current;
				Assert.IsTrue(accounts.Contains(account));
				index++;
			}
			Assert.AreEqual(accounts.Count, index);
		}


		/// <summary> Test of GetDirectRefrences method, of class Owasp.Esapi.AccessReferenceMap.</summary>
		[Test]
		public void Test_GetIndirectReferences()
		{
			Console.Out.WriteLine("GetIndirectreferences");
			var enumerator = arm.GetIndirectReferences().GetEnumerator();
			var index = 0;
			while (enumerator.MoveNext())
			{
				var indirectReference = (string) enumerator.Current;
				Assert.IsNotNull(arm.GetDirectReference(indirectReference));
				index++;
			}
			Assert.AreEqual(accounts.Count, index);
		}


		/// <summary>
		///     Test of getIndirectReference method, of class
		///     Owasp.Esapi.AccessReferenceMap.
		/// </summary>
		[Test]
		public void Test_GetIndirectReference()
		{
			Console.Out.WriteLine("GetIndirectReference");
			var indirect = arm.GetIndirectReference(account1);
			Assert.AreNotEqual(indirect, account1);
			Assert.AreEqual(arm.GetDirectReference(indirect), account1);
		}

		/// <summary>
		///     Test of getDirectReference method, of class
		///     Owasp.Esapi.AccessReferenceMap.
		/// </summary>
		/// <throws>  AccessControlException </throws>
		/// <summary>
		///     the access control exception
		/// </summary>
		public void Test_GetDirectReference()
		{
			Assert.Throws<AccessControlException>(() =>
				arm.GetDirectReference("invalid"));
		}

		[Test]
		public void Test_GetDirectReferenceNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				arm.GetDirectReference(null));
		}

		[Test]
		public void Test_GetIndirectReferenceNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				arm.GetIndirectReference(null));
		}

		[Test]
		public void Test_AddDirectReferenceNull()
		{
			arm.AddDirectReference(null);
		}

		[Test]
		public void Test_AddDirectReference()
		{
			var direct = Guid.NewGuid();

			var indirect = arm.AddDirectReference(direct);
			Assert.AreEqual(arm.GetDirectReference(indirect), direct);
		}

		[Test]
		public void Test_RemoveDirectReferenceNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				arm.RemoveDirectReference(null));
		}

		[Test]
		public void Test_RemoveDirectReference()
		{
			var direct = Guid.NewGuid();

			var indirect = arm.AddDirectReference(direct);
			Assert.AreEqual(direct, arm.GetDirectReference(indirect));

			Assert.AreEqual(indirect, arm.RemoveDirectReference(direct));
		}

		class Account
		{
			int Balance;
			string Name;

			public Account(int balance, string name)
			{
				Name = name;
				Balance = balance;
			}
		}
	}
}