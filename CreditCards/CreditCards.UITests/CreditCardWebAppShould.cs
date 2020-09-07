using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;
using Xunit;

namespace CreditCards.UITests
{
	public class CreditCardWebAppShould
	{
		private static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		[Fact]
		[Trait("Category", "Smoke")]
		public void LoadApplicationPage()
		{
			var options = new ChromeOptions();
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);

			//if (config.Browser.IsHeadless)
			//	options.AddArguments("--headless");

			using var driver = new ChromeDriver(path, options)
			{
				Url = "http://localhost:5258/"
			};

			driver.Manage().Window.Maximize();
		}
	}
}
