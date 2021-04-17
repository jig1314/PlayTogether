Feature: Delete Profile
	As an user, I want to have the ability to delete my profile so that my information is no longer in the application.

@BDD
Scenario: Existing user logged into the application and deletes profile
	Given I have created a profile
		| FirstName | LastName | Email             | Username | Password  | ConfirmPassword | Gender | CountryofResidence | DateofBirth |
		| Test      | Name     | testing@gmail.com | testName | NotR3@Lpw | NotR3@Lpw       | Male   | United States      | 01/01/2000  |
	And I have logged in
	When I access my user profile settings page
	And Enter my password
		| Password  |
		| NotR3@Lpw |
	Then I can delete my profile
		| Username | Password |
		| testName | NotR3@Lpw |