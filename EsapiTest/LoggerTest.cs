using System;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Interfaces;

namespace EsapiTest
{
	/// <summary>
	///     Summary description for LoggerTest
	/// </summary>
	public class LoggerTest
	{
		readonly ILogger logger;

		public LoggerTest()
		{
			logger = Esapi.Logger;
		}

		/// <summary>
		///     Gets or sets the test context which provides
		///     information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary> Test of Info method, of class Owasp.Esapi.Logger.</summary>
		[Test]
		public void Test_Info()
		{
			Console.Out.WriteLine("Info");
			logger.Info(LogEventTypes.SECURITY, "test message");
			logger.Info(LogEventTypes.SECURITY, "test message", null);
			logger.Info(LogEventTypes.SECURITY, "%3escript%3f test message", null);
			logger.Info(LogEventTypes.SECURITY, "<script> test message", null);
		}

		/// <summary> Test of LogDebug method, of class Owasp.Esapi.Logger.</summary>
		[Test]
		public void Test_LogDebug()
		{
			Console.Out.WriteLine("logDebug");
			logger.Debug(LogEventTypes.SECURITY, "test message");
			logger.Debug(LogEventTypes.SECURITY, "test message", null);
		}

		/// <summary> Test of Error method, of class Owasp.Esapi.Logger.</summary>
		[Test]
		public void Test_Error()
		{
			Console.Out.WriteLine("Error");
			logger.Error(LogEventTypes.SECURITY, "test message");
			logger.Error(LogEventTypes.SECURITY, "test message", null);
		}

		/// <summary> Test of Warning method, of class Owasp.Esapi.Logger.</summary>
		[Test]
		public void Test_Warning()
		{
			Console.Out.WriteLine("Warning");
			logger.Warning(LogEventTypes.SECURITY, "test message");
			logger.Warning(LogEventTypes.SECURITY, "test message", null);
		}

		/// <summary> Test of Fatal method, of class Owasp.Esapi.Logger.</summary>
		[Test]
		public void Test_Fatal()
		{
			Console.Out.WriteLine("Fatal");
			logger.Fatal(LogEventTypes.SECURITY, "test message");
			logger.Fatal(LogEventTypes.SECURITY, "test message", null);
		}

		[Test]
		public void Test_ParseLogLevel()
		{
			Assert.AreEqual(LogLevels.ParseLogLevel("OFF"), LogLevels.OFF);
			Assert.AreEqual(LogLevels.ParseLogLevel("FATAL"), LogLevels.FATAL);
			Assert.AreEqual(LogLevels.ParseLogLevel("ERROR"), LogLevels.ERROR);
			Assert.AreEqual(LogLevels.ParseLogLevel("WARNING"), LogLevels.WARN);
			Assert.AreEqual(LogLevels.ParseLogLevel("INFO"), LogLevels.INFO);
			Assert.AreEqual(LogLevels.ParseLogLevel("DEBUG"), LogLevels.DEBUG);
			Assert.AreEqual(LogLevels.ParseLogLevel("ALL"), LogLevels.ALL);

			Assert.AreEqual(LogLevels.ParseLogLevel(string.Empty), LogLevels.ALL);
			Assert.AreEqual(LogLevels.ParseLogLevel(null), LogLevels.ALL);
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