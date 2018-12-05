namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public partial class Show
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("genres", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Genres { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("runtime", NullValueHandling = NullValueHandling.Ignore)]
        public long Runtime { get; set; }

        [JsonProperty("premiered", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Premiered { get; set; }

        [JsonProperty("officialSite", NullValueHandling = NullValueHandling.Ignore)]
        public Uri OfficialSite { get; set; }

        [JsonProperty("rating", NullValueHandling = NullValueHandling.Ignore)]
        public Rating Rating { get; set; }

        [JsonProperty("weight", NullValueHandling = NullValueHandling.Ignore)]
        public long Weight { get; set; }

        [JsonProperty("externals", NullValueHandling = NullValueHandling.Ignore)]
        public Externals Externals { get; set; }

        [JsonProperty("summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public long Updated { get; set; }

        [JsonProperty("_embedded", NullValueHandling = NullValueHandling.Ignore)]
        public Embedded Embedded { get; set; }
    }
    public partial class Show
    {
        public static Show FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Show>(json, AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain.Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Show self)
        {
            return JsonConvert.SerializeObject(self, AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain.Converter.Settings);
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
