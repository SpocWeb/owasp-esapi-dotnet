using System;
using System.Web;
using EsapiTest.Surrogates;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Runtime;
using Owasp.Esapi.Runtime.Actions;

namespace EsapiTest.Runtime.Actions
{
	public class TransferActionTest
	{
		[Test]
		public void TestInitialize()
		{
			Esapi.Reset();
			EsapiConfig.Reset();
		}

		[Test]
		public void Test_Execute()
		{
			var detector = Esapi.IntrusionDetector;

			var url = Guid.NewGuid().ToString();
			var action = new TransferAction(url);

			// Set context
			MockHttpContext.InitializeCurrentContext();
			var page = new SurrogateWebPage();
			HttpContext.Current.Handler = page;

			// Block
			try
			{
				Assert.AreNotEqual(HttpContext.Current.Request.RawUrl, action.Url);
				action.Execute(ActionArgs.Empty);

				Assert.Fail("Request not terminated");
			}
			catch (Exception exp)
			{
				// FIXME : so far there is no other way to test the transfer except to check 
				// the stack of the exception. Ideally we should be able to mock the request
				// transfer itself
				Assert.IsTrue(exp.StackTrace.Contains("at System.Web.HttpServerUtility.TransferRequest(String path)"));
			}
		}

		[Test]
		public void Test_Create()
		{
			var url = Guid.NewGuid().ToString();

			var action = new TransferAction(url);
			Assert.AreEqual(action.Url, url);
		}

		[Test]
		public void Test_InvalidCreate()
		{
			try
			{
				new TransferAction(null);
				Assert.Fail("Null arg");
			}
			catch (ArgumentException)
			{
			}

			try
			{
				new TransferAction(string.Empty);
				Assert.Fail("Empty arg");
			}
			catch (ArgumentException)
			{
			}
		}
	}
}