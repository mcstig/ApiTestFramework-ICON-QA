using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using ApiTestFramework.Support;

namespace ApiTestFramework.StepDefinitions
{
    [Binding]
    public class UsersApiSteps
    {
        private RestResponse _response;
        private JObject _responseBody;

        [When(@"I send a GET request to ""(.*)""")]
        public void WhenISendAGetRequestTo(string url)
        {
            var apiKey = ReqresConfig.GetApiKey();

            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Get
            };

            request.AddHeader("User-Agent", "ApiTestFramework");
            request.AddHeader("x-api-key", apiKey);

            _response = client.Execute(request);

            Console.WriteLine("Response Content: " + _response.Content);

            if (!string.IsNullOrWhiteSpace(_response.Content))
            {
                try
                {
                    _responseBody = JObject.Parse(_response.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JSON Parse Error: " + ex.Message);
                    _responseBody = null;
                }
            }
            else
            {
                _responseBody = null;
            }
        }

        [Then(@"the response status code should be (.*)")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            Assert.AreEqual(statusCode, (int)_response.StatusCode);
        }

        [Then(@"the response should contain exactly (.*) users")]
        public void ThenTheResponseShouldContainExactlyNUsers(int expectedCount)
        {
            Assert.IsNotNull(_responseBody, "Response body is null or failed to parse.");

            var users = _responseBody["data"];
            Assert.IsNotNull(users, "Users data is missing in response.");
            Assert.AreEqual(expectedCount, users.Count());
        }

        [Then(@"the response page number should be (.*)")]
        public void ThenTheResponsePageNumberShouldBe(int expectedPage)
        {
            var page = (int)_responseBody["page"];
            Assert.AreEqual(expectedPage, page);
        }

        [Then(@"the response should contain the user:")]
        public void ThenTheResponseShouldContainTheUser(Table table)
        {
            var expected = table.Rows[0];
            var users = _responseBody["data"];

            bool userFound = users.Any(user =>
                user["first_name"].ToString() == expected["first_name"] &&
                user["last_name"].ToString() == expected["last_name"] &&
                user["email"].ToString() == expected["email"] &&
                user["avatar"].ToString() == expected["avatar"]
            );

            Assert.IsTrue(userFound, "Expected user not found in response.");
        }
    }
}
