using System.IO;
using System.Web;

namespace Owasp.Esapi.Swingset
{
	public class FileUtil
	{
		public static string RetrieveFileBody(string FileName)
		{
			var exactFileName = string.Format("~/App_Data/{0}", Path.GetFileName(FileName));
			return File.ReadAllText(HttpContext.Current.Server.MapPath(exactFileName));
		}
	}
}