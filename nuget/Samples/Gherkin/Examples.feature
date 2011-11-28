Feature: Example
	This narrative text do not have to follow any particular format, use what fits you. 
	Parameters (like <start>) in table scenarios can be written as [start], $start or <start>
	In Gherkin syntax examples usually have "Scenario Outline" which you can use, but in nbehave you can use "Scenario" as we have done in this example, 
	you can use whichever you want.
	
Scenario: eating
  Given there are <start> cucumbers
  When I eat <eat> cucumbers
  Then I should have <left> cucumbers

Examples:
    | start | eat | left |
    |  12   |  5  |  7   |
    |  20   |  5  |  15  |