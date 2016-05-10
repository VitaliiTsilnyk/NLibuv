using System;

namespace NLibuv
{
	public class UvException : Exception
	{
		public UvException(string message) : base(message) { }
	}
}