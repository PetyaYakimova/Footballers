using Newtonsoft.Json;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDto
    {
        [JsonProperty("Name")]
        public  string? Name { get; set; }

        [JsonProperty("Nationality")]
        public string? Nationality { get; set; }

        [JsonProperty("Trophies")]
        public int? Trophies { get; set; }

        [JsonProperty("Footballers")]
        public List<int> Footballers { get; set; } = new List<int>();
    }
}
