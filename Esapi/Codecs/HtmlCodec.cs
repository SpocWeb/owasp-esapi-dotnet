using System.Web;
using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi.Codecs
{
	/// <summary>
	///     This class performs HTML encoding. This is useful for encoding values that will be displayed in a browser
	///     as HTML  (i.e. &lt;b&gt; "untrusted data here" &lt;/b&gt;)
	/// </summary>
	/// <inheritdoc />
	[Codec(BuiltinCodecs.Html)]
	public class HtmlCodec : ICodec
	{
		public string Encode(string input)
		{
			return Microsoft.Security.Application.Encoder.HtmlEncode(input);
		}

		public string Decode(string input)
		{
			return HttpUtility.HtmlDecode(input);
		}
	}
}