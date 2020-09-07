using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;
using Xunit;

namespace CreditCards.UITests
{
	public class CreditCardWebAppShould
	{
		#region Constants
		private const string AboutUrl = "http://localhost:5258/Home/About";
		private const string HomeUrl = "http://localhost:5258/";
		private const string HomeTitle = "Home Page - Credit Cards";
		#endregion

		private static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		private static readonly ChromeOptions options = new ChromeOptions();

		public CreditCardWebAppShould()
		{
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
			//if (config.Browser.IsHeadless)
			//	options.AddArguments("--headless");
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void LoadApplicationPage()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			Helper.Pause();

			Assert.Equal(HomeTitle, driver.Title);
			Assert.Equal(HomeUrl, driver.Url);
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void ReloadHomePage()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			Helper.Pause();

			driver.Navigate().Refresh();

			Assert.Equal(HomeTitle, driver.Title);
			Assert.Equal(HomeUrl, driver.Url);
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void ReloadHomePageOnBack()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			Helper.Pause();
			driver.Url = AboutUrl;
			Helper.Pause();
			driver.Navigate().Back(); 
			Helper.Pause();

			Assert.Equal(HomeTitle, driver.Title);
			Assert.Equal(HomeUrl, driver.Url);

			//TODO: Assert Page was reloaded
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void ReloadHomePageOnForward()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = AboutUrl
			};

			driver.Manage().Window.Maximize();
			Helper.Pause();

			driver.Url = HomeUrl;
			Helper.Pause();

			driver.Navigate().Back();
			Helper.Pause();

			driver.Navigate().Forward();
			Helper.Pause();

			Assert.Equal(HomeTitle, driver.Title);
			Assert.Equal(HomeUrl, driver.Url);

			//TODO: Assert Page was reloaded
		}
	}
}
