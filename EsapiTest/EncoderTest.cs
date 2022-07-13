using System;
using System.Collections.Generic;
using System.IO;
using EsapiTest.Surrogates;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Codecs;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Interfaces;
using Rhino.Mocks;
using Shouldly;
using Is = Rhino.Mocks.Constraints.Is;

namespace EsapiTest
{
	/// <summary> Summary description for EncoderTest </summary>
	public class EncoderTest
	{
		public static IEnumerable<TestCaseData> EncodeForHtmlTests = new List<TestCaseData>
		{
			new TestCaseData(null) {ExpectedResult = null},
			new TestCaseData("") {ExpectedResult = ""},
			new TestCaseData("a" + (char) 0 + "b" + (char) 4 + "c" + (char) 128 + "d" + (char) 150 + "e" + (char) 159 +
			                 "f" + (char) 9 + "g") {ExpectedResult = "a&#0;b&#4;c&#128;d&#150;e&#159;f&#9;g"},
			new TestCaseData("<script>") {ExpectedResult = "&lt;script&gt;"},
			new TestCaseData("&lt;script&gt;") {ExpectedResult = "&amp;lt;script&amp;gt&#59;"},
			new TestCaseData("!@$%()=+{}[]")
				{ExpectedResult = "&#33;&#64;&#36;&#37;&#40;&#41;&#61;&#43;&#123;&#125;&#91;&#93;"},
			// Assert.AreEqual("&#x21;&#x40;&#x24;&#x25;&#x28;&#x29;&#x3d;&#x2b;&#x7b;&#x7d;&#x5b;&#x5d;", encoder.Encode(BuiltinCodec.Html, encoder.Canonicalize("&#33;&#64;&#36;&#37;&#40;&#41;&#61;&#43;&#123;&#125;&#91;&#93;") ) );

			new TestCaseData(",.-_ ") {ExpectedResult = ",.-_ "},
			new TestCaseData("dir&") {ExpectedResult = "dir&amp;"},
			new TestCaseData("one&two") {ExpectedResult = "one&amp;two"}
		};

		[Test]
		public void InitializeTest()
		{
			Esapi.Reset();
			EsapiConfig.Reset();

			SurrogateEncoder.DefaultEncoder = null;
		}

		/// <summary>
		///     Test of Canonicalize method, of class Owasp.Esapi.Validator.
		/// </summary>
		/// <throws>  ValidationException </throws>
		[Test]
		public void Test_Canonicalize()
		{
		}

		/// <summary>
		///     Test of Normalize method, of class Owasp.Esapi.Validator.
		/// </summary>
		/// <throws>  ValidationException </throws>
		/// <summary>
		///     the validation exception
		/// </summary>
		[Test]
		public void Test_Normalize()
		{
			Console.Out.WriteLine("Normalize");
			// Assert.AreEqual(Esapi.Encoder.Normalize("é à î _ @ \" < > \u20A0"), "e a i _ @ \" < > ");
		}

		/// <summary> Test of EncodeForHtml method, of class Owasp.Esapi.Encoder.</summary>
		// test invalid characters are replaced with spaces
		[TestCaseSource(nameof(EncodeForHtmlTests))]
		public string Test_EncodeForHtml(string input)
		{
			return Esapi.Encoder.Encode(BuiltinCodecs.Html, input);
		}

		/// <summary> Test of EncodeForHtmlAttribute method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_EncodeForHtmlAttribute()
		{
			Console.Out.WriteLine("EncodeForHtmlAttribute");
			var encoder = Esapi.Encoder;
			Assert.AreEqual("", encoder.Encode(BuiltinCodecs.HtmlAttribute, null));
			Assert.AreEqual("&lt;script&#62;", encoder.Encode(BuiltinCodecs.HtmlAttribute, "<script>"));
			Assert.AreEqual(",.-_", encoder.Encode(BuiltinCodecs.HtmlAttribute, ",.-_"));
			Assert.AreEqual("&#32;&#33;&#64;&#36;&#37;&#40;&#41;&#61;&#43;&#123;&#125;&#91;&#93;",
				encoder.Encode(BuiltinCodecs.HtmlAttribute, " !@$%()=+{}[]"));
		}

