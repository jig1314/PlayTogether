using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PlayTogether.Tests.Pages;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace PlayTogether.Tests.Steps
{
    [Binding]
    [Scope(Feature = "Search For Gamers")]
    public sealed class SearchForGamersSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly IWebDriver webDriver;
        private readonly CreateProfilePage createProfilePage;
        private readonly SearchForGamersPage searchForGamersPage;

        private string PlayTogetherBaseUrl;

        public SearchForGamersSteps()
        {
            webDriver = new ChromeDriver();
            webDriver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 30);

            createProfilePage = new CreateProfilePage(webDriver);
            searchForGamersPage = new SearchForGamersPage(webDriver);
        }

        [BeforeScenario]
        public void SetBaseUrl()
        {
            PlayTogetherBaseUrl = Environment.GetEnvironmentVariable("PlayTogetherUrl");
        }

        [Given(@"I have created a profile")]
        public void GivenIHaveCreatedAProfile(Table registerInfotable)
        {
            webDriver.Navigate().GoToUrl(PlayTogetherBaseUrl);
            createProfilePage.ClickRegister();
            createProfilePage.SubmitRequiredInformation(registerInfotable);
        }

        [Given(@"I have logged in")]
        public void GivenIHaveLoggedIn()
        {
            bool searchForGamersLinkVisible = searchForGamersPage.SearchForGamersLinkVisible();
            Assert.IsTrue(searchForGamersLinkVisible);
        }

        [When(@"I am on the gamer search page")]
        public void WhenIAmOnTheGamerSearchPage()
        {
            searchForGamersPage.ClickSearchForGamers();
        }

        [Then(@"I can filter for gamers by information based on their profile")]
        public void ThenICanFilterForGamersByInformationBasedOnTheirProfile(Table searchCriteria)
        {
            searchForGamersPage.SearchWithCriteria(searchCriteria);
        }

        [Then(@"I will receive feedback")]
        public void ThenIWillReceiveFeedback()
        {
            bool receivedFeedback = searchForGamersPage.DidReceivedFeedback();
            Assert.IsTrue(receivedFeedback);
        }

        [AfterScenario]
        public void Dispose()
        {
            var client = new RestClient(PlayTogetherBaseUrl);
            var request = new RestRequest("api/user/delete/testName", Method.DELETE);
            client.Execute(request);

            webDriver.Dispose();
        }
    }
}
