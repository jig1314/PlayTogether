Feature: Create Profile
	As an user, I want to have the ability to create a profile so that I can access the application's features.

@basic
Scenario: New user creating a profile
	Given I access the application
	When I click register link
	And I submit the required information
		| FirstName | LastName | Email             | Username | Password  | ConfirmPassword | Gender | CountryofResidence | DateofBirth |
		| Test      | Name     | testing@gmail.com | testName | NotR3@Lpw | NotR3@Lpw       | Male   | United States      | 01/01/2000  |
	Then I have the ability to create a profile