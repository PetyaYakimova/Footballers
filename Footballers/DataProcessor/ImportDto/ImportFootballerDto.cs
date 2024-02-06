using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Footballer")]
    public class ImportFootballerDto
    {
        [XmlElement("Name")]
        public string? Name { get; set; }

        [XmlElement("ContractStartDate")]
        public string? ContractStartDate { get; set; }

        [XmlElement("ContractEndDate")]
        public string? ContractEndDate { get; set; }

        [XmlElement("BestSkillType")]
        public int? BestSkillType { get; set; }

        [XmlElement("PositionType")]
        public int? PositionType { get; set; }
    }
}
