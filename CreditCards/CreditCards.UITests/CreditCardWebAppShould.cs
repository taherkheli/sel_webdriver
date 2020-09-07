using OpenQA.Selenium;
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
			string initialToken = driver.FindElement(By.Id("GenerationToken")).Text;
			driver.Url = AboutUrl;
			Helper.Pause();
			driver.Navigate().Back(); 
			Helper.Pause();
			string reloadedToken = driver.FindElement(By.Id("GenerationToken")).Text;

			Assert.Equal(HomeTitle, driver.Title);
			Assert.Equal(HomeUrl, driver.Url);
			Assert.NotEqual(initialToken, reloadedToken);
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
			//Helper.Pause();

			driver.Url = HomeUrl;
			//Helper.Pause();
			string initialToken = driver.FindElement(By.Id("GenerationToken")).Text;

			driver.Navigate().Back();
			//Helper.Pause();

			driver.Navigate().Forward();
			//Helper.Pause();
			string reloadedToken = driver.FindElement(By.Id("GenerationToken")).Text;

			Assert.Equal(HomeTitle, driver.Title);
			Assert.Equal(HomeUrl, driver.Url);
			Assert.NotEqual(initialToken, reloadedToken);
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void DisplayProductsAndRates()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			var firstTableCell = driver.FindElement(By.TagName("td"));
			var firstProduct = firstTableCell.Text;

			Assert.Equal("Easy Credit Card", firstProduct);

			//TODO: Check rest of the table
		}
	}
}
