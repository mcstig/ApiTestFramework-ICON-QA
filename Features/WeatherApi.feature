Feature: WeatherStack API Testing
  As a QA Engineer
  I want to validate the weatherstack API functionality
  So that I can ensure reliable weather data responses globally

  Scenario Outline: Validate weather data and freshness for a given location
    Given I send a GET request to the Weatherstack API for "<location>"
    Then the response should contain weather data for "<location>"
    And the observation time should be within the last 24 hours

    Examples:
      | location       |
      | Malta          |
      | United States  |
      | Japan          |
      | Australia      |
      | South Africa   |
