using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Drawing;
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
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito", "--start-maximized");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void LoadHomePage()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Minimize();
			Helper.Pause();
			driver.Manage().Window.Size = new Size(300, 400);
			Helper.Pause();
			driver.Manage().Window.Position = new Point(1, 1);
			Helper.Pause();
			driver.Manage().Window.Position = new Point(50, 50);
			Helper.Pause();
			driver.Manage().Window.Position = new Point(100, 100);
			Helper.Pause();
			driver.Manage().Window.FullScreen();
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
			driver.Url = HomeUrl;
			string initialToken = driver.FindElement(By.Id("GenerationToken")).Text;
			driver.Navigate().Back();
			driver.Navigate().Forward();
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
			var tableCells = driver.FindElements(By.TagName("td"));

			Assert.Equal("Easy Credit Card", tableCells[0].Text);
			Assert.Equal("20% APR", tableCells[1].Text);

			Assert.Equal("Silver Credit Card", tableCells[2].Text);
			Assert.Equal("18% APR", tableCells[3].Text);

			Assert.Equal("Gold Credit Card", tableCells[4].Text);
			Assert.Equal("17% APR", tableCells[5].Text);
		}

		[Fact]
		public void OpenContactFooterLinkInNewTab()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			driver.FindElement(By.Id("ContactFooter")).Click();
			Helper.Pause();

			var allTabs = driver.WindowHandles;
			var homePageTab = allTabs[0];
			var contactTab = allTabs[1];

			driver.SwitchTo().Window(contactTab);
			Helper.Pause();

			Assert.EndsWith("/Home/Contact", driver.Url);
		}

		[Fact]
		public void AlertIfLiveChatClosed()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			driver.FindElement(By.Id("LiveChat")).Click();

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
			wait.Until(ExpectedConditions.AlertIsPresent());

			var alert = driver.SwitchTo().Alert();
			Assert.Equal("Live chat is currently closed.", alert.Text);

			Helper.Pause();
			alert.Accept();
			Helper.Pause();
		}

		[Fact]
		public void NotNavigateToAboutUsWhenCancelClicked()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			Assert.Equal(HomeTitle, driver.Title);		
			driver.FindElement(By.Id("LearnAboutUs")).Click();

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
			wait.Until(ExpectedConditions.AlertIsPresent());

			var alert = driver.SwitchTo().Alert();
			alert.Dismiss();
			Assert.Equal(HomeTitle, driver.Title);
		}

		[Fact]
		public void NotDisplayCookieUseMessage()
		{
			using var driver = new ChromeDriver(path, options);
			driver.Url = HomeUrl;   //we must load the page first before manipulating cookies 
			
			driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
			driver.Navigate().Refresh();
			driver.Manage().Window.Maximize();
			
			var elements = driver.FindElements(By.Id("CookiesBeingUsed"));
			
			Assert.Empty(elements);

			var cookieValue = driver.Manage().Cookies.GetCookieNamed("acceptedCookies");
			Assert.Equal("true", cookieValue.Value);

			driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
			driver.Navigate().Refresh();
			Assert.NotNull(driver.FindElement(By.Id("CookiesBeingUsed")));
		}

		[Fact]
		[UseReporter(typeof(BeyondCompareReporter))]
		public void RenderAboutPage()
		{
			var fileName = "aboutpage.png";

			using var driver = new ChromeDriver(path, options)
			{
				Url = AboutUrl
			};

			driver.Manage().Window.Maximize();
			var screenShotDriver = (ITakesScreenshot)driver;
			var screenShot = screenShotDriver.GetScreenshot();
			screenShot.SaveAsFile(fileName, ScreenshotImageFormat.Png);

			FileInfo file = new FileInfo(fileName);
			Approvals.Verify(file);
		}
	}
}
