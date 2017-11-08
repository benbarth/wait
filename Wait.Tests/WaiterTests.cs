using System;
using System.Threading;
using Waiting;
using NUnit.Framework;

namespace WaitTests
{
	[TestFixture()]
	public class WaiterTests
	{
		[Test()]
		public void ExampleUsage()
		{
			Waiter waiter = Wait.GetWaiter();

			new Thread(() =>
			{
				// Downloading
				waiter.Done();
			}).Start();

			waiter.WaitUntilDone();
			// Download is done

			Assert.Pass();
		}

		[Test()]
		public void TestWaitWithWaiter()
		{
			var waited = false;
			var waiter = Wait.GetWaiter();

			// Long running task
			new Thread(() =>
			{
				Thread.Sleep(100);
				waited = true;
				waiter.Done();
			}).Start();

			Assert.False(waited);
			waiter.WaitUntilDone();
			Assert.True(waited);
		}

		[Test()]
		public void TestWaiterWithMultipleDones()
		{
			var waited = false;
			var waiter = Wait.GetWaiter();

			// Long running task
			new Thread(() =>
			{
				Assert.Throws<System.Threading.SemaphoreFullException>(() =>
				{
					Thread.Sleep(100);
					waited = true;
					waiter.Done();
					waiter.Done();
				});
			}).Start();

			Assert.False(waited);
			waiter.WaitUntilDone();
			Assert.True(waited);
		}

	}
}
