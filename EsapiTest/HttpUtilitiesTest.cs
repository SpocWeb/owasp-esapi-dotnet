using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using EsapiTest.Surrogates;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Errors;

namespace EsapiTest
{
	/// <summary>
	/// </summary>
	public class HttpUtilitiesTest
	{
		[Test]
		public void TestInitialize()
		{
			Esapi.Reset();
			EsapiConfig.Reset();
		}

		[Test]
		public void Test_AddCsrfToken()
		{
			MockHttpContext.InitializeCurrentContext();

			var page = new SurrogateWebPage();
			HttpContext.Current.Handler = page;

			Esapi.HttpUtilities.AddCsrfToken();
			Assert.AreEqual(page.ViewStateUserKey, HttpContext.Current.Session.SessionID);
		}

		[Test]
		public void Test_AddCsrfTokenHref()
		{
			MockHttpContext.InitializeCurrentContext();

			var href = "http://localhost/somepage.aspx";

			var csrfUri = new Uri(Esapi.HttpUtilities.AddCsrfToken(href));
			Assert.IsTrue(csrfUri.Query.Contains(HttpUtilities.CSRF_TOKEN_NAME));
		}

		[Test]
		public void Test_LoadCustom()
		{
			EsapiConfig.Instance.HttpUtilities.Type = typeof(SurrogateHttpUtilities).AssemblyQualifiedName;

			var utilities = Esapi.HttpUtilities;
			Assert.AreEqual(utilities.GetType(), typeof(SurrogateHttpUtilities));
		}

		[Test]
		public void Test_LogHttpRequest()
		{
			// Force log initialization
			var logger = new Logger(typeof(HttpUtilitiesTest).ToString());

			// Reset current configuration
			LogManager.ResetConfiguration();

			// Redirect log output to strinb guilder
			var sb = new StringBuilder();
			var appender = new TextWriterAppender();
			appender.Writer = new StringWriter(sb);
			appender.Threshold = Level.Debug;
			appender.Layout = new PatternLayout();
			appender.ActivateOptions();
			BasicConfigurator.Configure(appender);

			// Initialize current request
			var userIdentity = Guid.NewGuid().ToString();
			MockHttpContext.InitializeCurrentContext();
			HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(userIdentity), null);

			// Log and test
			Esapi.HttpUtilities.LogHttpRequest(HttpContext.Current.Request, Esapi.Logger, null);
			Assert.IsFalse(string.IsNullOrEmpty(sb.ToString()));
			Assert.IsTrue(sb.ToString().Contains(userIdentity));
		}

		[Test]
		public void Test_LogNullHttpRequest()
		{
			MockHttpContext.InitializeCurrentContext();
			Assert.Throws<ArgumentNullException>(() =>
				Esapi.HttpUtilities.LogHttpRequest(null, Esapi.Logger, null));
		}

		[Test]
		public void Test_LogHttpRequestNullLogger()
		{
			MockHttpContext.InitializeCurrentContext();
			Assert.Throws<ArgumentNullException>(() =>
				Esapi.HttpUtilities.LogHttpRequest(HttpContext.Current.Request, null, null));
		}

		[Test]
		public void Test_SecureRequest()
		{
			MockHttpContext.InitializeCurrentContext();
			Assert.Throws<AccessControlException>(() =>
				Esapi.HttpUtilities.AssertSecureRequest(HttpContext.Current.Request));
		}

		[Test]
		public void Test_NullSecureRequest()
		{
			Assert.Throws<ArgumentNullException>(() =>
				Esapi.HttpUtilities.AssertSecureRequest(null));
		}

		//[Test]
		//public void Test_AddNoCacheHeaders()
		//{
		//    MockHttpContext.InitializeCurrentContext();

		//    Esapi.HttpUtilities.AddNoCacheHeaders();

		//    Assert.IsNotNull(HttpContext.Current.Response.Headers.Get("Cache-Control"));
		//    Assert.IsNotNull(HttpContext.Current.Response.Headers.Get("Pragma"));
		//    Assert.AreEqual(HttpContext.Current.Response.Expires, -1);
		//}
	}
}