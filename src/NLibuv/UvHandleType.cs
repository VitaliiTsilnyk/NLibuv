using System;

namespace NLibuv
{
	public enum UvHandleType
	{
		Unknown = 0,
		Async,
		Check,
		FsEvent,
		FsPoll,
		Handle,
		Idle,
		NamedPipe,
		Poll,
		Prepare,
		Process,
		Stream,
		Tcp,
		Timer,
		Tty,
		Udp,
		Signal,
		File,
	}
}