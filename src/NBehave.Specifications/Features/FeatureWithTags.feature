@tag1 @tag2
Feature: Greeting system with tags
    As a project member
    I want specs written in a non techie way
    So that everyone can understand them
    
	@tag3 @tag4
    Scenario: Greeting someone
        Given my name is Morgan
        When I'm greeted
        Then I should be greeted with “Hello, Morgan!”

	@tag5
    Scenario: Greeting someone else
        Given my name is Anna
        When I'm greeted
        Then I should be greeted with “Hello, Anna!”