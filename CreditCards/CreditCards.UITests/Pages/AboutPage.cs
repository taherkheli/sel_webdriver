using OpenQA.Selenium;

namespace CreditCards.UITests.Pages
{
	class AboutPage : Page
	{
		public AboutPage(IWebDriver driver) : base (driver)
		{
		}

		protected override string Url => "http://localhost:5258/Home/About";

		protected override string Title => "About - Credit Cards";
	}
}
