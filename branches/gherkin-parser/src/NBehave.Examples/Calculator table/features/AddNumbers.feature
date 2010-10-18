Feature: add, subtract, divide and multiply

Scenario: Add numbers
	Given I have entered [num1] into the calculator
	And I have entered [num2] into the calculator
	When I add the numbers
	Then the sum should be [result]
	
Examples:
|num1|num2|result|
|   1|   2|     3|
|  -1|   2|     1|
|   1|  -2|    -1|
|  -2|  -3|    -5|