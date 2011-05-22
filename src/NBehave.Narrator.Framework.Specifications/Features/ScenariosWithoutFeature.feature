Feature: Greeting someone
	As a polite person
	I want to be able to say hello
	So that this scenario doesn't fail due to not having a feature declared

Scenario: greeting Morgan
	Given my name is Morgan
	When I'm greeted
	Then I should be greeted with “Hello, Morgan!”

Scenario: greeting scott
	Given my name is Scott
	When I'm greeted
	Then I should be greeted with “Hello, Scott!”