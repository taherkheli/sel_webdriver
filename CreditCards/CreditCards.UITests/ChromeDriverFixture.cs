using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace CreditCards.UITests
{
	public class ChromeDriverFixture : IDisposable
	{
		private static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		private static readonly ChromeOptions options = new ChromeOptions();

		public IWebDriver Driver { get; private set; }

		public ChromeDriverFixture()
		{
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito", "--start-maximized");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
			//if (config.Browser.IsHeadless)
			//	options.AddArguments("--headless");

			Driver = new ChromeDriver(path, options);
		}

		public void Dispose()
		{
			Driver.Dispose();
		}
	}
}
