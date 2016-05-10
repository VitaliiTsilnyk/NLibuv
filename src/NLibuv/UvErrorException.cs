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
		/// Gets the libuv error name.
		/// </summary>
		public string ErrorName { get; }

		public UvErrorException(int statusCode, string errorName, string errorDescription)
			: base($"Libuv Error: {errorName} ({statusCode}) {errorDescription}")
		{
			this.StatusCode = statusCode;
			this.ErrorName = errorName;
		}
	}
}