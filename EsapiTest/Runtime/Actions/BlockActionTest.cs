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
	/// <summary>
	///     Summary description for BlockActionTest
	/// </summary>
	public class BlockActionTest
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
			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			// Should be loaded by default
			var action = new BlockAction();

			// Set context
			MockHttpContext.InitializeCurrentContext();
			var page = new SurrogateWebPage();
			HttpContext.Current.Handler = page;

			// Block
			Assert.AreNotEqual(HttpContext.Current.Response.StatusCode, action.StatusCode);

			action.Execute(ActionArgs.Empty);
			Assert.AreEqual(HttpContext.Current.Response.StatusCode, action.StatusCode);
		}

		[Test]
		public void Test_SetStatusCode()
		{
			var statusCode = new Random((int) DateTime.Now.Ticks).Next();

			var action = new BlockAction();

			Assert.AreNotEqual(action.StatusCode, statusCode);
			action.StatusCode = statusCode;
			Assert.AreEqual(action.StatusCode, statusCode);
		}
	}
}