Feature: tables
	This narrative text do not have to follow any particular format, use what fits you. 
	Parameters (like <start>) in table scenarios can be written as [start], $start or <start>
	In Gherkin syntax examples usually have "Scenario Outline" which you can use, but in nbehave you can use "Scenario" as we have done in this example, 
	you can use whichever you want.
	
Scenario: a table

Given a list of contacts:
	| Name          | Country |
	|Morgan Persson | Sweden  |
	|Jimmy Nilsson  | Sweden  |
	|Jimmy Bogard   | USA     |

When I search for contacts from Sweden

Then I should find:
	| Name           | Country |
	| Morgan Persson | Sweden  |
	| Jimmy Nilsson  | Sweden  |