using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CreditCards.UITests
{
	internal static class Helper
	{
		public static void Pause(int ms = 2000)
		{
			Thread.Sleep(ms);
		}
	}
}
