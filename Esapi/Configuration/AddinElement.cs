﻿using System.Configuration;

namespace Owasp.Esapi.Configuration
{
	/// <summary>
	///     The AddinElement Configuration Element.
	/// </summary>
	public class AddinElement : ConfigurationElement
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

		#region Type Property

		/// <summary>
		///     The XML name of the <see cref="Type" /> property.
		/// </summary>
		internal const string TypePropertyName = "type";

		/// <summary>
		///     Gets or sets the Type.
		/// </summary>
		[ConfigurationProperty(TypePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
		public string Type
		{
			get => (string) base[TypePropertyName];
			set => base[TypePropertyName] = value;
		}

		#endregion

		#region properties Property

		/// <summary>
		///     The XML name of the <see cref="PropertiesPropertyName" /> property.
		/// </summary>
		internal const string PropertiesPropertyName = "properties";

		/// <summary>
		///     Gets or sets the properties.
		/// </summary>
		[ConfigurationProperty(PropertiesPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
		public KeyValueConfigurationCollection PropertyValues
		{
			get => (KeyValueConfigurationCollection) base[PropertiesPropertyName];
			set => base[PropertiesPropertyName] = value;
		}

		#endregion
	}
}