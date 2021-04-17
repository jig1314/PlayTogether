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
    public class LoginPage
    {
        public IWebDriver WebDriver { get; }

        public IWebElement UserName => WebDriver.FindElement(By.Id("userNameText"));

        public IWebElement Password => WebDriver.FindElement(By.Id("passwordText"));

        public IWebElement SubmitButton => WebDriver.FindElement(By.Id("loginSubmitButton"));

        public IWebElement LoginLink => WebDriver.FindElement(By.Id("loginLink"));

        public IWebElement ErrorAlert => WebDriver.FindElement(By.Id("loginErrorAlert"));

        public LoginPage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public void GoToLoginPage()
        {
            LoginLink.Click();
        }

        public bool AttemptLogin(Table loginInfo)
        {
            dynamic login = loginInfo.CreateDynamicInstance();
            UserName.SendKeys(login.Username);
            Password.SendKeys(login.Password);

            SubmitButton.Click();

            return !ErrorAlert.Displayed;
        }
    }
}
