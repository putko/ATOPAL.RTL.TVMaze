using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    public partial class Character
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
