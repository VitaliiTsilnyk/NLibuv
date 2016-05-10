using System;

namespace NLibuv
{
	public interface IUvHandle
	{
		/// <summary>
		/// Gets the type of the current handle.
		/// </summary>
		UvHandleType HandleType { get; }

		/// <summary>
		/// Gets the loop object, on which the handle was initialized.
		/// </summary>
		UvLoop Loop { get; }

		/// <summary>
		/// Properly closes the handle and ensures that current object will be disposed correctly.
		/// </summary>
		void Close();
	}
}