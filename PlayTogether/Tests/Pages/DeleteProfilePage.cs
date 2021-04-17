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
    public class DeleteProfilePage
    {
        public IWebDriver WebDriver { get; }

        public IWebElement MyProfileLink => WebDriver.FindElement(By.Id("myProfileLink"));

        public IWebElement ManageAccountButton  => WebDriver.FindElement(By.Id("manageAccountButton"));

        public IWebElement DeleteAccountButton => WebDriver.FindElement(By.Id("deleteAccountLink"));

        public IWebElement PasswordTextBox => WebDriver.FindElement(By.Id("deleteAccountPasswordText"));

        public IWebElement SubmitButton => WebDriver.FindElement(By.Id("deleteAccountSubmitButton"));

        public IWebElement RegisterLink => WebDriver.FindElement(By.Id("registerLink"));

        public DeleteProfilePage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public void GoToUserProfilePage()
        {
            MyProfileLink.Click();
        }

        public void GoToUserProfileSettingsPage()
        {
            ManageAccountButton.Click();
        }

        public void GoToDeleteAccountPage()
        {
            DeleteAccountButton.Click();
        }

        public void SubmitPassword(Table passwordInfo)
        {
            dynamic password = passwordInfo.CreateDynamicInstance();
            PasswordTextBox.SendKeys(password.Password);

            SubmitButton.Click();
        }

        public bool IsProfileDeleted()
        {
            return RegisterLink.Displayed;
        }
    }
}
