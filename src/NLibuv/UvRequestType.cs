using System;

namespace NLibuv
{
	public enum UvRequestType
	{
		Unknown = 0,
		Req,
		Connect,
		Write,
		Shutdown,
		UdpSend,
		Fs,
		Work,
		Getaddrinfo,
		Getnameinfo,
	}
}