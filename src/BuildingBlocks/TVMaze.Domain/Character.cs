namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;

    public class Character
    {
        [JsonProperty(propertyName: "id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty(propertyName: "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}