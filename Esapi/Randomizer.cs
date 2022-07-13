using System;
using System.Security.Cryptography;
using System.Text;
using Owasp.Esapi.Errors;
using Owasp.Esapi.Interfaces;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi
{
	/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer" />
	/// <summary>
	///     Reference implementation of the <see cref="Owasp.Esapi.Interfaces.IRandomizer" /> interface.
	///     Builds on the MSCAPI provider and <see cref="RandomNumberGenerator" />
	///     to provide a cryptographically strong source of entropy.
	///     The specific algorithm used is configurable in the ESAPI properties.
	/// </summary>
	public class Randomizer : IRandomizer
	{
		readonly RandomNumberGenerator randomNumberGenerator;

		/// <summary>
		///     Instantiates the class, with the appropriate algorithm.
		/// </summary>
		public Randomizer()
		{
			var algorithm = Esapi.SecurityConfiguration.RandomAlgorithm;
			try
			{
				randomNumberGenerator = RandomNumberGenerator.Create(algorithm);
			}
			catch (Exception e)
			{
				throw new EncryptionException(EM.Randomizer_Failure,
					string.Format(EM.Randomizer_AlgCreateFailed1, algorithm), e);
			}
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer.GetRandomBoolean()" />
		public bool GetRandomBoolean()
		{
			var randomByte = new byte[1];
			randomNumberGenerator.GetBytes(randomByte);
			return randomByte[0] >= 128;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer.GetRandomGUID()" />
		public Guid GetRandomGUID()
		{
			var guidString = string.Format("{0}-{1}-{2}-{3}-{4}",
				GetRandomString(8, CharSetValues.Hex),
				GetRandomString(4, CharSetValues.Hex),
				GetRandomString(4, CharSetValues.Hex),
				GetRandomString(4, CharSetValues.Hex),
				GetRandomString(12, CharSetValues.Hex));
			return new Guid(guidString);
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer.GetRandomString(int, char[])" />
		public string GetRandomString(int length, char[] characterSet)
		{
			var sb = new StringBuilder();

			for (var loop = 0; loop < length; loop++)
			{
				var index = GetRandomInteger(0, characterSet.GetLength(0) - 1);
				sb.Append(characterSet[index]);
			}
			var nonce = sb.ToString();
			return nonce;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer.GetRandomInteger(int, int)" />
		public int GetRandomInteger(int min, int max)
		{
			var range = (double) max - min;
			var randomBytes = new byte[sizeof(int)];
			randomNumberGenerator.GetBytes(randomBytes);
			var randomFactor = BitConverter.ToUInt32(randomBytes, 0);
			var divisor = (double) randomFactor / uint.MaxValue;
			var randomNumber = Convert.ToInt32(Math.Round(range * divisor) + min);
			return randomNumber;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer.GetRandomDouble(double, double)" />
		public double GetRandomDouble(double min, double max)
		{
			// This method only gives you 32 bits of entropy (based of random int).
			var factor = max - min;
			var random = GetRandomInteger(0, int.MaxValue) / (double) int.MaxValue;
			return random * factor + min;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IRandomizer.GetRandomFilename(string)" />
		public string GetRandomFilename(string extension)
		{
			return GetRandomString(12, CharSetValues.Alphanumerics) + "." + extension;
		}

		static bool Contains(StringBuilder sb, char c)
		{
			for (var i = 0; i < sb.Length; i++)
				if (sb[i] == c)
					return true;
			return false;
		}
	}
}