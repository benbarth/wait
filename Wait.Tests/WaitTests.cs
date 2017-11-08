using System;
using System.Threading;
using System.Threading.Tasks;
using Waiting;
using NUnit.Framework;

namespace WaitTests
{
	[TestFixture()]
	public class WaitTests
	{
		[Test()]
		public void ExampleUsage()
		{
			Wait.On("critical section", () =>
			{
				// Only one "critical section" will run at a time
			});

			// Imagine this running on a different thread
			Wait.On("critical section", () =>
			{
				// This might take a while
			});

			Assert.Pass();
		}

		[Test()]
		public void TestWaitWithAction()
		{
			var waited = 0;

			// Long running task
			new Thread(() =>
			{
				Wait.On("test", () =>
				{
					Thread.Sleep(300);
					Assert.AreEqual(0, waited);
					waited++;
				});
			}).Start();

			// Long running task
			new Thread(() =>
			{
				Wait.On("test", () =>
				{
					Thread.Sleep(200);
					Assert.AreEqual(1, waited);
					waited++;
				});
			}).Start();

			Thread.Sleep(100);
			Wait.On("test", () =>
			{
				Assert.AreEqual(2, waited);
			});
		}
	}
}
