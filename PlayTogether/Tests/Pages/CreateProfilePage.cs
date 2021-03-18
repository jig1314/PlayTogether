using OpenQA.Selenium;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace PlayTogether.Tests.Pages
{
    public class CreateProfilePage
    {
        public IWebDriver WebDriver { get; }

        #region Properties

        public IWebElement RegisterLink => WebDriver.FindElement(By.Id("registerLink"));

        public IWebElement LogOutButton => WebDriver.FindElement(By.Id("logOutButton"));

        public IWebElement FirstName => WebDriver.FindElement(By.Id("firstNameText"));

        public IWebElement LastName => WebDriver.FindElement(By.Id("lastNameText"));

        public IWebElement Email => WebDriver.FindElement(By.Id("emailText"));

        public IWebElement UserName => WebDriver.FindElement(By.Id("userNameText"));

        public IWebElement Password => WebDriver.FindElement(By.Id("passwordText"));

        public IWebElement ConfirmPassword => WebDriver.FindElement(By.Id("confirmPasswordText"));

        public SelectElement Gender => new SelectElement(WebDriver.FindElement(By.Id("genderDropdown")));

        public SelectElement CountryOfResidence => new SelectElement(WebDriver.FindElement(By.Id("countryOfResidenceDropdown")));

        public IWebElement Submit => WebDriver.FindElement(By.Id("submitButton"));

        #endregion

        public CreateProfilePage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public void ClickRegister()
        {
            RegisterLink.Click();
        }

        public void SubmitRequiredInformation(Table registerInfotable)
        {
            dynamic registerInfo = registerInfotable.CreateDynamicInstance();
            FirstName.SendKeys(registerInfo.FirstName);
            LastName.SendKeys(registerInfo.LastName);
            Email.SendKeys(registerInfo.Email);
            UserName.SendKeys(registerInfo.Username);
            Password.SendKeys(registerInfo.Password);
            ConfirmPassword.SendKeys(registerInfo.ConfirmPassword);
            Gender.SelectByText(registerInfo.Gender);
            CountryOfResidence.SelectByText(registerInfo.CountryofResidence);

            ((IJavaScriptExecutor)WebDriver).ExecuteScript("document.getElementById('dateOfBirthDatePicker').removeAttribute('readonly',0);");
            IWebElement dateOfBirth = WebDriver.FindElement(By.Id("dateOfBirthDatePicker"));
            dateOfBirth.Clear();

            DateTime date = Convert.ChangeType(registerInfo.DateofBirth, TypeCode.DateTime);
            dateOfBirth.SendKeys(date.ToString("MM/dd/yyyy"));

            Submit.Click();
        }

        public bool IsProfileCreated()
        {
            return LogOutButton.Displayed;
        }

    }
}
