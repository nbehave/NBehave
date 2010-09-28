Story: A fancy greeting system
	As a bdd guy
	I want this to work
	So that I can specify my stories as text
	
	Scenario: A greeting
		Given my name is Morgan
		When I'm greeted
		Then I should be greeted with “Hello, Morgan!”
	
	Scenario: Another greeting
		Given my name is Axel
		When I'm greeted
		Then I should be greeted with “Hello, Axel!”