using System;
using System.Collections.Generic;

namespace Owasp.Esapi.Runtime
{
	/// <summary>
	///     Object repository
	/// </summary>
	/// <typeparam id="TObject"></typeparam>
	internal class NamedObjectRepository<TObject> : IObjectRepository<string, TObject>
		where TObject : class
	{
		public NamedObjectRepository()
		{
			Entries = new Dictionary<string, TObject>();
		}

		public NamedObjectRepository(IDictionary<string, TObject> entries)
		{
			Entries = entries != null ? new Dictionary<string, TObject>(entries) : new Dictionary<string, TObject>();
		}

		/// <summary>
		///     Get entries
		/// </summary>
		protected Dictionary<string, TObject> Entries { get; }

		#region IObjectRepository<TName, TObject> Members

		/// <summary>
		///     Register object
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string Register(TObject value)
		{
			var id = Guid.NewGuid().ToString();
			Register(id, value);
			return id;
		}

		/// <summary>
		///     Register object
		/// </summary>
		/// <param id="id"></param>
		/// <param id="value"></param>
		/// <returns></returns>
		public void Register(string id, TObject value)
		{
			if (string.IsNullOrEmpty(id) || Entries.ContainsKey(id)) throw new ArgumentException("Invalid id", "id");
			if (value == null) throw new ArgumentNullException("value");
			Entries[id] = value;
		}

		/// <summary>
		///     Revoke object
		/// </summary>
		/// <param id="id"></param>
		/// <returns></returns>
		public void Revoke(string id)
		{
			Entries.Remove(id);
		}

		/// <summary>
		///     Lookup object
		/// </summary>
		/// <param id="id"></param>
		/// <param id="value"></param>
		/// <returns></returns>
		public bool Lookup(string id, out TObject value)
		{
			return Entries.TryGetValue(id, out value);
		}

		/// <summary>
		///     Get count
		/// </summary>
		public int Count => Entries.Count;

		/// <summary>
		///     Get keys
		/// </summary>
		public ICollection<string> Ids => Entries.Keys;

		/// <summary>
		///     Get objects
		/// </summary>
		public ICollection<TObject> Objects => Entries.Values;

		/// <summary>
		///     Get object
		/// </summary>
		/// <param id="id"></param>
		/// <returns></returns>
		public TObject Get(string id)
		{
			return Entries[id];
		}

		#endregion
	}
}