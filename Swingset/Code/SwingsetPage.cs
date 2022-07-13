using System;
using System.Web.UI;
using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.Swingset
{
	public class SwingsetPage : Page
	{
		public ILogger logger = Esapi.Logger;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Esapi.HttpUtilities.AddCsrfToken();
			Esapi.HttpUtilities.AddNoCacheHeaders();
		}
	}
}