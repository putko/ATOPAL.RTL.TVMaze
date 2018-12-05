namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration
{
    public class BackgroundTaskSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public string EventBusUserName { get; set; }

        public string EventBusPassword { get; set; }

        public long UpdateCheckInterval { get; set; }

        public int RateLimitSeconds { get; set; }

        public int RateLimitOccurences { get; set; }

        public string TVMazeAPIBaseURL { get; set; }

    }
}
