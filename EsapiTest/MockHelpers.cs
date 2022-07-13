using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;

namespace EsapiTest
{
	/// <remarks>
	///     From http://www.jasonbock.net/JB/Default.aspx?blog=entry.161daabc728842aca6f329d87c81cfcb
	/// </remarks>
	public sealed class MockHttpContext
	{
		const string ThreadDataKeyAppPath = ".appPath";
		const string ThreadDataKeyAppPathValue = "c:\\inetpub\\wwwroot\\webapp\\";
		const string ThreadDataKeyAppVPath = ".appVPath";
		const string ThreadDataKeyAppVPathValue = "/webapp";

		public MockHttpContext()
			: this("default.aspx", string.Empty)
		{
		}

		public MockHttpContext(string page, string query)
		{
			Thread.GetDomain().SetData(ThreadDataKeyAppPath, ThreadDataKeyAppPathValue);
			Thread.GetDomain().SetData(ThreadDataKeyAppVPath, ThreadDataKeyAppVPathValue);

			var request = new SimpleWorkerRequest(page, query, new StringWriter());
			Context = new HttpContext(request);

			var container = new HttpSessionStateContainer(Guid.NewGuid().ToString("N"),
				new SessionStateItemCollection(),
				new HttpStaticObjectsCollection(), 5, true, HttpCookieMode.AutoDetect, SessionStateMode.InProc,
				false);

			var state = Activator.CreateInstance(typeof(HttpSessionState),
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
				null, new object[] {container}, CultureInfo.CurrentCulture) as HttpSessionState;
			Context.Items["AspSession"] = state;
		}

		public HttpContext Context { get; }

		/// <summary>
		///     Set a mock context as HttpContext.Current
		/// </summary>
		public static void InitializeCurrentContext()
		{
			CallContext.HostContext = new MockHttpContext().Context;
		}

		/// <summary>
		///     Set current http context
		/// </summary>
		/// <param name="context"></param>
		public static void SetCurrentContext(MockHttpContext context)
		{
			if (context == null) throw new ArgumentNullException("request");
			CallContext.HostContext = context.Context;
		}
	}
}