using OpenQA.Selenium;
using System;

namespace CreditCards.UITests.Pages
{
	class ApplicationPage
	{

		#region Constants
		private const string Url = "http://localhost:5258/Apply";
		private const string Title = "Credit Card Application - Credit Cards";
		#endregion

		private readonly IWebDriver _driver;

		public ApplicationPage(IWebDriver driver)
		{
			_driver = driver;
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
	}
}
