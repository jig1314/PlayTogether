using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PlayTogether.Tests.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace PlayTogether.Tests.Steps
{
    [Binding]
    [Scope(Feature = "Delete Profile")]
    public sealed class DeleteProfileSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly IWebDriver webDriver;
        private readonly CreateProfilePage createProfilePage;
        private readonly DeleteProfilePage deleteProfilePage;
        private readonly LoginPage loginPage;
        private string PlayTogetherBaseUrl;

        public DeleteProfileSteps(ScenarioContext scenarioContext)
        {
            webDriver = new ChromeDriver();
            webDriver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 30);

            createProfilePage = new CreateProfilePage(webDriver);
            deleteProfilePage = new DeleteProfilePage(webDriver);
            loginPage = new LoginPage(webDriver);
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
            bool profileCreated = createProfilePage.IsProfileCreated();
            Assert.IsTrue(profileCreated);
        }

        [When(@"I access my user profile settings page")]
        public void WhenIAccessMyUserProfileSettingsPage()
        {
            deleteProfilePage.GoToUserProfilePage();
            deleteProfilePage.GoToUserProfileSettingsPage();
            deleteProfilePage.GoToDeleteAccountPage();
        }

        [When(@"Enter my password")]
        public void WhenEnterMyPassword(Table passwordInfo)
        {
            deleteProfilePage.SubmitPassword(passwordInfo);
        }

        [Then(@"I can delete my profile")]
        public void ThenICanDeleteMyProfile(Table loginInfo)
        {
            loginPage.GoToLoginPage();
            bool successful = loginPage.AttemptLogin(loginInfo);
            Assert.IsFalse(successful);
        }

        [AfterScenario]
        public void Dispose()
        {
            webDriver.Dispose();
        }
    }
}
