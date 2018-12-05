using AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.TVMaze
{
    public class TVMazeApi
    {
        private readonly BackgroundTaskSettings _settings;
        private const string UpdatePath = "updates/shows";
        private const string ShowsWithCastPath = "shows/{id}?embed=cast";
        
        public TVMazeApi(BackgroundTaskSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        private string Execute(RestRequest request)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new System.Uri(_settings.TVMazeAPIBaseURL)
            };
            IRestResponse response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                ApplicationException exception = new ApplicationException(message, response.ErrorException);
            }
            return response.Content;
        }

        public Dictionary<string, long> GetUpdates()
        {
            RestRequest request = new RestRequest(TVMazeApi.UpdatePath, Method.GET, DataFormat.Json);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            var result = JsonConvert.DeserializeObject<Dictionary<string, long>>(Execute(request));
            return result;
        }
        public Show GetShowDetail(string showId)
        {
            RestRequest request = new RestRequest(TVMazeApi.ShowsWithCastPath, Method.GET, DataFormat.Json);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter(new Parameter("id", showId, ParameterType.UrlSegment));

            var result = Show.FromJson(Execute(request));
            return result;
        }
    }
}