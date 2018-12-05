using Newtonsoft.Json;
using System;

namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    public partial class Person
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("birthday", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Birthday { get; set; }

        [JsonProperty("deathday", NullValueHandling = NullValueHandling.Ignore)]
        public object Deathday { get; set; }

        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }
    }
}
