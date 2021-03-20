using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using PlayTogether.Tests.Pages;
using NUnit.Framework;
using RestSharp;

namespace PlayTogether.Tests.Steps
{
    [Binding]
    [Scope(Feature = "Create Profile")]
    public sealed class CreateProfileSteps 
    {
        private readonly IWebDriver webDriver;
        private readonly CreateProfilePage createProfilePage;

        private string PlayTogetherBaseUrl;

        public CreateProfileSteps()
        {
            webDriver = new ChromeDriver();
            webDriver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);

            createProfilePage = new CreateProfilePage(webDriver);
        }

        [BeforeScenario]
        public void SetBaseUrl()
        {
            PlayTogetherBaseUrl = Environment.GetEnvironmentVariable("PlayTogetherUrl");
        }

        [Given(@"I access the application")]
        public void WhenIAccessTheApplication()
        {
            webDriver.Navigate().GoToUrl(PlayTogetherBaseUrl);
        }

        [When(@"I click register link")]
        public void WhenIClickRegisterLink()
        {
            createProfilePage.ClickRegister();
        }

        [When(@"I submit the required information")]
        public void WhenISubmitTheRequiredInformation(Table registerInfotable)
        {
            createProfilePage.SubmitRequiredInformation(registerInfotable);
        }

        [Then(@"I have the ability to create a profile")]
        public void ThenIHaveTheAbilityToCreateAProfile()
        {
            bool profileCreated = createProfilePage.IsProfileCreated();
            Assert.IsTrue(profileCreated);
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
