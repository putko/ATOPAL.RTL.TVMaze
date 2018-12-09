namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class Show
    {
        [JsonProperty(propertyName: "id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty(propertyName: "url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty(propertyName: "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(propertyName: "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(propertyName: "language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty(propertyName: "genres", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Genres { get; set; }

        [JsonProperty(propertyName: "status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(propertyName: "runtime", NullValueHandling = NullValueHandling.Ignore)]
        public long Runtime { get; set; }

        [JsonProperty(propertyName: "premiered", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Premiered { get; set; }

        [JsonProperty(propertyName: "officialSite", NullValueHandling = NullValueHandling.Ignore)]
        public Uri OfficialSite { get; set; }

        [JsonProperty(propertyName: "rating", NullValueHandling = NullValueHandling.Ignore)]
        public Rating Rating { get; set; }

        [JsonProperty(propertyName: "weight", NullValueHandling = NullValueHandling.Ignore)]
        public long Weight { get; set; }

        [JsonProperty(propertyName: "externals", NullValueHandling = NullValueHandling.Ignore)]
        public Externals Externals { get; set; }

        [JsonProperty(propertyName: "summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty(propertyName: "updated", NullValueHandling = NullValueHandling.Ignore)]
        public long Updated { get; set; }

        [JsonProperty(propertyName: "_embedded", NullValueHandling = NullValueHandling.Ignore)]
        public Embedded Embedded { get; set; }
    }

    public partial class Show
    {
        public static Show FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Show>(value: json, settings: Converter.Settings);
        }
    }
}