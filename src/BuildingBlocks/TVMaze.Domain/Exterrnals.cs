using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    public partial class Externals
    {
        [JsonProperty("tvrage", NullValueHandling = NullValueHandling.Ignore)]
        public long Tvrage { get; set; }

        [JsonProperty("thetvdb", NullValueHandling = NullValueHandling.Ignore)]
        public long Thetvdb { get; set; }

        [JsonProperty("imdb", NullValueHandling = NullValueHandling.Ignore)]
        public string Imdb { get; set; }
    }
}
