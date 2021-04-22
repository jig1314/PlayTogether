using NUnit.Framework;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayTogether.Tests.UnitTests
{
    [TestFixture]
    [Category("UnitTest")]
    public class ChatGroupValidatorTests
    {
        private ChatGroupValidator validator;

        [SetUp]
        public void ChatGroupValidatorTestSetUp()
        {
            validator = new ChatGroupValidator();
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenLeftEmpty()
        {
            var model = new ChatGroupViewModel() { GroupName = null };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsLessThanSign()
        {
            var model = new ChatGroupViewModel() { GroupName = "New<Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsGreaterThanSign()
        {
            var model = new ChatGroupViewModel() { GroupName = "New>Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsPoundSign()
        {
            var model = new ChatGroupViewModel() { GroupName = "New#Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsPlusSign()
        {
            var model = new ChatGroupViewModel() { GroupName = "New+Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsPercentSign()
        {
            var model = new ChatGroupViewModel() { GroupName = "New%Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsGlyphSign()
        {
            var model = new ChatGroupViewModel() { GroupName = "New|Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsForwardSlash()
        {
            var model = new ChatGroupViewModel() { GroupName = "New/Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveValidationError_WhenContainsBackSlash()
        {
            var model = new ChatGroupViewModel() { GroupName = "New\\Group" };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.GroupName);
        }

        [Test]
        public void GroupNameShould_HaveNoValidationErrors_WhenDoesNotContainInvalidCharacter()
        {
            var model = new ChatGroupViewModel() { GroupName = "New_Group" };
            var result = validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.GroupName);
        }
    }
}
