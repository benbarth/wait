using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Waiting
{
	public static class Wait
	{
		private static readonly object _referenceCountLock = new object();
		private static readonly Dictionary<string, WaitLock> keys = new Dictionary<string, WaitLock>();

		public static Waiter GetWaiter()
		{
			return new Waiter();
		}

		public static void On(string key, Action action)
		{
			WaitLock waitLock = GetAndIncrementReference(key);

			try
			{
				lock (waitLock)
				{
					action();
				}
			}
			finally
			{
				DecrementReference(key, waitLock);
			}
		}

		private static WaitLock GetAndIncrementReference(string key)
		{
			lock (_referenceCountLock)
			{
				WaitLock waitLock = null;
				if (!keys.TryGetValue(key, out waitLock))
				{
					waitLock = new WaitLock();
					keys.Add(key, waitLock);
				}

				waitLock.NumberOfReferences++;

				return waitLock;
			}
		}

		private static void DecrementReference(string key, WaitLock waitLock)
		{
			lock (_referenceCountLock)
			{
				waitLock.NumberOfReferences--;
				if (waitLock.NumberOfReferences <= 0)
				{
					keys.Remove(key);
				}
			}
		}

		private class WaitLock
		{
			public long NumberOfReferences { get; set; }
		}
	}

	public class Waiter
	{
		private readonly Semaphore semaphore = new Semaphore(0, 1);

		public void WaitUntilDone()
		{
			semaphore.WaitOne();
		}

		public void Done()
		{
			semaphore.Release();
		}
	}

}
