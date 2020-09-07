using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

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

		public CreditCardApplicationShould()
		{
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
			//if (config.Browser.IsHeadless)
			//	options.AddArguments("--headless");
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
			Helper.Pause(1000); //allow carousel to become visible

			driver.FindElement(By.LinkText("Easy: Apply Now!")).Click();

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
			Helper.Pause(1000); //allow carousel to become visible
			driver.FindElement(By.CssSelector("[data-slide='next']")).Click();
			Helper.Pause(1000); //allow carousel to become visible

			driver.FindElement(By.ClassName("customer-service-apply-now")).Click(); //if there are many with this classname, fetches the first 

			Assert.Equal(ApplyUrl, driver.Url);
			Assert.Equal(ApplyTitle, driver.Title);
		}
	}
}
