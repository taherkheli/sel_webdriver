using OpenQA.Selenium;
using System;

namespace CreditCards.UITests.Pages
{
	class Page
	{
		protected IWebDriver _driver;

		protected virtual string Url { get; }

		protected virtual string Title { get; }

		public Page(IWebDriver driver)
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
	}
}
