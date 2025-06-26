using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Threading;
using TechTalk.SpecFlow;
using YourProjectNamespace.Support;

namespace ApiTestFramework.StepDefinitions
{
    [Binding]
    public class WeatherApiSteps
    {
        private RestResponse _response;
        private JObject _json;
        private string _location;

        [Given(@"I send a GET request to the Weatherstack API for ""(.*)""")]
        public void GivenISendAGETRequestToTheWeatherstackAPI(string location)
        {
            _location = location;
            string accessKey = WeatherstackConfig.GetApiKey();
            var client = new RestClient($"http://api.weatherstack.com/current?access_key={accessKey}&query={location}");
            var request = new RestRequest();

            const int maxRetries = 3;
            int retry = 0;
            int waitTimeMs = 500;

            while (retry < maxRetries)
            {
                _response = client.Execute(request);

                if (!string.IsNullOrWhiteSpace(_response.Content))
                {
                    try
                    {
                        _json = JObject.Parse(_response.Content);

                        if (_json["location"] != null && _json["current"] != null)
                            return; // ? All good, exit early
                    }
                    catch
                    {
                        // Ignore and retry
                    }
                }

                retry++;
                Thread.Sleep(waitTimeMs);
                waitTimeMs *= 2; // Exponential backoff (e.g., 500ms ? 1s ? 2s)
            }

            throw new Exception("API failed to return valid data after 3 attempts.");
        }

        [Then(@"the response should contain weather data for ""(.*)""")]
        public void ThenTheResponseShouldContainWeatherDataFor(string location)
        {
            Assert.That(_response.IsSuccessful, "API call failed.");

            // Validate the country returned matches the requested location
            Assert.IsNotNull(_json["location"], "Location object not found in response.");
            Assert.IsNotNull(_json["location"]["country"], "Country is missing in the location object.");

            var actualCountry = _json["location"]["country"]?.ToString().ToLower();
            Assert.That(actualCountry, Does.Contain(location.ToLower()),
                $"Expected country to contain '{location}', but got '{actualCountry}'.");

            // Also check core weather data exists
            Assert.That(_json["current"]?["temperature"], Is.Not.Null, "Temperature data is missing.");
        }

        [Then(@"the observation time should be within the last 24 hours")]
        public void ThenTheObservationTimeShouldBeWithinLast24Hours()
        {
            string observationTime = _json["current"]?["observation_time"]?.ToString();
            string localDate = _json["location"]?["localtime"]?.ToString();

            var datePart = DateTime.Parse(localDate).Date;
            var timePart = DateTime.Parse(observationTime).TimeOfDay;
            var fullObservationTime = datePart + timePart;

            Assert.That(fullObservationTime, Is.GreaterThan(DateTime.Now.AddHours(-24)), "Observation time is older than 24 hours.");
        }
    }
}
