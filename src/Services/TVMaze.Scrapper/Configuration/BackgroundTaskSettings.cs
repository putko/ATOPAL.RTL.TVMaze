namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration
{
    public class BackgroundTaskSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int GracePeriodTime { get; set; }

        public int CheckUpdateTime { get; set; }
    }
}
