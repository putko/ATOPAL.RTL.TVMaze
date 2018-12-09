namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using System;
    using System.ComponentModel;
    using Newtonsoft.Json;

    public class Person
    {
        [JsonProperty(propertyName: "id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty(propertyName: "url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty(propertyName: "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(propertyName: "birthday", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Birthday { get; set; }

        [JsonProperty(propertyName: "deathday", NullValueHandling = NullValueHandling.Ignore)]
        public object Deathday { get; set; }

        [JsonProperty(propertyName: "gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }
    }
}