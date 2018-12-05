namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class Embedded
    {
        [JsonProperty("cast", NullValueHandling = NullValueHandling.Ignore)]
        public List<Cast> Cast { get; set; }
    }
}
