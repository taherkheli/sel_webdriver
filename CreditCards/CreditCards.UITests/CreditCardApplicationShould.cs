using CreditCards.UITests.Pages;
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
	public class CreditCardApplicationShould : IClassFixture<ChromeDriverFixture>
	{
		#region Constants
		private const string ApplyUrl = "http://localhost:5258/Apply";
		private const string HomeUrl = "http://localhost:5258/";
		private const string ApplyTitle = "Credit Card Application - Credit Cards";
		#endregion

		private readonly IWebDriver _driver;

		public CreditCardApplicationShould(ChromeDriverFixture chromeDriverFixture)
		{
			_driver = chromeDriverFixture.Driver;
		}

		[Fact]
		public void BeInitiatedFromHomePage_NewLowRate()
		{
			var homePage = new HomePage(_driver);
			var appPage = homePage.ClickApplyLowRateLink();
			appPage.EnsurePageLoaded();
		}

		[Fact]
		public void BeInitiatedFromHomePage_EasyApplication()
		{
			var homePage = new HomePage(_driver);

			_driver.FindElement(By.CssSelector("[data-slide='next']")).Click(); //dun wanna wait 10 s ;)
			homePage.WaitForEasyAppCarouselPage();

			var appPage = homePage.ClickApplyEasyAppLink();
			appPage.EnsurePageLoaded();
		}

		[Fact]
		public void BeInitiatedFromHomePage_CustomerService()
		{
			var homePage = new HomePage(_driver);

			_driver.FindElement(By.CssSelector("[data-slide='next']")).Click();
			Helper.Pause(1000);
			_driver.FindElement(By.CssSelector("[data-slide='next']")).Click();

			Func<IWebDriver, IWebElement> findEnabledAndVisible = delegate (IWebDriver d)
			{
				var element = d.FindElement(By.ClassName("customer-service-apply-now"));

				if (element is null)
					throw new NotFoundException();

				if (element.Enabled && element.Displayed)
					return element;

				throw new NotFoundException();
			};

			var element = _driver.FindElement(By.ClassName("customer-service-apply-now"));  //if there are many with this classname, fetches the first
			//was coming through ITestOutputHelper output passed into ctor by xunit
			//output.WriteLine($"{DateTime.Now.ToLongTimeString()} Found element Displayed='{element.Displayed}' Enabled='{element.Enabled}'");

			var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
			//instead of using 'ExpectedConditions' functionality we can write our own custom conditions
			element = wait.Until(findEnabledAndVisible);
			//element = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));

			//output.WriteLine($"{DateTime.Now.ToLongTimeString()} Found element Displayed='{element.Displayed}' Enabled='{element.Enabled}'");
			element.Click();

			Assert.Equal(ApplyUrl, _driver.Url);
			Assert.Equal(ApplyTitle, _driver.Title);
		}

		[Fact]
		public void BeInitiatedFromHomePage_RandomGreeting()
		{
			_driver.Url = HomeUrl;
			_driver.FindElement(By.PartialLinkText("- Apply Now!")).Click();

			Assert.Equal(ApplyUrl, _driver.Url);
			Assert.Equal(ApplyTitle, _driver.Title);
		}

		[Fact]
		public void BeInitiatedFromHomePage_RandomGreeting_Using_XPATH()
		{
			_driver.Url = HomeUrl;

			//absolute XPATH -> tied to html structure
			//driver.FindElement(By.XPath("/html/body/div/div[4]/div/p/a")).Click();  

			//relative XPATH -> retrieved via http://xpather.com/ -> less brittle -> when nothing else can be used for locating an element
			_driver.FindElement(By.XPath("//a[text()[contains(.,'- Apply Now!')]]")).Click();

			Assert.Equal(ApplyUrl, _driver.Url);
			Assert.Equal(ApplyTitle, _driver.Title);
		}

		[Fact]
		public void BeSubmittedWhenValid()
		{
			const string FirstName = "Dudda";
			const string LastName = "Bayrozgaar";
			const string Number = "12345-A";
			const string Age = "18";
			const string Income = "50000";

			
			var appPage = new ApplicationPage(_driver);
			appPage.EnterFirstName(FirstName);
			appPage.EnterLastName(LastName);
			appPage.EnterFrequentFlyerNumber(Number);
			appPage.EnterAge(Age);
			appPage.EnterGrossAnnualIncome(Income);
			appPage.ChooseMaritalStatusSingle();
			appPage.ChooseBusinessSourceTV();
			appPage.AcceptTerms();
			var appCompletePage = appPage.SubmitApplication();

			appCompletePage.EnsurePageLoaded();
			Assert.Equal("ReferredToHuman", appCompletePage.Decision);
			Assert.NotEmpty(appCompletePage.ReferenceNumber);
			Assert.Equal($"{FirstName} {LastName}", appCompletePage.FullName);
			Assert.Equal(Age, appCompletePage.Age);
			Assert.Equal(Income, appCompletePage.Income);
			Assert.Equal("Single", appCompletePage.RelationshipStatus);
			Assert.Equal("TV", appCompletePage.BusinessSource);
		}

		[Fact]
		public void BeSubmittedWhenValidationErrorsCorrected()
		{
			const string FirstName = "Sarah";
			const string InvalidAge = "17";
			const string ValidAge = "18";
			
			var appPage = new ApplicationPage(_driver);

			appPage.EnterFirstName(FirstName);
			// Don't enter lastname
			appPage.EnterFrequentFlyerNumber("123456-A");
			appPage.EnterAge(InvalidAge);
			appPage.EnterGrossAnnualIncome("50000");
			appPage.ChooseMaritalStatusSingle();
			appPage.ChooseBusinessSourceTV();
			appPage.AcceptTerms();
			appPage.SubmitApplication();   //due to validation failure, null will be returned

			// Assert that validation failed                
			Assert.Equal(2, appPage.ValidationMessages.Count);
			Assert.Contains("Please provide a last name", appPage.ValidationMessages);
			Assert.Contains("You must be at least 18 years old", appPage.ValidationMessages);

			// Fix errors
			appPage.EnterLastName("Smith");
			appPage.ClearAge();
			appPage.EnterAge(ValidAge);

			// Resubmit form
			var appCompletePage = appPage.SubmitApplication();

			// Check form submitted  => not needed actually coz appCompletePage ctor calls it anyway
			appCompletePage.EnsurePageLoaded();
		}
	}
}
