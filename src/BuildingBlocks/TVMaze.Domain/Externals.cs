namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;

    public class Externals
    {
        [JsonProperty(propertyName: "tvrage", NullValueHandling = NullValueHandling.Ignore)]
        public long TvRage { get; set; }

        [JsonProperty(propertyName: "thetvdb", NullValueHandling = NullValueHandling.Ignore)]
        public long TheTvDb { get; set; }

        [JsonProperty(propertyName: "imdb", NullValueHandling = NullValueHandling.Ignore)]
        public string Imdb { get; set; }
    }
}