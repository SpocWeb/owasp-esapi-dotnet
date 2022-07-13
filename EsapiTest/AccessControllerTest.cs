﻿using System;
using System.Security.Principal;
using System.Threading;
using EsapiTest.Surrogates;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Errors;
using Owasp.Esapi.Interfaces;
using Rhino.Mocks;

namespace EsapiTest
{
	/// <summary>
	///     Summary description for AccessControllerTest
	/// </summary>
	public class AccessControllerTest
	{
		void SetCurrentUser(string subject)
		{
			Thread.CurrentPrincipal = !string.IsNullOrEmpty(subject)
				? new GenericPrincipal(new GenericIdentity(subject), null)
				: null;
		}

		[Test]
		public void InitializeTests()
		{
			// Reset cached data
			Esapi.Reset();
			EsapiConfig.Reset();
			SetCurrentUser(null);
		}

		[Test]
		public void Test_AccessControllerAddRule()
		{
			var test = Guid.NewGuid().ToString();

			Esapi.AccessController.AddRule(test, test, test);
			Assert.IsTrue(Esapi.AccessController.IsAuthorized(test, test, test));
		}

		[Test]
		public void Test_AddDuplicateRule()
		{
			var test = Guid.NewGuid().ToString();

			Esapi.AccessController.AddRule(test, test, test);
			Assert.Throws<EnterpriseSecurityException>(() =>
				Esapi.AccessController.AddRule(test, test, test));
		}

		[Test]
		public void Test_AddRuleNullParams()
		{
			try
			{
				Esapi.AccessController.AddRule(null, string.Empty, string.Empty);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				Esapi.AccessController.AddRule(string.Empty, null, string.Empty);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				Esapi.AccessController.AddRule(string.Empty, string.Empty, null);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void Test_IsAuthorizedResource()
		{
			Guid action = Guid.NewGuid(), resource = Guid.NewGuid();
			var subject = Guid.NewGuid().ToString();

			SetCurrentUser(subject);

			// Allow action
			Esapi.AccessController.AddRule(subject, action, resource);

			// Verify current
			Assert.IsTrue(Esapi.AccessController.IsAuthorized(action, resource));

			Assert.IsFalse(Esapi.AccessController.IsAuthorized(action, Guid.NewGuid()));
		}

		[Test]
		public void Test_IsAuthorizedSubject()
		{
			Guid action = Guid.NewGuid(), resource = Guid.NewGuid(), subject = Guid.NewGuid();

			Esapi.AccessController.AddRule(subject, action, resource);
			Assert.IsTrue(Esapi.AccessController.IsAuthorized(subject, action, resource));

			Assert.IsFalse(Esapi.AccessController.IsAuthorized(Guid.NewGuid(), action, resource));
		}

		[Test]
		public void Test_IsAuthorizedNullParams()
		{
			try
			{
				Esapi.AccessController.IsAuthorized(null, string.Empty, string.Empty);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				Esapi.AccessController.IsAuthorized(string.Empty, null, string.Empty);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				Esapi.AccessController.IsAuthorized(string.Empty, string.Empty, null);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void Test_AccessControllerRemoveRule()
		{
			var test = Guid.NewGuid().ToString();

			Esapi.AccessController.AddRule(test, test, test);
			Esapi.AccessController.RemoveRule(test, test, test);
			Assert.IsFalse(Esapi.AccessController.IsAuthorized(test, test, test));
		}


		[Test]
		public void Test_RemoveRuleWrongSubject()
		{
			var test = Guid.NewGuid().ToString();

			Esapi.AccessController.AddRule(test, test, test);
			Assert.Throws<EnterpriseSecurityException>(() =>
				Esapi.AccessController.RemoveRule(string.Empty, test, test));
		}

		[Test]
		public void Test_RemoveRuleWrongAction()
		{
			var test = Guid.NewGuid().ToString();

			Esapi.AccessController.AddRule(test, test, test);
			Assert.Throws<EnterpriseSecurityException>(() =>
				Esapi.AccessController.RemoveRule(test, string.Empty, test));
		}

		[Test]
		public void Test_RemoveRuleWrongResource()
		{
			var test = Guid.NewGuid().ToString();

			Esapi.AccessController.AddRule(test, test, test);
			Assert.Throws<EnterpriseSecurityException>(() =>
				Esapi.AccessController.RemoveRule(test, test, string.Empty));
		}

		[Test]
		public void Test_RemoveRuleNullParams()
		{
			try
			{
				Esapi.AccessController.RemoveRule(null, string.Empty, string.Empty);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				Esapi.AccessController.RemoveRule(string.Empty, null, string.Empty);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				Esapi.AccessController.RemoveRule(string.Empty, string.Empty, null);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
		}

		[Test]
		public void Test_LoadCustom()
		{
			var mocks = new MockRepository();

			// Set new controller type
			EsapiConfig.Instance.AccessController.Type = typeof(SurrogateAccessController).AssemblyQualifiedName;

			// Get existing
			var accessController = Esapi.AccessController;
			Assert.IsTrue(accessController.GetType().Equals(typeof(SurrogateAccessController)));

			// Call some methods
			var mockController = mocks.StrictMock<IAccessController>();
			((SurrogateAccessController) accessController).Impl = mockController;

			Expect.Call(mockController.IsAuthorized(null, null)).Return(true);
			Expect.Call(mockController.IsAuthorized(null, null, null)).Return(false);
			mocks.ReplayAll();

			Assert.IsTrue(Esapi.AccessController.IsAuthorized(null, null));
			Assert.IsFalse(Esapi.AccessController.IsAuthorized(null, null, null));
			mocks.VerifyAll();
		}
	}
}