Feature: Greeting someone
	As a polite person
	I want to be able to say hello
	So that this scenario doesn't fail due to not having a feature declared

Scenario: greeting Morgan
	Given my name is Kalle
	When I'm greeted
	Then I should be greeted with “Hello, Kalle!”
#comment