using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace PlayTogether.Tests.Pages
{
    public class SearchForGamersPage
    {
        public IWebDriver WebDriver { get; }

        public IWebElement SearchForGamersLink => WebDriver.FindElement(By.LinkText("Search For Gamers"));

        public IWebElement GamerSearchBar => WebDriver.FindElement(By.Id("gamerSearchBar"));


        public SearchForGamersPage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public bool SearchForGamersLinkVisible()
        {
            return SearchForGamersLink.Displayed;
        }

        public void ClickSearchForGamers()
        {
            SearchForGamersLink.Click();
        }

        public void SearchWithCriteria(Table searchCriteria)
        {
            dynamic search = searchCriteria.CreateDynamicInstance();
            GamerSearchBar.SendKeys(search.SearchCriteria);
            GamerSearchBar.SendKeys("\n");
        }

        public bool DidReceivedFeedback()
        {
            var receivedResult = WebDriver.FindElements(By.ClassName("col-lg-4"))?.Count > 0;

            if (receivedResult)
                return receivedResult;

            var receivedNoResultsNotice = WebDriver.FindElement(By.Id("noResultsNotice")).Displayed;
            return receivedNoResultsNotice;
        }
    }
}
