﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Owasp.Esapi.Configuration;
using Owasp.Esapi.Interfaces;

namespace Owasp.Esapi
{
	internal class EsapiLoader
	{
		#region AccessController

		/// <summary>
		///     Load access controller
		/// </summary>
		/// <param name="controllerConfig">Access controller configuration element</param>
		/// <returns></returns>
		internal static IAccessController LoadAccessController(AccessControllerElement controllerConfig)
		{
			Debug.Assert(controllerConfig != null);

			if (!string.IsNullOrEmpty(controllerConfig.Type))
				return ObjectBuilder.Build<IAccessController>(controllerConfig.Type);

			return new AccessController();
		}

		#endregion

		#region Encryptor

		/// <summary>
		///     Load encryptor instance
		/// </summary>
		/// <param name="encryptorConfig"></param>
		/// <returns></returns>
		internal static IEncryptor LoadEncryptor(EncryptorElement encryptorConfig)
		{
			Debug.Assert(encryptorConfig != null);

			if (!string.IsNullOrEmpty(encryptorConfig.Type))
				return ObjectBuilder.Build<IEncryptor>(encryptorConfig.Type);

			// Default
			return new Encryptor();
		}

		#endregion

		#region IntrusionDetector

		/// <summary>
		///     Load instrusion detector instance
		/// </summary>
		/// <param name="detectorConfig"></param>
		/// <returns></returns>
		internal static IIntrusionDetector LoadIntrusionDetector(IntrusionDetectorElement detectorConfig)
		{
			Debug.Assert(detectorConfig != null);

			IIntrusionDetector detector = null;
			if (!string.IsNullOrEmpty(detectorConfig.Type))
				detector = ObjectBuilder.Build<IIntrusionDetector>(detectorConfig.Type);
			else // Create default 
				detector = new IntrusionDetector();

			// Load event thresholds
			foreach (ThresholdElement e in detectorConfig.EventThresholds)
			{
				var actions = e.Actions.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

				var threshold = new Threshold(e.Name, e.Count, e.Interval, actions);
				detector.AddThreshold(threshold);
			}

			return detector;
		}

		#endregion

		#region HttpUtilities

		/// <summary>
		///     Load HTTP utilities
		/// </summary>
		/// <param name="utilitiesConfig"></param>
		/// <returns></returns>
		internal static IHttpUtilities LoadHttpUtilities(HttpUtilitiesElement utilitiesConfig)
		{
			Debug.Assert(utilitiesConfig != null);

			if (!string.IsNullOrEmpty(utilitiesConfig.Type))
				return ObjectBuilder.Build<IHttpUtilities>(utilitiesConfig.Type);

			// Default
			return new HttpUtilities();
		}

		#endregion

		#region Randomizer

		/// <summary>
		///     Load randomizer instance
		/// </summary>
		/// <param name="randomizerConfig"></param>
		/// <returns></returns>
		internal static IRandomizer LoadRandomizer(RandomizerElement randomizerConfig)
		{
			Debug.Assert(randomizerConfig != null);

			if (!string.IsNullOrEmpty(randomizerConfig.Type))
				return ObjectBuilder.Build<IRandomizer>(randomizerConfig.Type);

			// Default
			return new Randomizer();
		}

		#endregion

		#region Security Configuration

		/// <summary>
		///     Load security configuration instance
		/// </summary>
		/// <param name="securityConfig"></param>
		/// <returns></returns>
		internal static ISecurityConfiguration LoadSecurityConfiguration(SecurityConfigurationElement securityConfig)
		{
			Debug.Assert(securityConfig != null);

			// Custom configuration
			if (!string.IsNullOrEmpty(securityConfig.Type))
				return ObjectBuilder.Build<ISecurityConfiguration>(securityConfig.Type);

			// Default
			return new SecurityConfiguration(securityConfig);
		}

		#endregion

		#region Encoder

		/// <summary>
		///     Load codec instance
		/// </summary>
		/// <param name="encoder">Encoder instance</param>
		/// <param name="codec">Codec type</param>
		/// <returns></returns>
		static bool LoadCodec(IEncoder encoder, Type codec)
		{
			Debug.Assert(encoder != null);
			Debug.Assert(codec != null);

			var loaded = false;

			var attrs = codec.GetCustomAttributes(typeof(CodecAttribute), false);
			if (attrs != null && attrs.Length > 0)
			{
				var codecAttr = (CodecAttribute) attrs[0];

				if (codecAttr.AutoLoad)
				{
					encoder.AddCodec(codecAttr.Name, ObjectBuilder.Build<ICodec>(codec));
					loaded = true;
				}
			}

			return loaded;
		}

		/// <summary>
		///     Load codecs from assembly
		/// </summary>
		/// <param name="encoder"></param>
		/// <param name="assembly"></param>
		/// <param name="typeMatch"></param>
		static void LoadCodecs(IEncoder encoder, Assembly assembly, Regex typeMatch)
		{
			Debug.Assert(encoder != null);
			Debug.Assert(assembly != null);
			Debug.Assert(typeMatch != null);

			foreach (var type in assembly.GetTypes())
				if (typeMatch.IsMatch(type.FullName))
					LoadCodec(encoder, type);
		}

