using System;

namespace NLibuv
{
	/// <summary>
	/// Represents an exception, raised on libuv error.
	/// </summary>
	public class UvErrorException : UvException
	{
		/// <summary>
		/// Gets the status code returned by a libuv function.
		/// </summary>
		public int StatusCode { get; }

		/// <summary>
		/// Gets the libuv error code.
		/// </summary>
		public UvErrorCode ErrorCode { get; }

		/// <summary>
		/// Gets the libuv error name.
		/// </summary>
		public string ErrorName { get; }

		/// <summary>
		/// Gets the libuv error description.
		/// </summary>
		public string ErrorDescription { get; }

		public UvErrorException(int statusCode, UvErrorCode errorCode, string errorName, string errorDescription)
			: base($"Libuv Error: {errorName} ({statusCode}) {errorDescription}")
		{
			this.StatusCode = statusCode;
			this.ErrorCode = errorCode;
			this.ErrorName = errorName;
			this.ErrorDescription = errorDescription;
		}
	}
}