using System;

namespace NLibuv
{
	public interface IUvRequest
	{
		/// <summary>
		/// Gets the type of the current request.
		/// </summary>
		UvRequestType RequestType { get; }

		/// <summary>
		/// Gets the handle for which the current request was created.
		/// </summary>
		IUvHandle BaseHandle { get; }

		/// <summary>
		/// Properly closes the request and ensures that current object will be disposed correctly.
		/// </summary>
		void Close();
	}
}