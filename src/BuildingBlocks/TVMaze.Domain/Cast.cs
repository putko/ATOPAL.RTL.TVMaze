namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;

    public class Cast
    {
        [JsonProperty(propertyName: "person", NullValueHandling = NullValueHandling.Ignore)]
        public Person Person { get; set; }

        [JsonProperty(propertyName: "character", NullValueHandling = NullValueHandling.Ignore)]
        public Character Character { get; set; }

        [JsonProperty(propertyName: "self", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Self { get; set; }

        [JsonProperty(propertyName: "voice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Voice { get; set; }
    }
}