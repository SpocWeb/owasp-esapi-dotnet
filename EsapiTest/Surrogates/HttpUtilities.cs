﻿using System.Collections.Generic;
using System.Web;
using Owasp.Esapi.Interfaces;

namespace EsapiTest.Surrogates
{
	// Forward http utilities
	internal class SurrogateHttpUtilities : IHttpUtilities
	{
		public IHttpUtilities Impl { get; set; }

		#region IHttpUtilities Members

		public void AddCsrfToken()
		{
			Impl.AddCsrfToken();
		}

		public string AddCsrfToken(string href)
		{
			return Impl.AddCsrfToken(href);
		}

		public void VerifyCsrfToken()
		{
			Impl.VerifyCsrfToken();
		}

		public void AddNoCacheHeaders()
		{
			Impl.AddNoCacheHeaders();
		}

		public void ChangeSessionIdentifier()
		{
			Impl.ChangeSessionIdentifier();
		}

		public void LogHttpRequest(HttpRequest request, ILogger logger, ICollection<string> obfuscatedParams)
		{
			Impl.LogHttpRequest(request, logger, obfuscatedParams);
		}

		public void AssertSecureRequest(HttpRequest request)
		{
			Impl.AssertSecureRequest(request);
		}

		#endregion
	}
}