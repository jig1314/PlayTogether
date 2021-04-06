Feature: Search For Gamers
	As an user, I want to have the ability to search for other users based on their profile information so that I can view their profile.

@BDD
Scenario: An user with looking for other users
	Given I have created a profile
		| FirstName | LastName | Email             | Username | Password  | ConfirmPassword | Gender | CountryofResidence | DateofBirth |
		| Test      | Name     | testing@gmail.com | testName | NotR3@Lpw | NotR3@Lpw       | Male   | United States      | 01/01/2000  |
	And I have logged in
	When I am on the gamer search page
	Then I can filter for gamers by information based on their profile 
		| SearchCriteria |
		| gmail          |
	And I will receive feedback