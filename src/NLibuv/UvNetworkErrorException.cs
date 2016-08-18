using System;
using System.Net.Sockets;

namespace NLibuv
{
    public class UvNetworkErrorException : UvErrorException
    {
		/// <summary>
		/// Gets the mapped SocketError value for the current network error.
		/// </summary>
		public SocketError SocketError { get; }

		public UvNetworkErrorException(int statusCode, UvErrorCode errorCode, string errorName, string errorDescription)
			: base(statusCode, errorCode, errorName, errorDescription)
		{
			var socketError = Map(errorCode);
			if (socketError == 0)
			{
				throw new ArgumentException($"Error code is not supported by {nameof(UvNetworkErrorException)}.", nameof(errorCode));
			}
			this.SocketError = socketError;
		}

		/// <summary>
		/// Checks whenever <see cref="UvNetworkErrorException"/> supports given error code.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <returns></returns>
		public static bool IsNetworkError(UvErrorCode errorCode)
		{
			return Map(errorCode) != SocketError.SocketError;
		}

		/// <summary>
		/// Maps <see cref="UvErrorCode"/> to corresponding standard .NET's <see cref="SocketError"/> code.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <returns>Corresponding <see cref="SocketError"/> or <see cref="SocketError.SocketError"/> if mapping failed.</returns>
		public static SocketError Map(UvErrorCode errorCode)
		{
			switch (errorCode)
			{
				case UvErrorCode.EADDRINUSE:       return SocketError.AddressAlreadyInUse;
				case UvErrorCode.EADDRNOTAVAIL:    return SocketError.AddressNotAvailable;
				case UvErrorCode.EAFNOSUPPORT:     return SocketError.AddressFamilyNotSupported;
				case UvErrorCode.EAGAIN:           return SocketError.WouldBlock;
				case UvErrorCode.EALREADY:         return SocketError.AlreadyInProgress;
				case UvErrorCode.ECONNABORTED:     return SocketError.ConnectionAborted;
				case UvErrorCode.ECONNREFUSED:     return SocketError.ConnectionRefused;
				case UvErrorCode.ECONNRESET:       return SocketError.ConnectionReset;
				case UvErrorCode.EDESTADDRREQ:     return SocketError.DestinationAddressRequired;
				case UvErrorCode.EFAULT:           return SocketError.Fault;
				case UvErrorCode.EHOSTUNREACH:     return SocketError.HostUnreachable;
				case UvErrorCode.EISCONN:          return SocketError.IsConnected;
				case UvErrorCode.EMFILE:           return SocketError.TooManyOpenSockets;
				case UvErrorCode.EMSGSIZE:         return SocketError.MessageSize;
				case UvErrorCode.ENETDOWN:         return SocketError.NetworkDown;
				case UvErrorCode.ENETUNREACH:      return SocketError.NetworkUnreachable;
				case UvErrorCode.ENOBUFS:          return SocketError.NoBufferSpaceAvailable;
				case UvErrorCode.ENOPROTOOPT:      return SocketError.ProtocolOption;
				case UvErrorCode.ENOTCONN:         return SocketError.NotConnected;
				case UvErrorCode.EPROTO:           return SocketError.ProtocolNotSupported;
				case UvErrorCode.EPROTONOSUPPORT:  return SocketError.ProtocolNotSupported;
				case UvErrorCode.EPROTOTYPE:       return SocketError.ProtocolType;
				case UvErrorCode.ESHUTDOWN:        return SocketError.Shutdown;
				case UvErrorCode.ETIMEDOUT:        return SocketError.TimedOut;

				default:
					return SocketError.SocketError;
			}
		}

		/// <summary>
		/// Converts the <see cref="UvNetworkErrorException"/> into <see cref="SocketException"/>.
		/// </summary>
		/// <returns></returns>
		public SocketException ToSocketException()
		{
			return new SocketException((int)this.SocketError);
		}
	}
}
