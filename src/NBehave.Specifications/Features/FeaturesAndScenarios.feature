Feature: S1
  As a X1
  I want Y1
  So that Z1

  Scenario: SC1
    Given something
    When some event occurs
    Then there is some outcome
  Scenario: SC2
    Given something two
    When some event #2 occurs
    Then there is some outcome #2
  Scenario: Pending scenario
    Given something pending
    And something pending
    And something more pending
    When some pending event occurs
    And some more pending event occurs
    Then this text should still show up in xml output

Feature: S2
  As a X2
  I want Y2
  So that Z2

  Scenario: SC1
    Given something
    When some event occurs
    Then there is some outcome

Feature: S3
  As a X3
  I want Y3
  So that Z3

  Scenario: SC3
    Given something
    When some event occurs
    Then there is some outcome
  Scenario: FailingScenario
    Given something x
    When some event y occurs
    Then there is some failing outcome
