using Footballers.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Coach")]
    public class ExportCoachDto
    {
        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }

        [XmlElement("CoachName")]
        public string Name { get; set; } = null!;

        [XmlArray("Footballers")]
        public List<ExportFootballerDto> Footballers { get; set; } = new List<ExportFootballerDto>()!;
    }
}
