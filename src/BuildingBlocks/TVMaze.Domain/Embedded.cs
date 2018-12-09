namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Embedded
    {
        [JsonProperty(propertyName: "cast", NullValueHandling = NullValueHandling.Ignore)]
        public List<Cast> Cast { get; set; }
    }
}