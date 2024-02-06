using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Coach")]
    public class ImportCoachDto
    {
        [XmlElement("Name")]
        public string? Name { get; set; }

        [XmlElement("Nationality")]
        public string? Nationality { get; set; }

        [XmlArray("Footballers")]
        public List<ImportFootballerDto> Footballers { get; set; } = new List<ImportFootballerDto>();
    }
}
