using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Owasp.Esapi.Errors;
using Owasp.Esapi.Interfaces;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi
{
	/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor" />
	/// <summary>
	///     Reference implementation of the <see cref="Owasp.Esapi.Interfaces.IEncryptor" /> IEncryptor interface. This
	///     implementation
	///     layers on the .NET provided cryptographic package.
	///     Algorithms used are configurable in the configuration file.
	/// </summary>
	public class Encryptor : IEncryptor
	{
		/// <summary>The asymmetric key pair </summary>
		readonly CspParameters asymmetricKeyPair;

		internal string encoding = Esapi.SecurityConfiguration.CharacterEncoding;

		internal string encryptAlgorithm = Esapi.SecurityConfiguration.EncryptionAlgorithm;
		internal string hashAlgorithm = Esapi.SecurityConfiguration.HashAlgorithm;

		/// <summary>
		///     The symmetric key
		/// </summary>
		readonly byte[] secretKey;

		internal string signatureAlgorithm = Esapi.SecurityConfiguration.DigitalSignatureAlgorithm;

		/// <summary>
		///     Public constructor.
		/// </summary>
		public Encryptor()
		{
			var pass = Esapi.SecurityConfiguration.MasterPassword;
			var salt = Esapi.SecurityConfiguration.MasterSalt;

			try
			{
				// Set up encryption and decryption                
				var symmetricAlgorithm = SymmetricAlgorithm.Create(encryptAlgorithm);
				var rfc2898 = new Rfc2898DeriveBytes(pass, salt);
				secretKey = rfc2898.GetBytes(symmetricAlgorithm.KeySize / 8);

				// TODO: Hardcoded value 13 is the code for DSA
				asymmetricKeyPair = new CspParameters(13);

				// The asymmetric key will be stored in the key container using the name ESAPI.
				asymmetricKeyPair.KeyContainerName = "ESAPI";
			}
			catch (Exception e)
			{
				throw new EncryptionException(EM.Encryptor_EncryptionFailure, EM.Encryptor_EncryptorCreateFailed, e);
			}
		}

		/// <summary>
		///     Gets a timestamp representing the current date and time to be used by
		///     other functions in the library.
		/// </summary>
		/// <returns>
		///     The timestamp in long format.
		/// </returns>
		public long TimeStamp => DateTime.Now.Ticks;

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.Hash(string, string)" />
		public string Hash(string plaintext, string salt)
		{
			try
			{
				// Create a new instance of the hash crypto service provider.
				var hasher = HashAlgorithm.Create(hashAlgorithm);

				// Convert the data to hash to an array of Bytes.
				var originalBytes = Encoding.UTF8.GetBytes(plaintext + salt);

				// Compute the Hash. This returns an array of Bytes.
				var hashBytes = hasher.ComputeHash(originalBytes);

				// rehash a number of times to help strengthen weak passwords
				// FIXME: ENHANCE make iterations configurable                
				for (var i = 0; i < 1024; i++) hashBytes = hasher.ComputeHash(hashBytes);
				// Optionally, represent the hash value as a base64-encoded string, 
				// For example, if you need to display the value or transmit it over a network.
				var hashBase64String = Convert.ToBase64String(hashBytes);

				return hashBase64String;
			}
			catch (Exception e)
			{
				throw new EncryptionException(EM.Encryptor_EncryptionFailure,
					string.Format(EM.Encryptor_WrongHashAlg1, hashAlgorithm), e);
			}
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.Encrypt(string)" />
		public string Encrypt(string plaintext)
		{
			try
			{
				// Create a new key and initialization vector.
				// If a key is not provided, a key of appropriate length is
				// automatically generated. You can retrieve its value through the Key
				// property. Similarly, an initialization vector is automatically
				// generated if you do not specify one.

				// Get the encryptor.
				var symmetricAlgorithm = SymmetricAlgorithm.Create(encryptAlgorithm);
				symmetricAlgorithm.GenerateIV();
				var iv = symmetricAlgorithm.IV;

				var encryptor = symmetricAlgorithm.CreateEncryptor(secretKey, iv);
				// Define a new CryptoStream object to hold the encrypted bytes and encrypt the data.
				var msEncrypt = new MemoryStream();
				var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
				try
				{
					// Convert the data to a byte array.
					var textConverter = Encoding.GetEncoding(encoding);
					var plaintextBytes = textConverter.GetBytes(plaintext);
					// Encrypt the data by writing it to the CryptoStream object.
					// Write all data to the crypto stream and flush it.
					csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
					csEncrypt.FlushFinalBlock();

					// Get encrypted array of bytes from the memory stream.
					var encryptedBytes = msEncrypt.ToArray();
					var encryptedBytesPlusIv = Combine(iv, encryptedBytes);
					return Convert.ToBase64String(encryptedBytesPlusIv);
				}
				finally
				{
					symmetricAlgorithm.Clear();
					msEncrypt.Close();
					csEncrypt.Close();
				}
			}
			catch (Exception e)
			{
				throw new EncryptionException(EM.Encryptor_EncryptionFailure,
					string.Format(EM.Encryptor_DecryptFailed1, e.Message), e);
			}
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.Decrypt(string)" />
		public string Decrypt(string ciphertext)
		{
			// Note - Cipher is not threadsafe so we create one locally
			try
			{
				var symmetricAlgorithm = SymmetricAlgorithm.Create(encryptAlgorithm);

				var ciphertextBytes = Convert.FromBase64String(ciphertext);

				// Ciphertext actually includes the IV, so we need to split it out
				// Get first part of array, which is IV
				var ivLength = symmetricAlgorithm.IV.Length;
				var ivBytes = new byte[ivLength];
				Array.Copy(ciphertextBytes, ivBytes, ivLength);

				// Get second part of array which is actual ciphertext
				var onlyCiphertextLength = ciphertextBytes.Length - ivLength;
				var onlyCiphertextBytes = new byte[onlyCiphertextLength];
				Array.Copy(ciphertextBytes, ivLength, onlyCiphertextBytes, 0, onlyCiphertextLength);

				var decryptor = symmetricAlgorithm.CreateDecryptor(secretKey, ivBytes);

				// Now decrypt the previously encrypted data using the decryptor.
				var msDecrypt = new MemoryStream(onlyCiphertextBytes);
				var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
				try
				{
					// Read the data out of the crypto stream.
					var plaintextBytes = new byte[onlyCiphertextLength];
					var decryptedBytes = csDecrypt.Read(plaintextBytes, 0, onlyCiphertextLength);

					// Convert the byte array back into a string.
					var textConverter = Encoding.GetEncoding(encoding);
					var plaintext = textConverter.GetString(plaintextBytes, 0, decryptedBytes);
					return plaintext;
				}
				finally
				{
					symmetricAlgorithm.Clear();
					msDecrypt.Close();
					csDecrypt.Close();
				}
			}
			catch (Exception e)
			{
				throw new EncryptionException(EM.Encryptor_DecryptionFailure,
					string.Format(EM.Encryptor_DecryptFailed1, e.Message), e);
			}
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.Sign(string)" />
		public string Sign(string data)
		{
			if (data == null) throw new ArgumentNullException();

			// Since this is the only asymmetric algorithm with signing capabilities, its hardcoded.
			// The more general APIs just aren't friendly.
			var dsaCsp = new DSACryptoServiceProvider(asymmetricKeyPair);
			var textConverter = Encoding.GetEncoding(encoding);
			var dataBytes = textConverter.GetBytes(data);
			var signatureBytes = dsaCsp.SignData(dataBytes);
			return Convert.ToBase64String(signatureBytes);
		}


		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.VerifySignature(string, string)" />
		public bool VerifySignature(string signature, string data)
		{
			try
			{
				var dsaCsp = new DSACryptoServiceProvider(asymmetricKeyPair);
				var textConverter = Encoding.GetEncoding(encoding);
				var signatureBytes = Convert.FromBase64String(signature);
				var dataBytes = textConverter.GetBytes(data);
				return dsaCsp.VerifyData(dataBytes, signatureBytes);
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		///     Creates a seal that binds a set of data and an expiration timestamp.
		/// </summary>
		/// <param name="data">
		///     The data to seal.
		/// </param>
		/// <param name="expiration">
		///     The timestamp of the expiration date of the data.
		/// </param>
		/// <returns>
		///     The seal value.
		/// </returns>
		/// <seealso cref="Owasp.Esapi.Interfaces.IEncryptor.Seal(string, long)">
		/// </seealso>
		public string Seal(string data, long expiration)
		{
			try
			{
				return Encrypt(expiration + ":" + data);
			}
			catch (EncryptionException e)
			{
				throw new IntegrityException(e.UserMessage, e.LogMessage, e);
			}
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.Unseal(string)" />
		public string Unseal(string seal)
		{
			string plaintext = null;
			try
			{
				plaintext = Decrypt(seal);
			}
			catch (EncryptionException e)
			{
				throw new EncryptionException(EM.Encryptor_InvalidSeal, EM.Encryptor_SealDecryptFailed, e);
			}

			var index = plaintext.IndexOf(":");
			if (index == -1) throw new EncryptionException(EM.Encryptor_InvalidSeal, EM.Encryptor_SealWrongFormat);

			var timestring = plaintext.Substring(0, index);
			var now = DateTime.Now.Ticks;
			var expiration = Convert.ToInt64(timestring);
			if (now > expiration) throw new EncryptionException(EM.Encryptor_InvalidSeal, EM.Encryptor_SealExpired);

			index = plaintext.IndexOf(":", index + 1);
			var sealedValue = plaintext.Substring(index + 1);
			return sealedValue;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncryptor.VerifySeal(string)" />
		public bool VerifySeal(string seal)
		{
			try
			{
				Unseal(seal);
				return true;
			}
			catch (EncryptionException)
			{
				return false;
			}
		}

		static byte[] Combine(byte[] a, byte[] b)
		{
			var c = new byte[a.Length + b.Length];
			Buffer.BlockCopy(a, 0, c, 0, a.Length);
			Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
			return c;
		}
	}
}