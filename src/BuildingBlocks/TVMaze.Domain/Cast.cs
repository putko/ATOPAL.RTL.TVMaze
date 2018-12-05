using AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain;
using Newtonsoft.Json;

namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain 
{
    public partial class Cast
    {
        [JsonProperty("person", NullValueHandling = NullValueHandling.Ignore)]
        public Person Person { get; set; }

        [JsonProperty("character", NullValueHandling = NullValueHandling.Ignore)]
        public Character Character { get; set; }

        [JsonProperty("self", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Self { get; set; }

        [JsonProperty("voice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Voice { get; set; }
    }
}