		/// <summary> Test of EncodeForJavaScript method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_EncodeForJavaScript()
		{
			Console.Out.WriteLine("EncodeForJavaScript");
			var encoder = Esapi.Encoder;
			Assert.AreEqual("''", encoder.Encode(BuiltinCodecs.JavaScript, null));
			Assert.AreEqual("'\\x3cscript\\x3e'", encoder.Encode(BuiltinCodecs.JavaScript, "<script>"));
			Assert.AreEqual("',.-_ '", encoder.Encode(BuiltinCodecs.JavaScript, ",.-_ "));
			Assert.AreEqual("'\\x21\\x40\\x24\\x25\\x28\\x29\\x3d\\x2b\\x7b\\x7d\\x5b\\x5d'",
				encoder.Encode(BuiltinCodecs.JavaScript, "!@$%()=+{}[]"));
		}

		/// <summary>
		///     Test of EncodeForVisualBasicScript method, of class
		///     Owasp.Esapi.Encoder.
		/// </summary>
		[Test]
		public void Test_EncodeForVbScript()
		{
			Console.Out.WriteLine("EncodeForVbScript");
			var encoder = Esapi.Encoder;
			encoder.Encode(BuiltinCodecs.VbScript, null).ShouldBe("\"\"");
			encoder.Encode(BuiltinCodecs.VbScript, "<script>").ShouldBe("chrw(60)&\"script\"&chrw(62)");
			encoder.Encode(BuiltinCodecs.VbScript, "x !@$%()=+{}[]").ShouldBe(
				"\"x \"&chrw(33)&chrw(64)&chrw(36)&chrw(37)&chrw(40)&chrw(41)&chrw(61)&chrw(43)&chrw(123)&chrw(125)&chrw(91)&chrw(93)");
			encoder.Encode(BuiltinCodecs.VbScript, "alert('ESAPI test!')")
				.ShouldBe("\"alert\"&chrw(40)&chrw(39)&\"ESAPI test\"&chrw(33)&chrw(39)&chrw(41)");
			encoder.Encode(BuiltinCodecs.VbScript, "jeff.williams@aspectsecurity.com")
				.ShouldBe("\"jeff.williams\"&chrw(64)&\"aspectsecurity.com\"");
			encoder.Encode(BuiltinCodecs.VbScript, "test <> test").ShouldBe("\"test \"&chrw(60)&chrw(62)&\" test\"");
		}


		/// <summary> Test of EncodeForXML method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_EncodeForXML()
		{
			Console.Out.WriteLine("EncodeForXML");
			var encoder = Esapi.Encoder;
			Assert.AreEqual(null, encoder.Encode(BuiltinCodecs.Xml, null));
			Assert.AreEqual("", encoder.Encode(BuiltinCodecs.Xml, ""));
			Assert.AreEqual(" ", encoder.Encode(BuiltinCodecs.Xml, " "));
			Assert.AreEqual("&lt;script&gt;", encoder.Encode(BuiltinCodecs.Xml, "<script>"));
			Assert.AreEqual(",.-_", encoder.Encode(BuiltinCodecs.Xml, ",.-_"));
			Assert.AreEqual("&#33;&#64;&#36;&#37;&#40;&#41;&#61;&#43;&#123;&#125;&#91;&#93;"
				, encoder.Encode(BuiltinCodecs.Xml, "!@$%()=+{}[]"));
		}

		/// <summary> Test of EncodeForXMLAttribute method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_EncodeForXMLAttribute()
		{
			Console.Out.WriteLine("EncodeForXMLAttribute");
			var encoder = Esapi.Encoder;
			Assert.AreEqual("", encoder.Encode(BuiltinCodecs.XmlAttribute, null));
			Assert.AreEqual("&#32;", encoder.Encode(BuiltinCodecs.XmlAttribute, " "));
			Assert.AreEqual("&lt;script&gt;", encoder.Encode(BuiltinCodecs.XmlAttribute, "<script>"));
			Assert.AreEqual(",.-_", encoder.Encode(BuiltinCodecs.XmlAttribute, ",.-_"));
			Assert.AreEqual("&#32;&#33;&#64;&#36;&#37;&#40;&#41;&#61;&#43;&#123;&#125;&#91;&#93;",
				encoder.Encode(BuiltinCodecs.XmlAttribute, " !@$%()=+{}[]"));
		}

