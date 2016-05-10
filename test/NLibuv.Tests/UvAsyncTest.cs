using System;
using System.Threading;
using Xunit;

namespace NLibuv.Tests
{
	public class UvAsyncTest
	{
		[Fact]
		public void AsyncCallbackTet()
		{
			var callbackCallCount = 0;
			var loop = new UvLoop();
			var async = new UvAsync(loop, thisAsync =>
			{
				callbackCallCount++;
				thisAsync.Close();
			});

			Assert.Equal(0, callbackCallCount);

			loop.Run(UvLoopRunMode.RunNowait);
			Assert.Equal(0, callbackCallCount);

			new Thread(() => { async.Send(); }).Start();
			Assert.Equal(0, callbackCallCount);

			loop.Run();
			loop.Close();

			Assert.Equal(1, callbackCallCount);
		}

		[Fact]
		public void ArgumentNullExceptionTest()
		{
			var loop = new UvLoop();

			Assert.Throws<ArgumentNullException>(() => new UvAsync(null, async => { }));
			Assert.Throws<ArgumentNullException>(() => new UvAsync(loop, null));

			loop.Close();
		}
	}
}