using System;
using System.Collections.Generic;
using NUnit.Framework;
using Owasp.Esapi.Runtime;
using Rhino.Mocks;

namespace EsapiTest.Runtime
{
	internal class ObjectRepositoryMock
	{
		/// <summary>
		///     Create named objects dictionary
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mocks"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		internal static IDictionary<string, T> MockNamedObjects<T>(MockRepository mocks, int size)
			where T : class
		{
			Assert.IsNotNull(mocks);
			return MockNamedObjects(() => mocks.StrictMock<T>(), size);
		}

		/// <summary>
		///     Create named objects dictionary
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="size"></param>
		/// <returns></returns>
		internal static IDictionary<string, T> MockNamedObjects<T>(Func<T> mockFactory, int size)
			where T : class
		{
			Assert.IsNotNull(mockFactory);
			Assert.IsTrue(size > 0);

			var objects = new Dictionary<string, T>();

			for (var i = 0; i < size; ++i) objects[Guid.NewGuid().ToString()] = mockFactory();

			return objects;
		}

		/// <summary>
		///     Add named objects
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="from"></param>
		/// <param name="to"></param>
		internal static void AddNamedObjects<T>(IDictionary<string, T> from, IObjectRepository<string, T> to)
			where T : class
		{
			foreach (var k in from.Keys) to.Register(k, from[k]);
		}

		/// <summary>
		///     Assert contains
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		internal static void AssertContains<T>(IDictionary<string, T> source, IObjectRepository<string, T> target)
			where T : class
		{
			Assert.AreEqual(source.Count, target.Count);

			foreach (var k in source.Keys) Assert.AreEqual(source[k], target.Get(k));
		}

		/// <summary>
		///     For each
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objects"></param>
		/// <param name="action"></param>
		internal static void ForEach<T>(IObjectRepository<string, T> objects, Action<T> action)
			where T : class
		{
			foreach (var v in objects.Objects) action(v);
		}

		/// <summary>
		///     Assert create remove
		/// </summary>
		/// <param name="objects"></param>
		internal static void AssertMockAddRemove<T>(Func<T> mockFactory, IObjectRepository<string, T> objects)
			where T : class
		{
			Assert.IsNotNull(mockFactory);
			Assert.IsNotNull(objects);

			var name = Guid.NewGuid().ToString();
			var t = mockFactory();

			objects.Register(name, t);
			Assert.AreEqual(objects.Get(name), t);

			objects.Revoke(name);
			Assert.IsFalse(objects.Objects.Contains(t));
			Assert.IsFalse(objects.Ids.Contains(name));
		}

		internal static void AssertMockAddRemove<T>(MockRepository mocks, IObjectRepository<string, T> objects)
			where T : class
		{
			Assert.IsNotNull(mocks);
			AssertMockAddRemove(() => mocks.StrictMock<T>(), objects);
		}
	}
}