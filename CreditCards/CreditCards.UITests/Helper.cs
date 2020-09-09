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
