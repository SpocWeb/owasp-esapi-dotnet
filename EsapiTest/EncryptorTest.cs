using System;
using EsapiTest.Surrogates;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Errors;
using Owasp.Esapi.Interfaces;
using Rhino.Mocks;

namespace EsapiTest
{
	/// <summary>
	///     The Class EncryptorTest.
	/// </summary>
	public class EncryptorTest
	{
		[Test]
		public void InitializeTest()
		{
			Esapi.Reset();
			EsapiConfig.Reset();
		}

		/// <summary> Test of Hash method, of class Owasp.Esapi.Encryptor.</summary>
		[Test]
		public void Test_Hash()
		{
			Console.Out.WriteLine("hash");
			var encryptor = Esapi.Encryptor;
			var hash1 = encryptor.Hash("test1", "salt");
			var hash2 = encryptor.Hash("test2", "salt");
			Assert.IsFalse(hash1.Equals(hash2));
			var hash3 = encryptor.Hash("test", "salt1");
			var hash4 = encryptor.Hash("test", "salt2");
			Assert.IsFalse(hash3.Equals(hash4));
		}

		/// <summary>
		///     Test of Encrypt method, of class Owasp.Esapi.Encryptor.
		/// </summary>
		/// <throws>  EncryptionException </throws>
		/// <summary>
		///     the encryption exception
		/// </summary>
		[Test]
		public void Test_Encrypt()
		{
			Console.Out.WriteLine("Encrypt");
			var encryptor = Esapi.Encryptor;
			var plaintext = "test123";
			var ciphertext = encryptor.Encrypt(plaintext);
			var result = encryptor.Decrypt(ciphertext);
			Assert.AreEqual(plaintext, result);
		}

		/// <summary> Test of decrypt method, of class Owasp.Esapi.Encryptor.</summary>
		[Test]
		public void Test_Decrypt()
		{
			Console.Out.WriteLine("decrypt");
			var encryptor = Esapi.Encryptor;
			try
			{
				var plaintext = "test123";
				var ciphertext = encryptor.Encrypt(plaintext);
				Assert.IsFalse(plaintext.Equals(ciphertext));
				var result = encryptor.Decrypt(ciphertext);
				Assert.AreEqual(plaintext, result);
			}
			catch (EncryptionException)
			{
				Assert.Fail();
			}
		}

		/// <summary>
		///     Test of Sign method, of class Owasp.Esapi.Encryptor.
		/// </summary>
		/// <throws>  EncryptionException </throws>
		/// <summary>
		///     the encryption exception
		/// </summary>
		[Test]
		public void Test_Sign()
		{
			Console.Out.WriteLine("Sign");
			var encryptor = Esapi.Encryptor;
			var plaintext = Esapi.Randomizer.GetRandomString(32, CharSetValues.Alphanumerics);
			var signature = encryptor.Sign(plaintext);
			Assert.IsTrue(encryptor.VerifySignature(signature, plaintext));
			Assert.IsFalse(encryptor.VerifySignature(signature, "ridiculous"));
			Assert.IsFalse(encryptor.VerifySignature("ridiculous", plaintext));
		}

		/// <summary>
		///     Test of VerifySignature method, of class Owasp.Esapi.Encryptor.
		/// </summary>
		/// <throws>  EncryptionException </throws>
		/// <summary>
		///     the encryption exception
		/// </summary>
		[Test]
		public void Test_VerifySignature()
		{
			Console.Out.WriteLine("verifySignature");
			var encryptor = Esapi.Encryptor;
			var plaintext = Esapi.Randomizer.GetRandomString(32, CharSetValues.Alphanumerics);
			var signature = encryptor.Sign(plaintext);
			Assert.IsTrue(encryptor.VerifySignature(signature, plaintext));
		}


		/// <summary>
		///     Test of Seal method, of class Owasp.Esapi.Encryptor.
		/// </summary>
		/// <throws>  EncryptionException </throws>
		/// <summary>
		///     the encryption exception
		/// </summary>
		[Test]
		public void Test_Seal()
		{
			Console.Out.WriteLine("seal");
			var encryptor = Esapi.Encryptor;
			var plaintext = Esapi.Randomizer.GetRandomString(32, CharSetValues.Alphanumerics);
			var seal = encryptor.Seal(plaintext, encryptor.TimeStamp + 1000 * 60);
			encryptor.VerifySeal(seal);
		}

		/// <summary>
		///     Test of VerifySeal method, of class Owasp.Esapi.Encryptor.
		/// </summary>
		/// <throws>  EncryptionException </throws>
		/// <summary>
		///     the encryption exception
		/// </summary>
		[Test]
		public void Test_VerifySeal()
		{
			Console.Out.WriteLine("verifySeal");
			var encryptor = Esapi.Encryptor;
			var plaintext = Esapi.Randomizer.GetRandomString(32, CharSetValues.Alphanumerics);
			var seal = encryptor.Seal(plaintext, encryptor.TimeStamp + 1000 * 60);
			Assert.IsTrue(encryptor.VerifySeal(seal));
			Assert.IsFalse(encryptor.VerifySeal("ridiculous"));
			Assert.IsFalse(encryptor.VerifySeal(encryptor.Encrypt("ridiculous")));
			Assert.IsFalse(encryptor.VerifySeal(encryptor.Encrypt(100 + ":" + "ridiculous")));
			Assert.IsTrue(encryptor.VerifySeal(encryptor.Encrypt(long.MaxValue + ":" + "ridiculous")));
		}


		/// <summary> Test of decrypt method, of class Owasp.Esapi.Encryptor.</summary>
		[Test]
		public void Test_MulitpleInstances()
		{
			Console.Out.WriteLine("multiple instances");
			IEncryptor encryptor1 = new Encryptor();
			IEncryptor encryptor2 = new Encryptor();
			IEncryptor decryptor1 = new Encryptor();
			IEncryptor decryptor2 = new Encryptor();

			try
			{
				var plaintext = "test123";
				var ciphertext1 = encryptor1.Encrypt(plaintext);
				var ciphertext2 = encryptor2.Encrypt(plaintext);
				Assert.AreNotEqual(ciphertext1, ciphertext2);
				var plaintext1 = decryptor1.Decrypt(ciphertext1);
				var plaintext2 = decryptor2.Decrypt(ciphertext2);
				Assert.AreEqual(plaintext1, plaintext2);
			}

			catch (EncryptionException)
			{
				Assert.Fail();
			}
		}

		[Test]
		public void Test_LoadCustom()
		{
			var mocks = new MockRepository();

			// Set new encryptor
			EsapiConfig.Instance.Encryptor.Type = typeof(SurrogateEncryptor).AssemblyQualifiedName;

			var encryptor = Esapi.Encryptor;
			Assert.IsTrue(encryptor.GetType().Equals(typeof(SurrogateEncryptor)));

			// Do some calls
			var mockEncryptor = mocks.StrictMock<IEncryptor>();
			((SurrogateEncryptor) encryptor).Impl = mockEncryptor;

			Expect.Call(mockEncryptor.VerifySeal(null)).Return(true);
			Expect.Call(mockEncryptor.Seal(null, 0)).Return(null);
			mocks.ReplayAll();

			Assert.IsTrue(encryptor.VerifySeal(null));
			Assert.IsNull(encryptor.Seal(null, 0));
			mocks.VerifyAll();
		}
	}
}