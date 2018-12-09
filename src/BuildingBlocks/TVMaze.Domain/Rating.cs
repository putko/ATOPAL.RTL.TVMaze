namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;

    public class Rating
    {
        [JsonProperty(propertyName: "average", NullValueHandling = NullValueHandling.Ignore)]
        public double Average { get; set; }
    }
}