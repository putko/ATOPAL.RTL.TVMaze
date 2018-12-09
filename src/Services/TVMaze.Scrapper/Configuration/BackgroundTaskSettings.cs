namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration
{
    public class BackgroundTaskSettings
    {
        public long UpdateCheckInterval { get; set; }

        public int RateLimitSeconds { get; set; }

        public int RateLimitOccurrences { get; set; }

        public string TvMazeApiBaseUrl { get; set; }
    }
}