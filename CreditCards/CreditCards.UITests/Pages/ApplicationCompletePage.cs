using OpenQA.Selenium;
using System;

namespace CreditCards.UITests.Pages
{
	class ApplicationCompletePage
	{
		#region Constants
		private const string Url = "http://localhost:5258/Apply";
		private const string Title = "Application Complete - Credit Cards";
		#endregion

		private readonly IWebDriver _driver;

		public ApplicationCompletePage(IWebDriver driver)
		{
			_driver = driver;
			if (driver.Url != Url)
				_driver.Url = Url;

			EnsurePageLoaded();
		}

		public void EnsurePageLoaded(bool onlyCheckUrlStartsWithExpectedText = true)
		{
			bool urlIsCorrect = false;

			if (onlyCheckUrlStartsWithExpectedText)
				urlIsCorrect = _driver.Url.StartsWith(Url);
			else
				urlIsCorrect = (_driver.Url == Url);

			var pageHasLoaded = (urlIsCorrect && (_driver.Title == Title));

			if (!pageHasLoaded)
				throw new Exception($"Failed to load page. Page URL = '{_driver.Url}' PageSource: \r\n {_driver.PageSource}");
		}

		public string Decision => _driver.FindElement(By.Id("Decision")).Text;
		public string ReferenceNumber => _driver.FindElement(By.Id("ReferenceNumber")).Text;
		public string FullName => _driver.FindElement(By.Id("FullName")).Text;
		public string Age => _driver.FindElement(By.Id("Age")).Text;
		public string Income => _driver.FindElement(By.Id("Income")).Text;
		public string RelationshipStatus => _driver.FindElement(By.Id("RelationshipStatus")).Text;
		public string BusinessSource => _driver.FindElement(By.Id("BusinessSource")).Text;
	}
}
