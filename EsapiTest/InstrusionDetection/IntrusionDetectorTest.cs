using System;
using EsapiTest.Surrogates;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Errors;
using Owasp.Esapi.Runtime.Actions;

namespace EsapiTest.InstrusionDetector
{
	/// <summary>
	///     Summary description for IntrusionDetector
	/// </summary>
	public class IntrusionDetectorTest
	{
		[Test]
		public void InitializeTests()
		{
			Esapi.Reset();
			EsapiConfig.Reset();
		}

		[Test]
		public void Test_AddException()
		{
			Esapi.IntrusionDetector.AddException(new IntrusionException("user message", "log message"));
		}

		[Test]
		public void Test_AddESAPIException()
		{
			var secExp = new EnterpriseSecurityException();
			Esapi.IntrusionDetector.AddException(secExp);
		}

		[Test]
		public void Test_AddExceptionSecurityEvent()
		{
			var evtName = typeof(ArgumentException).FullName;

			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			var threshold = new Threshold(evtName, 1, 1, new[] {"log"});
			detector.AddThreshold(threshold);

			var arg = new ArgumentException();
			detector.AddException(arg);
		}

		[Test]
		public void Test_AddEvent()
		{
			var evtName = Guid.NewGuid().ToString();

			Esapi.IntrusionDetector.AddEvent(evtName);
		}

		[Test]
		public void Test_AddThreshold()
		{
			var evtName = Guid.NewGuid().ToString();

			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			var threshold = new Threshold(evtName, 1, 1, new[] {"logout"});
			detector.AddThreshold(threshold);
		}

		[Test]
		public void Test_AddThresholdMissingAction()
		{
			var evtName = Guid.NewGuid().ToString();

			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			var threshold = new Threshold(evtName, 1, 1, new[] {Guid.NewGuid().ToString()});
			Assert.Throws<ArgumentNullException>(() =>
				detector.AddThreshold(threshold));
		}

		[Test]
		public void Test_AddNullThreshold()
		{
			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			Assert.Throws<ArgumentNullException>(() =>
				detector.AddThreshold(null));
		}

		[Test]
		public void Test_AddDuplicateThreshold()
		{
			var evtName = Guid.NewGuid().ToString();

			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			var threshold = new Threshold(evtName, 1, 1, new[] {BuiltinActions.FormsAuthenticationLogout});
			detector.AddThreshold(threshold);

			var dup = new Threshold(evtName, 2, 2, null);
			Assert.Throws<ArgumentException>(() =>
				detector.AddThreshold(dup));
		}

		[Test]
		public void Test_RemoveThreshold()
		{
			var evtName = Guid.NewGuid().ToString();

			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			var threshold = new Threshold(evtName, 1, 1, new[] {"logout"});
			detector.AddThreshold(threshold);

			Assert.IsTrue(detector.RemoveThreshold(evtName));
		}

		[Test]
		public void Test_IntrusionDetected()
		{
			var evtName = Guid.NewGuid().ToString();

			var detector = Esapi.IntrusionDetector as IntrusionDetector;
			Assert.IsNotNull(detector);

			var threshold = new Threshold(evtName, 1, 1, new[] {"log"});
			detector.AddThreshold(threshold);

			Esapi.IntrusionDetector.AddEvent(evtName);
		}

		/// <summary>
		///     Test loading of a custom intrusion detector
		/// </summary>
		[Test]
		public void Test_LoadCustom()
		{
			// Set new 
			EsapiConfig.Instance.IntrusionDetector.Type = typeof(SurrogateIntrusionDetector).AssemblyQualifiedName;

			var detector = Esapi.IntrusionDetector;
			Assert.IsTrue(detector.GetType().Equals(typeof(SurrogateIntrusionDetector)));
		}
	}
}