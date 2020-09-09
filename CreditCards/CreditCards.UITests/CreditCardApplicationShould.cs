using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace CreditCards.UITests
{
	[Trait("Category", "Application")]
	public class CreditCardApplicationShould
	{
		#region Constants
		private const string ApplyUrl = "http://localhost:5258/Apply";
		private const string HomeUrl = "http://localhost:5258/";
		private const string ApplyTitle = "Credit Card Application - Credit Cards";
		#endregion

		private static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		private static readonly ChromeOptions options = new ChromeOptions();

		private readonly ITestOutputHelper output;

		public CreditCardApplicationShould(ITestOutputHelper output)
		{
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
			//if (config.Browser.IsHeadless)
			//	options.AddArguments("--headless");

			this.output = output;
		}

		[Fact]
		public void BeInitiatedFromHomePage_NewLowRate()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			driver.FindElement(By.Name("ApplyLowRate")).Click();

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}

		[Fact]
		public void BeInitiatedFromHomePage_EasyApplication()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			driver.FindElement(By.CssSelector("[data-slide='next']")).Click();

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
			//TODO: make lambda work
			//var applyLink = wait.Until( (d) => d.FindElement(By.LinkText("Easy: Apply Now!")));
			var applyLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Easy: Apply Now!")));
			applyLink.Click();

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}

		[Fact]
		public void BeInitiatedFromHomePage_CustomerService()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			driver.FindElement(By.CssSelector("[data-slide='next']")).Click();
			Helper.Pause(1000);
			driver.FindElement(By.CssSelector("[data-slide='next']")).Click();

			Func<IWebDriver, IWebElement> findEnabledAndVisible = delegate (IWebDriver d)
			{
				var element = d.FindElement(By.ClassName("customer-service-apply-now"));

				if (element is null)
					throw new NotFoundException();

				if (element.Enabled && element.Displayed)
					return element;

				throw new NotFoundException();
			};

			var element = driver.FindElement(By.ClassName("customer-service-apply-now"));  //if there are many with this classname, fetches the first
			output.WriteLine($"{DateTime.Now.ToLongTimeString()} Found element Displayed='{element.Displayed}' Enabled='{element.Enabled}'");

			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
			//instead of using 'ExpectedConditions' functionality we can write our own custom conditions
			element = wait.Until(findEnabledAndVisible);
			//element = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));

			output.WriteLine($"{DateTime.Now.ToLongTimeString()} Found element Displayed='{element.Displayed}' Enabled='{element.Enabled}'");
			element.Click();

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}

		[Fact]
		public void BeInitiatedFromHomePage_RandomGreeting()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			driver.FindElement(By.PartialLinkText("- Apply Now!")).Click();

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}

		[Fact]
		public void BeInitiatedFromHomePage_RandomGreeting_Using_XPATH()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = HomeUrl
			};

			driver.Manage().Window.Maximize();
			//absolute XPATH -> tied to html structure
			//driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a")).Click();  

			//relative XPATH -> retrieved via http://xpather.com/ -> less brittle -> when nothing else can be used for locating an element
			driver.FindElement(By.XPath("//a[text()[contains(.,'- Apply Now!')]]")).Click();  

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}
	}
}
