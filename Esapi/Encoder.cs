using System;
using System.Collections.Generic;
using Owasp.Esapi.Errors;
using Owasp.Esapi.Interfaces;
using EM = Owasp.Esapi.Resources.Errors;

namespace Owasp.Esapi
{
	/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder" />
	/// <summary>
	///     Reference implementation of the IEncoder interface.
	/// </summary>
	public class Encoder : IEncoder
	{
		readonly ILogger logger = Esapi.Logger;
		readonly Dictionary<string, ICodec> codecs = new Dictionary<string, ICodec>();

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.Canonicalize(string, bool)" />
		public string Canonicalize(string input, bool strict)
		{
			return Canonicalize(codecs.Keys, input, strict);
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.Canonicalize(IEnumerable&lt;string&gt;, string, bool)" />
		public string Canonicalize(IEnumerable<string> codecNames, string input, bool strict)
		{
			if (codecNames == null) throw new ArgumentNullException("codecNames");
			if (string.IsNullOrEmpty(input)) return input;

			var working = input;
			ICodec codecFound = null;
			var mixedCount = 1;
			var foundCount = 0;
			var clean = false;
			while (!clean)
			{
				clean = true;
				// try each codec and keep track of which ones work             
				foreach (var codecName in codecNames)
				{
					if (string.IsNullOrEmpty(codecName)) continue;

					var old = working;
					var codec = codecs[codecName];
					working = codec.Decode(working);
					if (!old.Equals(working))
					{
						if (codecFound != null && codecFound != codec) mixedCount++;
						codecFound = codec;
						if (clean) foundCount++;
						clean = false;
					}
				}
			}
			// do strict tests and handle if any mixed, multiple, nested encoding were found 
			if (foundCount >= 2 && mixedCount > 1)
			{
				if (strict)
					throw new IntrusionException(EM.Encoder_InputValidationFailure,
						string.Format(EM.Encoder_MultipleMixedEncoding2, foundCount, mixedCount));
				logger.Warning(LogEventTypes.SECURITY,
					string.Format(EM.Encoder_MultipleMixedEncoding2, foundCount, mixedCount));
			}
			else if (foundCount >= 2)
			{
				if (strict)
					throw new IntrusionException(EM.Encoder_InputValidationFailure,
						string.Format(EM.Encoder_MultipleEncoding1, foundCount));
				logger.Warning(LogEventTypes.SECURITY, string.Format(EM.Encoder_MultipleEncoding1, foundCount));
			}
			else if (mixedCount > 1)
			{
				if (strict)
					throw new IntrusionException(EM.Encoder_InputValidationFailure,
						string.Format(EM.Encoder_MixedEncoding1, mixedCount));
				logger.Warning(LogEventTypes.SECURITY, string.Format(EM.Encoder_MixedEncoding1, mixedCount));
			}
			return working;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.Normalize(string)" />
		public string Normalize(string input)
		{
			return input.Normalize();
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.Encode(string, string)" />
		public string Encode(string codecName, string input)
		{
			var codec = GetCodec(codecName);
			if (codec == null) throw new ArgumentOutOfRangeException("codecName");

			return codec.Encode(input);
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.Decode(string, string)" />
		public string Decode(string codecName, string input)
		{
			var codec = GetCodec(codecName);
			if (codec == null) throw new ArgumentOutOfRangeException("codecName");

			return codec.Decode(input);
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.GetCodec(string)" />
		public ICodec GetCodec(string codecName)
		{
			if (codecName == null) throw new ArgumentNullException("codecName");

			ICodec codec;
			codecs.TryGetValue(codecName, out codec);
			return codec;
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.AddCodec(string, ICodec)" />
		public void AddCodec(string codecName, ICodec codec)
		{
			if (codecName == null) throw new ArgumentNullException("codecName");
			codecs.Add(codecName, codec);
		}

		/// <inheritdoc cref="Owasp.Esapi.Interfaces.IEncoder.RemoveCodec(string)" />
		public void RemoveCodec(string codecName)
		{
			codecs.Remove(codecName);
		}
	}
}