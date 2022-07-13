using System.Configuration;

namespace Owasp.Esapi.Configuration
{
	/// <summary>
	///     The ExecuteRuleElement Configuration Element.
	/// </summary>
	public class ExecuteRuleElement : ConfigurationElement
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

		#region On Property

		/// <summary>
		///     The XML name of the <see cref="On" /> property.
		/// </summary>
		internal const string OnPropertyName = "on";

		/// <summary>
		///     Gets or sets the On.
		/// </summary>
		[ConfigurationProperty(OnPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
		public string On
		{
			get => (string) base[OnPropertyName];
			set => base[OnPropertyName] = value;
		}

		#endregion

		#region FaultActions Property

		/// <summary>
		///     The XML name of the <see cref="FaultActions" /> property.
		/// </summary>
		internal const string FaultActionsPropertyName = "faultActions";

		/// <summary>
		///     Gets or sets the FaultActions.
		/// </summary>
		[ConfigurationProperty(FaultActionsPropertyName, IsRequired = false, IsKey = false,
			IsDefaultCollection = false)]
		public NamedCollection FaultActions
		{
			get => (NamedCollection) base[FaultActionsPropertyName];
			set => base[FaultActionsPropertyName] = value;
		}

		#endregion
	}

	/// <summary>
	///     A collection of ExecuteRuleElement instances.
	/// </summary>
	[ConfigurationCollection(typeof(ExecuteRuleElement),
		CollectionType = ConfigurationElementCollectionType.BasicMapAlternate,
		AddItemName = ExecuteRuleElementPropertyName)]
	public class ExecuteRuleCollection : ConfigurationElementCollection
	{
		#region Constants

		/// <summary>
		///     The XML name of the individual <see cref="ExecuteRuleElement" /> instances in this collection.
		/// </summary>
		internal const string ExecuteRuleElementPropertyName = "executeRuleElement";

		#endregion

		#region Indexer

		/// <summary>
		///     Gets the <see cref="ExecuteRuleElement" /> at the specified index.
		/// </summary>
		/// <param name="index">The index of the <see cref="ExecuteRuleElement" /> to retrieve</param>
		public ExecuteRuleElement this[int index] => (ExecuteRuleElement) BaseGet(index);

		#endregion

		#region Add

		/// <summary>
		///     Adds the specified <see cref="ExecuteRuleElement" />.
		/// </summary>
		/// <param name="executeRuleElement">The <see cref="ExecuteRuleElement" /> to add.</param>
		public void Add(ExecuteRuleElement executeRuleElement)
		{
			base.BaseAdd(executeRuleElement);
		}

		#endregion

		#region Remove

		/// <summary>
		///     Removes the specified <see cref="ExecuteRuleElement" />.
		/// </summary>
		/// <param name="executeRuleElement">The <see cref="ExecuteRuleElement" /> to remove.</param>
		public void Remove(ExecuteRuleElement executeRuleElement)
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
			return elementName == ExecuteRuleElementPropertyName;
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
			return ((ExecuteRuleElement) element).Name;
		}

		/// <summary>
		///     When overridden in a derived class, creates a new <see cref="ConfigurationElement" />.
		/// </summary>
		/// <returns>
		///     A new <see cref="ConfigurationElement" />.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ExecuteRuleElement();
		}

		#endregion
	}
}