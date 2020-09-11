using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;
using CreditCards.UITests.Pages;
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
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);

			driver.Manage().Window.Minimize();
			driver.Manage().Window.Size = new Size(300, 400);
			driver.Manage().Window.Position = new Point(1, 1);
			driver.Manage().Window.Position = new Point(50, 50);
			driver.Manage().Window.Position = new Point(100, 100);
			driver.Manage().Window.FullScreen();
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void ReloadHomePage()
		{
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);
			driver.Navigate().Refresh();
			homePage.EnsurePageLoaded();
		}

		[Fact]
		[Trait("Category", "Smoke")]
		public void ReloadHomePageOnBack()
		{
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);

			string initialToken = homePage.GenerationToken;

			driver.Url = AboutUrl;
			driver.Navigate().Back();

			homePage.EnsurePageLoaded();
			string reloadedToken = homePage.GenerationToken;

			Assert.NotEqual(initialToken, reloadedToken);
		}

		[Fact]
		public void DisplayProductsAndRates()
		{
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);

			Assert.Equal("Easy Credit Card", homePage.Products[0].name);
			Assert.Equal("20% APR", homePage.Products[0].interestRate);

			Assert.Equal("Silver Credit Card", homePage.Products[1].name);
			Assert.Equal("18% APR", homePage.Products[1].interestRate);

			Assert.Equal("Gold Credit Card", homePage.Products[2].name);
			Assert.Equal("17% APR", homePage.Products[2].interestRate);
		}

		[Fact]
		public void OpenContactFooterLinkInNewTab()
		{
			using var driver = new ChromeDriver(path, options);			
			var homePage = new HomePage(driver);
			homePage.ClickContactFooterLink();

			var allTabs = driver.WindowHandles;
			var homePageTab = allTabs[0];
			var contactTab = allTabs[1];
			driver.SwitchTo().Window(contactTab);

			Assert.EndsWith("/Home/Contact", driver.Url);
		}

		[Fact]
		public void AlertIfLiveChatClosed()
		{
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);
			homePage.ClickLiveChatLink();		

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
			wait.Until(ExpectedConditions.AlertIsPresent());

			var alert = driver.SwitchTo().Alert();
			Assert.Equal("Live chat is currently closed.", alert.Text);

			alert.Accept();
		}

		[Fact]
		public void NavigateToAboutUsWhenOKClicked()
		{
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);
			homePage.ClickLearnAboutUsLink();

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
			wait.Until(ExpectedConditions.AlertIsPresent());

			var alert = driver.SwitchTo().Alert();
			alert.Accept();
			Assert.EndsWith("/Home/About", driver.Url);
		}

		[Fact]
		public void NotNavigateToAboutUsWhenCancelClicked()
		{
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);
			homePage.ClickLearnAboutUsLink();

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
			var alert = wait.Until(ExpectedConditions.AlertIsPresent());
			alert.Dismiss();

			homePage.EnsurePageLoaded();
		}

		[Fact]
		public void NotDisplayCookieUseMessage()
		{
			//we must load the page first before manipulating cookies
			using var driver = new ChromeDriver(path, options);
			var homePage = new HomePage(driver);
			
			driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
			driver.Navigate().Refresh();
			
			Assert.False(homePage.CookieMessageIsPresent);

			driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
			driver.Navigate().Refresh();
			
			Assert.True(homePage.CookieMessageIsPresent);
		}

		[Fact]
		[UseReporter(typeof(BeyondCompareReporter))]
		public void RenderAboutPage()
		{
			var fileName = "aboutpage.png";

			using var driver = new ChromeDriver(path, options);
			var aboutPage = new AboutPage(driver);
			
			var screenShot = (Screenshot)((ITakesScreenshot)driver).GetScreenshot();
			screenShot.SaveAsFile(fileName, ScreenshotImageFormat.Png);

			FileInfo file = new FileInfo(fileName);
			Approvals.Verify(file);
		}
	}
}
