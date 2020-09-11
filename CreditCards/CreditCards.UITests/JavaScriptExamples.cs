using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;
using Xunit;

namespace CreditCards.UITests
{
	public class JavaScriptExamples
	{
		#region Constants
		private const string OverlayUrl = "http://localhost:5258/jsoverlay.html";
		private const string PsUrl = "https://www.pluralsight.com/";
		private const string textToCheck = "Go to Pluralsight";
		#endregion

		private static readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		private static readonly ChromeOptions options = new ChromeOptions();

		public JavaScriptExamples()
		{
			options.AddArguments("no-sandbox", "disable-dev-shm-usage", "incognito", "--start-maximized");
			options.AddExcludedArgument("enable-automation");
			options.AddAdditionalOption("useAutomationExtension", false);
		}

		[Fact]
		public void ClickOverlayedLink()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = OverlayUrl
			};
			
			//This fails due to overlay on top preventing the click
			//driver.FindElement(By.Id("HiddenLink")).Click();

			//you can get around that by executing js directly
			var script = "document.getElementById('HiddenLink').click();";
			var js = (IJavaScriptExecutor)driver;
			js.ExecuteScript(script);

			Assert.Equal(PsUrl, driver.Url);
		}

		[Fact]
		public void GetOverlayedLinkText()
		{
			using var driver = new ChromeDriver(path, options)
			{
				Url = OverlayUrl
			};
						
			var script = "return document.getElementById('HiddenLink').innerHTML;";
			var js = (IJavaScriptExecutor)driver;
			var linkText = (string)js.ExecuteScript(script);

			Assert.Equal(textToCheck, linkText);
		}
	}
}
