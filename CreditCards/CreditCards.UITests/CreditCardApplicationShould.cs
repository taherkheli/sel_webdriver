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
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito", "--start-maximized");
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

			//absolute XPATH -> tied to html structure
			//driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a")).Click();  

			//relative XPATH -> retrieved via http://xpather.com/ -> less brittle -> when nothing else can be used for locating an element
			driver.FindElement(By.XPath("//a[text()[contains(.,'- Apply Now!')]]")).Click();

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}

		[Fact]
		public void BeSubmittedWhenValid()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = ApplyUrl
			};

			driver.FindElement(By.Id("FirstName")).SendKeys("Dudda");
			driver.FindElement(By.Id("LastName")).SendKeys("Bayrozgaar");
			driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys("12345-A");
			driver.FindElement(By.Id("Age")).SendKeys("18");
			driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("50000");

			var rb = driver.FindElement(By.Id("Single"));
			if (!rb.Selected)
				rb.Click();

			var businessSource = new SelectElement(driver.FindElement(By.Id("BusinessSource")));

			//Check default selection is correct
			Assert.Equal("I'd Rather Not Say", businessSource.SelectedOption.Text);

			//Get all available options
			foreach (var option in businessSource.Options)
				output.WriteLine($"Value: '{option.GetAttribute("value")}' Text:'{option.Text}'");

			Assert.Equal(5, businessSource.Options.Count);

			//Select an option
			businessSource.SelectByValue("Email");
			Helper.Pause(500);
			businessSource.SelectByText("Internet Search");
			Helper.Pause(500);
			businessSource.SelectByIndex(4);

			var cb = driver.FindElement(By.Id("TermsAccepted"));
			if (!cb.Selected)
				cb.Click();

			//Method1 submit button
			//driver.FindElement(By.Id("SubmitApplication")).Click();
			//Method2 calling submit on an element submits the form this element is contained in
			rb.Submit();

			Assert.StartsWith("Application Complete", driver.Title);
			Assert.Equal("ReferredToHuman", driver.FindElement(By.Id("Decision")).Text);
			Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
			Assert.Equal("Dudda Bayrozgaar", driver.FindElement(By.Id("FullName")).Text);
			Assert.Equal("18", driver.FindElement(By.Id("Age")).Text);
			Assert.Equal("50000", driver.FindElement(By.Id("Income")).Text);
			Assert.Equal("Single", driver.FindElement(By.Id("RelationshipStatus")).Text);
			Assert.Equal("TV", driver.FindElement(By.Id("BusinessSource")).Text);
		}

		[Fact]
		public void BeSubmittedWhenValidationErrorsCorrected()
		{
			const string firstName = "Sarah";
			const string invalidAge = "17";
			const string validAge = "18";

			using var driver = new ChromeDriver(path, options)
			{
				Url = ApplyUrl
			};

			driver.FindElement(By.Id("FirstName")).SendKeys(firstName);
			//Don't enter lastname
			driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys("123456-A");
			driver.FindElement(By.Id("Age")).SendKeys(invalidAge);
			driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("50000");
			driver.FindElement(By.Id("Single")).Click();
			var businessSource = new SelectElement(driver.FindElement(By.Id("BusinessSource")));
			businessSource.SelectByValue("Email");
			driver.FindElement(By.Id("TermsAccepted")).Click();
			driver.FindElement(By.Id("SubmitApplication")).Click();

			//Assert that validation failed                
			var validationErrors = driver.FindElements(By.CssSelector(".validation-summary-errors > ul > li"));
			Assert.Equal(2, validationErrors.Count);
			Assert.Equal("Please provide a last name", validationErrors[0].Text);
			Assert.Equal("You must be at least 18 years old", validationErrors[1].Text);

			// Fix errors
			driver.FindElement(By.Id("LastName")).SendKeys("Smith");
			driver.FindElement(By.Id("Age")).Clear();
			driver.FindElement(By.Id("Age")).SendKeys(validAge);

			// Resubmit form
			driver.FindElement(By.Id("SubmitApplication")).Click();

			// Check form submitted
			Assert.StartsWith("Application Complete", driver.Title);
			Assert.Equal("ReferredToHuman", driver.FindElement(By.Id("Decision")).Text);
			Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
			Assert.Equal("Sarah Smith", driver.FindElement(By.Id("FullName")).Text);
			Assert.Equal("18", driver.FindElement(By.Id("Age")).Text);
			Assert.Equal("50000", driver.FindElement(By.Id("Income")).Text);
			Assert.Equal("Single", driver.FindElement(By.Id("RelationshipStatus")).Text);
			Assert.Equal("Email", driver.FindElement(By.Id("BusinessSource")).Text);
		}
	}
}
