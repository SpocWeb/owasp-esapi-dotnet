using System;
using System.Collections;
using NUnit.Framework;
using Owasp.Esapi;

namespace EsapiTest
{
	/// <summary>
	///     Summary description for Randomizer
	/// </summary>
	public class RandomizerTest
	{
		/// <summary>
		///     Gets or sets the test context which provides
		///     information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary> Test of GetRandomString method, of class Owasp.Esapi.Randomizer.</summary>
		[Test]
		public void Test_GetRandomString()
		{
			Console.Out.WriteLine("GetRandomString");
			var length = 20;
			var randomizer = Esapi.Randomizer;
			for (var i = 0; i < 100; i++)
			{
				var result = randomizer.GetRandomString(length, CharSetValues.Alphanumerics);
				Assert.AreEqual(length, result.Length);
			}
		}

		/// <summary> Test of GetRandomInteger method, of class Owasp.Esapi.Randomizer.</summary>
		[Test]
		public void Test_GetRandomInteger()
		{
			Console.Out.WriteLine("GetRandomInteger");
			var min = int.MinValue;
			var max = int.MaxValue;
			var randomizer = Esapi.Randomizer;
			var minResult = (max - min) / 2;
			var maxResult = (max - min) / 2;
			for (var i = 0; i < 100; i++)
			{
				var result = randomizer.GetRandomInteger(min, max);
				if (result < minResult)
					minResult = result;
				if (result > maxResult)
					maxResult = result;
			}
			Assert.AreEqual(true, minResult >= min && maxResult <= max);
		}

		/// <summary> Test of GetRandomDouble method, of class Owasp.Esapi.Randomizer.</summary>
		[Test]
		public void Test_GetRandomDouble()
		{
			Console.Out.WriteLine("GetRandomDouble");
			double min = -20.5234F;
			double max = 100.12124F;
			var randomizer = Esapi.Randomizer;
			var minResult = (max - min) / 2;
			var maxResult = (max - min) / 2;
			for (var i = 0; i < 100; i++)
			{
				var result = randomizer.GetRandomDouble(min, max);
				if (result < minResult)
					minResult = result;
				if (result > maxResult)
					maxResult = result;
			}
			Assert.AreEqual(true, minResult >= min && maxResult < max);
		}


		/// <summary> Test of GetRandomGUID method, of class Owasp.Esapi.Randomizer.</summary>
		[Test]
		public void Test_GetRandomGUID()
		{
			Console.Out.WriteLine("GetRandomGUID");
			var randomizer = Esapi.Randomizer;
			var list = new ArrayList();
			for (var i = 0; i < 100; i++)
			{
				var guid = randomizer.GetRandomGUID().ToString();
				if (list.Contains(guid))
					Assert.Fail();
				list.Add(guid);
			}
		}

		#region Additional test attributes

		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//

		#endregion
	}
}