﻿using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using PlayTogether.Client.ViewModels;
using System;

namespace PlayTogether.Tests.UnitTests
{
    [TestFixture]
    [Category("UnitTest")]
    public class RegisterValidatorTests
    {
        private RegisterValidator validator;

        [SetUp]
        public void RegisterValidatorTestSetUp()
        {
            validator = new RegisterValidator(); 
        }

        [Test]
        public void FirstNameShould_HaveValidationError_WhenLeftEmpty()
        {
            var model = new RegisterViewModel() { FirstName = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Test]
        public void LastNameShould_HaveValidationError_WhenOnlyOneCharacter()
        {
            var model = new RegisterViewModel() { LastName = "D" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Test]
        public void LastNameShould_HaveValidationError_WhenDoesNotStartWithUppercaseLetter()
        {
            var model = new RegisterViewModel() { LastName = "gAmble" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Test]
        public void PasswordShould_HaveValidationError_WhenDoesNotContainUppercaseLetter()
        {
            var model = new RegisterViewModel() { Password = "lower@@123" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void PasswordShould_HaveValidationError_WhenDoesNotContainLowercaseLetter()
        {
            var model = new RegisterViewModel() { Password = "LOWER@@123" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void PasswordShould_HaveValidationError_WhenDoesNotContainNumbers()
        {
            var model = new RegisterViewModel() { Password = "LOWER@@blah" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void PasswordShould_HaveValidationError_WhenDoesNotSpecialCharacters()
        {
            var model = new RegisterViewModel() { Password = "LOWERblah123" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void PasswordShould_HaveValidationError_WhenIsNotSixCharactersLong()
        {
            var model = new RegisterViewModel() { Password = "Lo@1" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void ConfirmPasswordShould_HaveValidationError_WhenDoesNotMatchPassword()
        {
            var model = new RegisterViewModel() { Password = "Testing@123", ConfirmPassword = "Test!123" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        }

        [Test]
        public void DateOfBirthShould_HaveValidationError_WhenGreaterThan150YearsInThePastFromToday()
        {
            var model = new RegisterViewModel() { DateOfBirth = DateTime.Now.AddYears(-151) };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Test]
        public void DateOfBirthShould_HaveValidationError_WhenGreaterThanToday()
        {
            var model = new RegisterViewModel() { DateOfBirth = DateTime.Now.AddDays(1) };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Test]
        public void DateOfBirthShould_HaveValidationError_WhenEqualToday()
        {
            var model = new RegisterViewModel() { DateOfBirth = DateTime.Now };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Test]
        public void DateOfBirthShould_HaveValidationError_WhenEmpty()
        {
            var model = new RegisterViewModel() { DateOfBirth = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }
    }
}
