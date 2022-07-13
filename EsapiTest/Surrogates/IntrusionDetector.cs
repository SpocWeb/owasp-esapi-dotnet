using System;
using Owasp.Esapi;
using Owasp.Esapi.Interfaces;
using Owasp.Esapi.Runtime;

namespace EsapiTest.Surrogates
{
	//Forward intrusion detector
	internal class SurrogateIntrusionDetector : IIntrusionDetector
	{
		internal static IIntrusionDetector DefaultDetector;
		IIntrusionDetector _detector;

		public IIntrusionDetector Impl
		{
			get => _detector == null ? DefaultDetector : _detector;
			set => _detector = value;
		}

		#region IIntrusionDetector Members

		public void AddThreshold(Threshold threshold)
		{
			Impl.AddThreshold(threshold);
		}

		public bool RemoveThreshold(string eventName)
		{
			return Impl.RemoveThreshold(eventName);
		}

		public void AddException(Exception exception)
		{
			Impl.AddException(exception);
		}

		public void AddEvent(string eventName)
		{
			Impl.AddEvent(eventName);
		}

		#endregion
	}

	// Forward action
	internal class SurrogateAction : IAction
	{
		public IAction Impl { get; set; }

		#region IAction Members

		public void Execute(ActionArgs args)
		{
			Impl.Execute(args);
		}

		#endregion
	}
}