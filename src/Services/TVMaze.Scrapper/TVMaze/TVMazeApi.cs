namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.TVMaze
{
    using System;
    using System.Collections.Generic;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
    using Newtonsoft.Json;
    using RestSharp;

    public class TvMazeApi
    {
        // https://www.tvmaze.com/api#shows
        private const string UpdatePath = "updates/shows";
        private const string ShowsWithCastPath = "shows/{id}?embed=cast";

        private readonly BackgroundTaskSettings _settings;

        public TvMazeApi(BackgroundTaskSettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(paramName: nameof(settings));
        }

        private string Execute(RestRequest request)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(uriString: this._settings.TvMazeApiBaseUrl)
            };
            var response = client.Execute(request: request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var exception = new ApplicationException(message: message, innerException: response.ErrorException);
            }

            return response.Content;
        }

        public Dictionary<string, long> GetUpdates()
        {
            var request = new RestRequest(resource: TvMazeApi.UpdatePath, method: Method.GET,
                dataFormat: DataFormat.Json);
            request.AddHeader(name: "Content-Type", value: "application/json");
            request.AddHeader(name: "cache-control", value: "no-cache");
            var result = JsonConvert.DeserializeObject<Dictionary<string, long>>(value: this.Execute(request: request));
            return result;
        }

        public Show GetShowDetail(string showId)
        {
            var request = new RestRequest(resource: TvMazeApi.ShowsWithCastPath, method: Method.GET,
                dataFormat: DataFormat.Json);
            request.AddHeader(name: "Content-Type", value: "application/json");
            request.AddHeader(name: "cache-control", value: "no-cache");
            request.AddParameter(p: new Parameter(name: "id", value: showId, type: ParameterType.UrlSegment));

            var result = Show.FromJson(json: this.Execute(request: request));
            return result;
        }
    }
}