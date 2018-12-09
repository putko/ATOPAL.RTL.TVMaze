namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.Domain
{
    using Newtonsoft.Json;

    public static class Serialize
    {
        public static string ToJson(this Show self)
        {
            return JsonConvert.SerializeObject(value: self, settings: Converter.Settings);
        }
    }
}