		/// <summary>
		///     Load encoder element
		/// </summary>
		/// <param name="encoderConfig"></param>
		/// <returns></returns>
		internal static IEncoder LoadEncoder(EncoderElement encoderConfig)
		{
			Debug.Assert(encoderConfig != null);

			// Create encoder
			IEncoder encoder = null;

			if (!string.IsNullOrEmpty(encoderConfig.Type))
			{
				encoder = ObjectBuilder.Build<IEncoder>(encoderConfig.Type);
			}
			else
			{
				// Create default encoder and load all local codec defs
				encoder = new Encoder();
				LoadCodecs(encoder, typeof(Encoder).Assembly, MatchHelper.WildcardToRegex(@"Owasp.Esapi.Codecs.*"));
			}

			var codecs = encoderConfig.Codecs;

			// Load codec assemblies
			foreach (AddinAssemblyElement codecAssembly in codecs.Assemblies)
				try
				{
					var assembly = Assembly.Load(codecAssembly.Name);
					var typeMatch = MatchHelper.WildcardToRegex(codecAssembly.Types);

					LoadCodecs(encoder, assembly, typeMatch);
				}
				catch (Exception exp)
				{
					Esapi.Logger.Warning(LogLevels.WARN, "Failed to load codec assembly", exp);
				}

			// Specific codecs
			foreach (CodecElement codecElement in codecs)
			{
				var failMessage = string.Format("Failed to load codec \"{0}\"", codecElement.Name);

				try
				{
					var codec = AddinBuilder<ICodec>.MakeInstance(codecElement);
					encoder.AddCodec(codecElement.Name, codec);
				}
				catch (Exception exp)
				{
					Esapi.Logger.Warning(LogLevels.WARN, failMessage, exp);
				}
			}

			return encoder;
		}

		#endregion

		#region Validator

		/// <summary>
		///     Load validation rule
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="ruleType"></param>
		/// <returns></returns>
		static bool LoadValidationRule(IValidator validator, Type ruleType)
		{
			if (ruleType == null) throw new ArgumentNullException("ruleType");
			if (validator == null) throw new ArgumentNullException("validator");

			var loaded = false;

			var attrs = ruleType.GetCustomAttributes(typeof(ValidationRuleAttribute), false);
			if (attrs != null && attrs.Length > 0)
			{
				var ruleAttr = (ValidationRuleAttribute) attrs[0];

				if (ruleAttr.AutoLoad)
				{
					validator.AddRule(ruleAttr.Name, ObjectBuilder.Build<IValidationRule>(ruleType));
					loaded = true;
				}
			}

			return loaded;
		}

		/// <summary>
		///     Load assembly defined validation rules
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="assembly"></param>
		/// <param name="typeMatch"></param>
		static void LoadValidationRules(IValidator validator, Assembly assembly, Regex typeMatch)
		{
			Debug.Assert(validator != null);
			Debug.Assert(assembly != null);
			Debug.Assert(typeMatch != null);

			foreach (var type in assembly.GetTypes())
				if (typeMatch.IsMatch(type.FullName))
					LoadValidationRule(validator, type);
		}

		/// <summary>
		///     Load validator instance
		/// </summary>
		/// <param name="validatorConfig"></param>
		/// <returns></returns>
		internal static IValidator LoadValidator(ValidatorElement validatorConfig)
		{
			Debug.Assert(validatorConfig != null);

			IValidator validator = null;

			// Create custom
			if (!string.IsNullOrEmpty(validatorConfig.Type))
			{
				validator = ObjectBuilder.Build<IValidator>(validatorConfig.Type);
			}
			else
			{
				// Create default and load local rules
				validator = new Validator();
				LoadValidationRules(validator, typeof(Validator).Assembly,
					MatchHelper.WildcardToRegex(@"Owasp.Esapi.ValidationRules.*"));
			}

			var rules = validatorConfig.Rules;

			// Add rule assemblies
			foreach (AddinAssemblyElement ruleAssembly in rules.Assemblies)
				try
				{
					var assembly = Assembly.Load(ruleAssembly.Name);
					var typeMatch = MatchHelper.WildcardToRegex(ruleAssembly.Types);

					LoadValidationRules(validator, assembly, typeMatch);
				}
				catch (Exception exp)
				{
					Esapi.Logger.Warning(LogLevels.WARN, "Failed to load validation rule assembly", exp);
				}

			// Rules
			foreach (ValidationRuleElement ruleElement in rules)
			{
				var failMessage = string.Format("Failed to load validation rule \"{0}\"", ruleElement.Name);

				try
				{
					var rule = AddinBuilder<IValidationRule>.MakeInstance(ruleElement);
					validator.AddRule(ruleElement.Name, rule);
				}
				catch (Exception exp)
				{
					Esapi.Logger.Warning(LogLevels.WARN, failMessage, exp);
				}
			}

			return validator;
		}

		#endregion
	}
}