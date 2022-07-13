using System.Collections;
using System.Security.Principal;

namespace Owasp.Esapi.Interfaces
{
	/// <summary> Configuration Structures to direct the behavior of the ESAPI implementation. </summary>
	public interface ISecurityConfiguration
	{
		/// <summary> The master password </summary>
		string MasterPassword { get; }

		/// <summary> The master salt </summary>
		byte[] MasterSalt { get; }

		/// <summary> The allowed file extensions </summary>
		IList AllowedFileExtensions { get; }

		/// <summary> The allowed file upload size </summary>
		int AllowedFileUploadSize { get; }

		/// <summary> Name of the encryption algorithm </summary>
		string EncryptionAlgorithm { get; }

		/// <summary> Name of the hashing algorithm </summary>
		string HashAlgorithm { get; }

		/// <summary> The character encoding </summary>
		string CharacterEncoding { get; }

		/// <summary> Name of the digital signature algorithm </summary>
		string DigitalSignatureAlgorithm { get; }

		/// <summary> Name of the random number generation algorithm. </summary>
		string RandomAlgorithm { get; }

		/// <summary> The log level to use for logging. </summary>
		int LogLevel { get; }

		/// <summary> Whether or not HTML encoding is required in the log file. </summary>
		bool LogEncodingRequired { get; }

		/// <summary> Current user </summary>
		IPrincipal CurrentUser { get; }
	}
}