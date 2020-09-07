using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Xunit;

namespace CreditCards.UITests
{
	public class CreditCardWebAppShould
	{
		private static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		[Fact]
		[Trait("Category","Smoke")]
		public void LoadApplicationPage()
		{
			var options = new ChromeOptions();
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
			//options.AddAdditionalCapability("useAutomationExtension", false);

			//if (config.Browser.IsHeadless)
			//	options.AddArguments("--headless");

			using var driver = new ChromeDriver(path, options);



		}
	}
}
