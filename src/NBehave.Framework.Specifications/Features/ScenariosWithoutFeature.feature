Scenario: greeting Morgan
	Given my name is Morgan
	When I'm greeted
	Then I should be greeted with “Hello, Morgan!”

Scenario: greeting scott
	Given my name is Scott
	When I'm greeted
	Then I should be greeted with “Hello, Scott!”