namespace Owasp.Esapi.Interfaces
{
	/// <summary> Defines a Pair of methods for encoding and decoding Strings </summary>
	public interface ICodec
	{
		/// <summary>
		///     Decode a String that was encoded using the encode method in this Class.
		/// </summary>
		/// <param name="input">The string to decode.</param>
		/// <returns>The decoded string.</returns>
		string Encode(string input);

		/// <summary>
		///     Decode a String that was encoded using the encode method in this Class.
		/// </summary>
		/// <param name="input">The string to decode.</param>
		/// <returns>The decoded string.</returns>
		string Decode(string input);
	}
}