		/// <summary> Test of EncodeForURL method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_EncodeForURL()
		{
			Console.Out.WriteLine("EncodeForURL");
			var encoder = Esapi.Encoder;
			Assert.AreEqual("", encoder.Encode(BuiltinCodecs.Url, null));
			Assert.AreEqual("%3cscript%3e", encoder.Encode(BuiltinCodecs.Url, "<script>"));
		}

		/// <summary> Test of DecodeFromURL method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_DecodeFromURL()
		{
			Console.Out.WriteLine("DecodeFromURL");
			var encoder = Esapi.Encoder;
			try
			{
				// Assert.AreEqual("", encoder.DecodeFromUrl(null));
				Assert.AreEqual("<script>", encoder.Decode(BuiltinCodecs.Url, "%3Cscript%3E"));
				Assert.AreEqual("     ", encoder.Decode(BuiltinCodecs.Url, "+++++"));
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		/// <summary> Test of EncodeForBase64 method, of class Owasp.Esapi.Encoder.</summary>
		[Test]
		public void Test_EncodeForBase64()
		{
			Console.Out.WriteLine("EncodeForBase64");
			var encoder = Esapi.Encoder;
			try
			{
				for (var i = 0; i < 100; i++)
				{
					var random = Esapi.Randomizer.GetRandomString(20, CharSetValues.Specials);
					var encoded = encoder.Encode(BuiltinCodecs.Base64, random);
					var decoded = encoder.Decode(BuiltinCodecs.Base64, encoded);
					Assert.AreEqual(random, decoded);
				}
			}
			catch (IOException)
			{
				Assert.Fail();
			}
		}

		[Test]
		public void Test_AddCodec()
		{
			var mocks = new MockRepository();

			var codecName = Guid.NewGuid().ToString();
			var codec = mocks.StrictMock<ICodec>();

			Esapi.Encoder.AddCodec(codecName, codec);
			Assert.AreSame(Esapi.Encoder.GetCodec(codecName), codec);
		}

		[Test]
		public void Test_AddWrongCodecName()
		{
			var mocks = new MockRepository();

			Assert.Throws<ArgumentNullException>(() =>
				Esapi.Encoder.AddCodec(null, mocks.StrictMock<ICodec>()));
		}

		[Test]
		public void Test_AddDuplicateCodec()
		{
			var mocks = new MockRepository();

			var codecName = Guid.NewGuid().ToString();

			Esapi.Encoder.AddCodec(codecName, mocks.StrictMock<ICodec>());
			Assert.Throws<ArgumentException>(() =>
				Esapi.Encoder.AddCodec(codecName, mocks.StrictMock<ICodec>()));
		}

		[Test]
		public void Test_RemoveCodec()
		{
			var mocks = new MockRepository();

			var codecName = Guid.NewGuid().ToString();
			var codec = mocks.StrictMock<ICodec>();

			Esapi.Encoder.AddCodec(codecName, codec);
			Assert.AreSame(Esapi.Encoder.GetCodec(codecName), codec);

			Esapi.Encoder.RemoveCodec(codecName);
			Assert.IsNull(Esapi.Encoder.GetCodec(codecName));
		}

		[Test]
		public void Test_Encode()
		{
			var mocks = new MockRepository();

			var testString = Guid.NewGuid().ToString();
			var codecName = Guid.NewGuid().ToString();

			var codec = mocks.StrictMock<ICodec>();
			Expect.Call(codec.Encode(testString)).Return(testString);
			mocks.ReplayAll();

			Esapi.Encoder.AddCodec(codecName, codec);
			Assert.AreSame(Esapi.Encoder.GetCodec(codecName), codec);

			Assert.AreEqual(testString, Esapi.Encoder.Encode(codecName, testString));
			mocks.VerifyAll();
		}

		[Test]
		public void Test_EncodeWrongCodecName()
		{
			var codecName = Guid.NewGuid().ToString();

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				Esapi.Encoder.Encode(codecName, "test"));
		}

		[Test]
		public void Test_Decode()
		{
			var mocks = new MockRepository();

			var testString = Guid.NewGuid().ToString();
			var codecName = Guid.NewGuid().ToString();

			var codec = mocks.StrictMock<ICodec>();
			Expect.Call(codec.Decode(testString)).Return(testString);
			mocks.ReplayAll();

			Esapi.Encoder.AddCodec(codecName, codec);
			Esapi.Encoder.GetCodec(codecName).ShouldBeSameAs(codec);

			Esapi.Encoder.Decode(codecName, testString).ShouldBe(testString);
			mocks.VerifyAll();
		}

		[Test]
		public void Test_DecodeWrongCodecName()
		{
			var codecName = Guid.NewGuid().ToString();

			Assert.Throws<ArgumentOutOfRangeException>(() =>
				Esapi.Encoder.Decode(codecName, "test"));
		}

		[Test]
		public void Test_CanonicalizeNullCodec()
		{
			var codecs = new List<string>();
			codecs.Add(null);

			Esapi.Encoder.Canonicalize(codecs, "\0", false);
		}

		/// <summary>
		///     Tests loading of configuration defined encoder
		/// </summary>
		[Test]
		public void Test_LoadCustom()
		{
			// Set new
			EsapiConfig.Instance.Encoder.Type = typeof(SurrogateEncoder).AssemblyQualifiedName;

			var encoder = Esapi.Encoder;
			Assert.IsTrue(encoder.GetType().Equals(typeof(SurrogateEncoder)));
		}

		/// <summary>
		///     Tests loading of assembly defined codecs in a configuration defined
		///     encoder
		/// </summary>
		[Test]
		public void Test_LoadCustomAddinAssembly()
		{
			var mocks = new MockRepository();

			// Set new
			EsapiConfig.Instance.Encoder.Type = typeof(SurrogateEncoder).AssemblyQualifiedName;

			// Set assemblies to load
			var addinAssembly = new AddinAssemblyElement();
			addinAssembly.Name = typeof(Esapi).Assembly.FullName;
			EsapiConfig.Instance.Encoder.Codecs.Assemblies.Add(addinAssembly);

			// Set mock expectations
			var mockEncoder = mocks.StrictMock<IEncoder>();

			// Load default
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.Base64, null); })
				.Constraints(Is.Equal(BuiltinCodecs.Base64), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.Html, null); })
				.Constraints(Is.Equal(BuiltinCodecs.Html), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.HtmlAttribute, null); })
				.Constraints(Is.Equal(BuiltinCodecs.HtmlAttribute), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.JavaScript, null); })
				.Constraints(Is.Equal(BuiltinCodecs.JavaScript), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.Url, null); })
				.Constraints(Is.Equal(BuiltinCodecs.Url), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.VbScript, null); })
				.Constraints(Is.Equal(BuiltinCodecs.VbScript), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.Xml, null); })
				.Constraints(Is.Equal(BuiltinCodecs.Xml), Is.Anything());
			Expect.Call(delegate { mockEncoder.AddCodec(BuiltinCodecs.XmlAttribute, null); })
				.Constraints(Is.Equal(BuiltinCodecs.XmlAttribute), Is.Anything());
			mocks.ReplayAll();

			// Create and test
			SurrogateEncoder.DefaultEncoder = mockEncoder;
			var encoder = Esapi.Encoder;

			Assert.IsTrue(encoder.GetType().Equals(typeof(SurrogateEncoder)));
			mocks.VerifyAll();
		}

		/// <summary>
		///     Tests loading of configuration defined codecs
		/// </summary>
		[Test]
		public void Test_LoadCustomCodecs()
		{
			var mocks = new MockRepository();

			// Set new
			EsapiConfig.Instance.Encoder.Type = typeof(SurrogateEncoder).AssemblyQualifiedName;

			// Set codecs to load
			string[] codecNames = {Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
			foreach (var codecName in codecNames)
			{
				var codecElement = new CodecElement();
				codecElement.Name = codecName;
				codecElement.Type = typeof(SurrogateCodec).AssemblyQualifiedName;

				EsapiConfig.Instance.Encoder.Codecs.Add(codecElement);
			}

			// Set mock expectations
			var mockEncoder = mocks.StrictMock<IEncoder>();

			// Custom codecs are loaded and are of proper type
			foreach (var codecName in codecNames)
				Expect.Call(delegate { mockEncoder.AddCodec(codecName, null); })
					.Constraints(Is.Equal(codecName), Is.TypeOf<SurrogateCodec>());
			mocks.ReplayAll();

			// Create and test
			SurrogateEncoder.DefaultEncoder = mockEncoder;
			var encoder = Esapi.Encoder;

			Assert.IsTrue(encoder.GetType().Equals(typeof(SurrogateEncoder)));
			mocks.VerifyAll();
		}
	}
}