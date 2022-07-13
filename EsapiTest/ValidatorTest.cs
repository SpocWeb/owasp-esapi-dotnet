using System;
using EsapiTest.Surrogates;
using NUnit.Framework;
using Owasp.Esapi;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Interfaces;
using Owasp.Esapi.ValidationRules;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace EsapiTest
{
	/// <summary>
	/// </summary>
	public class ValidatorTest
	{
		[Test]
		public void InitializeTest()
		{
			Esapi.Reset();
			EsapiConfig.Reset();

			SurrogateValidator.DefaultValidator = null;
		}

		[Test]
		public void Test_CreditCardValidator()
		{
			var validator = Esapi.Validator;
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.CreditCard, "1234 9876 0000 0008"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.CreditCard, "1234987600000008"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.CreditCard, "Garbage"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.CreditCard, "12349876000000082"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.CreditCard, "4417 1234 5678 9112"));
		}

		/// <summary> Test of IsValidDouble method, of class Owasp.Esapi.Validator.</summary>
		[Test]
		public void Test_IsValidDouble()
		{
			var validator = Esapi.Validator;
			//testing negative range
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "-4"));

			//testing empty string
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, ""));
			//testing non-integers
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "4.3214"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "-1.65"));
			//other testing
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "4"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "400"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "400000000"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "alsdkf"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "--10"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "14.1414234x"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "Infinity"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "-Infinity"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "NaN"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "-NaN"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, "+NaN"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "1e-6"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Double, "-1e-6"));

			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, string.Empty));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Double, null));
		}

		[Test]
		public void Test_DoubleRuleRange()
		{
			var validator = Esapi.Validator;

			// Test range
			var id = Guid.NewGuid().ToString();
			var doubleRule = new DoubleValidationRule {MinValue = 0, MaxValue = 10};
			validator.AddRule(id, doubleRule);

			Assert.IsTrue(validator.IsValid(id, "0"));
			Assert.IsTrue(validator.IsValid(id, "10"));
			Assert.IsTrue(validator.IsValid(id, "5"));
			Assert.IsFalse(validator.IsValid(id, "-1"));
			Assert.IsFalse(validator.IsValid(id, "11"));
		}

		// <summary> Test of IsValidInteger method, of class Owasp.Esapi.Validator.</summary>
		public void Test_IsValidInteger()
		{
			var validator = Esapi.Validator;
			//testing negative range
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "-4"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Integer, "-4"));
			//testing null value
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, null));
			//testing empty string
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, ""));
			//testing non-integers
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "4.3214"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "-1.65"));
			//other testing
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Integer, "4"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Integer, "400"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Integer, "400000000"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "4000000000000"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "alsdkf"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "--10"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "14.1414234x"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "Infinity"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "-Infinity"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "NaN"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "-NaN"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "+NaN"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "1e-6"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, "-1e-6"));

			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, string.Empty));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Integer, null));
		}

		[Test]
		public void Test_IntegerRuleRange()
		{
			var validator = Esapi.Validator;

			// Test range
			var id = Guid.NewGuid().ToString();
			var rule = new IntegerValidationRule {MinValue = 0, MaxValue = 10};
			validator.AddRule(id, rule);

			Assert.IsTrue(validator.IsValid(id, "0"));
			Assert.IsTrue(validator.IsValid(id, "10"));
			Assert.IsTrue(validator.IsValid(id, "5"));
			Assert.IsFalse(validator.IsValid(id, "-1"));
			Assert.IsFalse(validator.IsValid(id, "11"));
		}

		/// <summary> Test of GetValidDate method, of class Owasp.Esapi.Validator.</summary>
		[Test]
		public void Test_GetValidDate()
		{
			var validator = Esapi.Validator;

			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Date, "June 23, 1967"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Date, "Jun 23, 1967"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Date, "June 32, 1967"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Date, "June 32 1967"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Date, "June 32 abcd"));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Date, string.Empty));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Date, null));
		}


		[Test]
		public void Test_DateRuleRange()
		{
			var validator = Esapi.Validator;

			// Test range
			var now = DateTime.Now;
			var id = Guid.NewGuid().ToString();

			// NOTE : conversion to string looses precision so force a conversion otherwise 
			// validating MinValue or MaxValue would fail (see the tests below)
			var rule = new DateValidationRule
			{
				MinValue = DateTime.Parse(now.AddDays(1).ToString()),
				MaxValue = DateTime.Parse(now.AddDays(10).ToString())
			};
			validator.AddRule(id, rule);

			Assert.IsTrue(validator.IsValid(id, now.AddDays(1).ToString()));
			Assert.IsTrue(validator.IsValid(id, now.AddDays(10).ToString()));
			Assert.IsTrue(validator.IsValid(id, now.AddDays(5).ToString()));
			Assert.IsFalse(validator.IsValid(id, now.ToString()));
			Assert.IsFalse(validator.IsValid(id, now.AddDays(11).ToString()));
		}

		[Test]
		public void Test_StringRule()
		{
			var validator = Esapi.Validator;

			var id = Guid.NewGuid().ToString();
			var rule = new StringValidationRule();
			validator.AddRule(id, rule);

			// Test valid
			Assert.IsTrue(validator.IsValid(id, Guid.NewGuid().ToString()));

			// Test allow null or empty
			Assert.IsFalse(validator.IsValid(id, string.Empty));
			Assert.IsFalse(validator.IsValid(id, null));

			rule.AllowNullOrEmpty = true;
			Assert.IsTrue(validator.IsValid(id, string.Empty));
			Assert.IsTrue(validator.IsValid(id, null));

			// Test whitelist
			Assert.IsTrue(validator.IsValid(id, "abc"));
			rule.AddWhitelistPattern("\\d+");
			Assert.IsFalse(validator.IsValid(id, "abc"));
			Assert.IsTrue(validator.IsValid(id, "123"));

			// Test blacklist
			rule.AddBlacklistPattern("1");
			Assert.IsFalse(validator.IsValid(id, "123"));
			Assert.IsTrue(validator.IsValid(id, "23"));
		}

		[Test]
		public void Test_StringRuleRange()
		{
			var validator = Esapi.Validator;

			// Test range
			var id = Guid.NewGuid().ToString();
			var rule = new StringValidationRule {MinLength = 1, MaxLength = 10};
			validator.AddRule(id, rule);

			Assert.IsTrue(validator.IsValid(id, "a"));
			Assert.IsTrue(validator.IsValid(id, "1234567890"));
			Assert.IsTrue(validator.IsValid(id, "12345"));
			Assert.IsFalse(validator.IsValid(id, ""));
			Assert.IsFalse(validator.IsValid(id, "12345678901"));
		}

		/// <summary> Test of IsValidPrintable method, of class Owasp.Esapi.Validator.</summary>
		[Test]
		public void Test_IsValidPrintable()
		{
			var validator = Esapi.Validator;
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Printable, "abcDEF"));
			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Printable, "!@#R()*$;><()"));

			char[] bytes = {(char) 0x60, (char) 0xFF, (char) 0x10, (char) 0x25};
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Printable, new string(bytes)));

			Assert.IsTrue(validator.IsValid(BuiltinValidationRules.Printable, string.Empty));
			Assert.IsFalse(validator.IsValid(BuiltinValidationRules.Printable, null));
		}

		[Test]
		public void Test_AddRule()
		{
			var mocks = new MockRepository();
			var rule = mocks.StrictMock<IValidationRule>();

			var test = Guid.NewGuid().ToString();

			Esapi.Validator.AddRule(test, rule);
			Assert.AreSame(Esapi.Validator.GetRule(test), rule);
		}

		[Test]
		public void Test_RemoveRule()
		{
			var mocks = new MockRepository();
			var rule = mocks.StrictMock<IValidationRule>();

			var test = Guid.NewGuid().ToString();

			Esapi.Validator.AddRule(test, rule);
			Assert.AreSame(Esapi.Validator.GetRule(test), rule);

			Esapi.Validator.RemoveRule(test);
			Assert.IsNull(Esapi.Validator.GetRule(test));
		}

		[Test]
		public void Test_IsValid()
		{
			var mocks = new MockRepository();

			var test = Guid.NewGuid().ToString();

			var rule = mocks.StrictMock<IValidationRule>();
			Expect.Call(rule.IsValid(test)).Return(true);
			mocks.ReplayAll();

			Esapi.Validator.AddRule(test, rule);
			Assert.AreSame(Esapi.Validator.GetRule(test), rule);

			Assert.IsTrue(Esapi.Validator.IsValid(test, test));
			mocks.VerifyAll();
		}

		/// <summary>
		///     Tests loading of configuration defined validator
		/// </summary>
		[Test]
		public void Test_LoadCustom()
		{
			// Set new
			EsapiConfig.Instance.Validator.Type = typeof(SurrogateValidator).AssemblyQualifiedName;

			var validator = Esapi.Validator;
			Assert.IsTrue(validator.GetType().Equals(typeof(SurrogateValidator)));
		}

		/// <summary>
		///     Tests loading of assembly defined rules in a configuration defined
		///     validator
		/// </summary>
		[Test]
		public void Test_LoadCustomAddinAssembly()
		{
			var mocks = new MockRepository();

			// Set new
			EsapiConfig.Instance.Validator.Type = typeof(SurrogateValidator).AssemblyQualifiedName;

			// Set assemblies to load
			var addinAssembly = new AddinAssemblyElement();
			addinAssembly.Name = typeof(Esapi).Assembly.FullName;
			EsapiConfig.Instance.Validator.Rules.Assemblies.Add(addinAssembly);

			// Set mock expectations
			var mockValidator = mocks.StrictMock<IValidator>();

			// Load default
			Expect.Call(delegate { mockValidator.AddRule(BuiltinValidationRules.CreditCard, null); })
				.Constraints(Is.Equal(BuiltinValidationRules.CreditCard), Is.Anything());
			Expect.Call(delegate { mockValidator.AddRule(BuiltinValidationRules.Date, null); })
				.Constraints(Is.Equal(BuiltinValidationRules.Date), Is.Anything());
			Expect.Call(delegate { mockValidator.AddRule(BuiltinValidationRules.Double, null); })
				.Constraints(Is.Equal(BuiltinValidationRules.Double), Is.Anything());
			Expect.Call(delegate { mockValidator.AddRule(BuiltinValidationRules.Integer, null); })
				.Constraints(Is.Equal(BuiltinValidationRules.Integer), Is.Anything());
			Expect.Call(delegate { mockValidator.AddRule(BuiltinValidationRules.Printable, null); })
				.Constraints(Is.Equal(BuiltinValidationRules.Printable), Is.Anything());
			mocks.ReplayAll();

			// Create and test
			SurrogateValidator.DefaultValidator = mockValidator;
			var validator = Esapi.Validator;

			Assert.IsTrue(validator.GetType().Equals(typeof(SurrogateValidator)));
			mocks.VerifyAll();
		}

		/// <summary>
		///     Tests loading of configuration defined codecs
		/// </summary>
		[Test]
		public void Test_LoadCustomCodecs()
		{
			var mocks = new MockRepository();

			// Set new
			EsapiConfig.Instance.Validator.Type = typeof(SurrogateValidator).AssemblyQualifiedName;

			// Set rules to load
			string[] ruleNames = {Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
			foreach (var ruleName in ruleNames)
			{
				var ruleElement = new ValidationRuleElement();
				ruleElement.Name = ruleName;
				ruleElement.Type = typeof(SurrogateValidationRule).AssemblyQualifiedName;

				EsapiConfig.Instance.Validator.Rules.Add(ruleElement);
			}

			// Set mock expectations
			var mockValidator = mocks.StrictMock<IValidator>();

			// Custom rules are loaded and are of proper type
			foreach (var ruleName in ruleNames)
				Expect.Call(delegate { mockValidator.AddRule(ruleName, null); })
					.Constraints(Is.Equal(ruleName), Is.TypeOf<SurrogateValidationRule>());
			mocks.ReplayAll();

			// Create and test
			SurrogateValidator.DefaultValidator = mockValidator;
			var validator = Esapi.Validator;

			Assert.IsTrue(validator.GetType().Equals(typeof(SurrogateValidator)));
			mocks.VerifyAll();
		}
	}
}