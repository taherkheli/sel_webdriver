using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Linq;

namespace CreditCards.UITests.Pages
{
	class ApplicationPage : Page
	{
		public ApplicationPage(IWebDriver driver) : base(driver)
		{
		}

		protected override string Url => "http://localhost:5258/Apply";

		protected override string Title => "Credit Card Application - Credit Cards";

		public ReadOnlyCollection<string> ValidationMessages
		{
			get
			{
				return _driver.FindElements(By.CssSelector(".validation-summary-errors > ul > li"))
											.Select(x => x.Text)
											.ToList()
											.AsReadOnly();
			}
		}

		public void EnterFirstName(string name) => _driver.FindElement(By.Id("FirstName")).SendKeys(name);
		public void EnterLastName(string name) => _driver.FindElement(By.Id("LastName")).SendKeys(name);
		public void EnterFrequentFlyerNumber(string number) => _driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys(number);
		public void EnterAge(string age) => _driver.FindElement(By.Id("Age")).SendKeys(age);
		public void EnterGrossAnnualIncome(string income) => _driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys(income);

		public void ChooseMaritalStatusSingle()
		{
			var rb = _driver.FindElement(By.Id("Single"));
			if (!rb.Selected)
				rb.Click();
		}

		public void ChooseBusinessSourceTV()
		{
			var businessSource = new SelectElement(_driver.FindElement(By.Id("BusinessSource")));
			businessSource.SelectByValue("TV");
		}

		//Method1 submit button
		//driver.FindElement(By.Id("SubmitApplication")).Click();
		//Method2 calling submit on an element submits the form this element is contained in
		//rb.Submit();
		public ApplicationCompletePage SubmitApplication()
		{ 
			_driver.FindElement(By.Id("SubmitApplication")).Click();

			//check if validation errors did not let normal flow to continue
			if (_driver.Title != "Application Complete - Credit Cards")   //quick n dirty ;) to make test pass
				return null;

			return new ApplicationCompletePage(_driver);
		}

		public void AcceptTerms()
		{
			var cb = _driver.FindElement(By.Id("TermsAccepted"));
			if (!cb.Selected)
				cb.Click();
		}

		public void ClearAge() => _driver.FindElement(By.Id("Age")).Clear();
	}
}
