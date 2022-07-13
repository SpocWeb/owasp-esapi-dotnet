using System;
using System.Collections.Generic;
using Owasp.Esapi.Errors;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi
{
	/// <summary>
	///     Security event
	/// </summary>
	internal class Event : IEquatable<Event>
	{
		readonly List<DateTime> _times;

		public Event(string name)
		{
			Name = name;
			_times = new List<DateTime>();
		}

		public string Name { get; }

		#region IEquatable<Event> Members

		public bool Equals(Event other)
		{
			if (other == null) return false;
			return Name == other.Name;
		}

		#endregion

		public void Increment(int maxOccurences, TimeSpan maxTimeSpan)
		{
			var now = DateTime.Now;
			_times.Add(now);

			while (_times.Count > maxOccurences)
				_times.RemoveAt(_times.Count - 1);

			if (_times.Count == maxOccurences)
				if (now - _times[maxOccurences - 1] < maxTimeSpan)
					throw new IntrusionException(EM.IntrusionDetector_ThresholdExceeded,
						string.Format(EM.InstrusionDetector_ThresholdExceeded1, Name));
		}

		#region Object overrides

		public override bool Equals(object obj)
		{
			return Equals(obj as Event);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion
	}
}