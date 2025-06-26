Feature: Users API Testing

  Scenario: Validate Users API on page 1
    When I send a GET request to "https://reqres.in/api/users?page=1"
    Then the response status code should be 200
    And the response should contain exactly 6 users
    And the response page number should be 1
    And the response should contain the user:
      | first_name | last_name | email                  | avatar                                   |
      | Janet      | Weaver    | janet.weaver@reqres.in | https://reqres.in/img/faces/2-image.jpg  |

  Scenario: Validate Users API on page 2
    When I send a GET request to "https://reqres.in/api/users?page=2"
    Then the response status code should be 200
    And the response should contain exactly 6 users
    And the response page number should be 2
    And the response should contain the user:
      | first_name | last_name | email                 | avatar                                    |
      | Byron      | Fields    | byron.fields@reqres.in | https://reqres.in/img/faces/10-image.jpg |

  Scenario: Validate Users API with non-existent page
    When I send a GET request to "https://reqres.in/api/users?page=12"
    Then the response status code should be 200
    And the response should contain exactly 0 users
