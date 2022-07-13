using System.Configuration;

namespace Owasp.Esapi.Configuration
{
	/// <summary>
	///     The NamedElement Configuration Element.
	/// </summary>
	public class NamedElement : ConfigurationElement
	{
		#region Name Property

		/// <summary>
		///     The XML name of the <see cref="Name" /> property.
		/// </summary>
		internal const string NamePropertyName = "name";

		/// <summary>
		///     Gets or sets the Name.
		/// </summary>
		[ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = true, IsDefaultCollection = false)]
		public string Name
		{
			get => (string) base[NamePropertyName];
			set => base[NamePropertyName] = value;
		}

		#endregion
	}

	/// <summary>
	///     A collection of NamedElement instances.
	/// </summary>
	[ConfigurationCollection(typeof(NamedElement),
		CollectionType = ConfigurationElementCollectionType.BasicMapAlternate, AddItemName = NamedElementPropertyName)]
	public class NamedCollection : ConfigurationElementCollection
	{
		#region Constants

		/// <summary>
		///     The XML name of the individual <see cref="NamedElement" /> instances in this collection.
		/// </summary>
		internal const string NamedElementPropertyName = "executeRuleElement";

		#endregion

		#region Indexer

		/// <summary>
		///     Gets the <see cref="NamedElement" /> at the specified index.
		/// </summary>
		/// <param name="index">The index of the <see cref="NamedElement" /> to retrieve</param>
		public NamedElement this[int index] => (NamedElement) BaseGet(index);

		#endregion

		#region Add

		/// <summary>
		///     Adds the specified <see cref="NamedElement" />.
		/// </summary>
		/// <param name="executeRuleElement">The <see cref="NamedElement" /> to add.</param>
		public void Add(NamedElement executeRuleElement)
		{
			base.BaseAdd(executeRuleElement);
		}

		#endregion

		#region Remove

		/// <summary>
		///     Removes the specified <see cref="NamedElement" />.
		/// </summary>
		/// <param name="executeRuleElement">The <see cref="NamedElement" /> to remove.</param>
		public void Remove(NamedElement executeRuleElement)
		{
			BaseRemove(executeRuleElement);
		}

		#endregion

		#region Overrides

		/// <summary>
		///     Gets the type of the <see cref="ConfigurationElementCollection" />.
		/// </summary>
		/// <returns>The <see cref="ConfigurationElementCollectionType" /> of this collection.</returns>
		public override ConfigurationElementCollectionType CollectionType =>
			ConfigurationElementCollectionType.BasicMapAlternate;

		/// <summary>
		///     Indicates whether the specified <see cref="ConfigurationElement" /> exists in the
		///     <see cref="ConfigurationElementCollection" />.
		/// </summary>
		/// <param name="elementName">The name of the element to verify.</param>
		/// <returns>
		///     <see langword="true" /> if the element exists in the collection; otherwise, <see langword="false" />. The default
		///     is <see langword="false" />.
		/// </returns>
		protected override bool IsElementName(string elementName)
		{
			return elementName == NamedElementPropertyName;
		}

		/// <summary>
		///     Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="ConfigurationElement" /> to return the key for.</param>
		/// <returns>
		///     An <see cref="object" /> that acts as the key for the specified <see cref="ConfigurationElement" />.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((NamedElement) element).Name;
		}

		/// <summary>
		///     When overridden in a derived class, creates a new <see cref="ConfigurationElement" />.
		/// </summary>
		/// <returns>
		///     A new <see cref="ConfigurationElement" />.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new NamedElement();
		}

		#endregion
	}
}