using OpenQA.Selenium;
using System;

namespace CreditCards.UITests.Pages
{
	class AboutPage
	{
		#region Constants
		private const string Url = "http://localhost:5258/Home/About";
		private const string Title = "About - Credit Cards";
		#endregion

		private readonly IWebDriver _driver;

		public AboutPage(IWebDriver driver)
		{
			_driver = driver;
			_driver.Url = Url;
			EnsurePageLoaded();
		}

		public void EnsurePageLoaded(bool onlyCheckUrlStartsWithExpectedText = true)    //basically ignore any query string params or fragments appearing in the URL
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
