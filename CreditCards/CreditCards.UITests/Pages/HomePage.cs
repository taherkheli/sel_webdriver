using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CreditCards.UITests.Pages
{
	class HomePage
	{
		#region Constants
		private const string Url = "http://localhost:5258/";
		private const string Title = "Home Page - Credit Cards";
		#endregion

		private readonly IWebDriver _driver;

		public HomePage(IWebDriver driver)
		{
			_driver = driver;
			_driver.Url = Url;
			EnsurePageLoaded();
		}

		public string GenerationToken 
		{
			get
			{				
				return _driver.FindElement(By.Id("GenerationToken")).Text;
			}
		}
		
		public bool CookieMessageIsPresent => _driver.FindElements(By.Id("CookiesBeingUsed")).Any();

		public ReadOnlyCollection<(string name, string interestRate)> Products 
		{
			get
			{
				var products = new List<(string name, string interestRate)>();
				var productCells = _driver.FindElements(By.TagName("td"));

				for (int i = 0; i < productCells.Count - 1; i+=2)
					products.Add((productCells[i].Text, productCells[i + 1].Text));

				return products.AsReadOnly();
			}
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

		public void ClickContactFooterLink() => _driver.FindElement(By.Id("ContactFooter")).Click();

		public void ClickLiveChatLink() => _driver.FindElement(By.Id("LiveChat")).Click();

		public void ClickLearnAboutUsLink() => _driver.FindElement(By.Id("LearnAboutUs")).Click();

	}
}
