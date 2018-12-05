namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;

    public partial class Rating
    {
        [JsonProperty("average", NullValueHandling = NullValueHandling.Ignore)]
        public double Average { get; set; }
    }
}