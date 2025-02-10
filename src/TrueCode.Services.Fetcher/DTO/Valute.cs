using System.Xml.Serialization;

namespace TrueCode.Services.Fetcher.DTO;

public record Valute
{
    [XmlAttribute("ID")]
    public required string ID { get; set; }
        
    [XmlElement("NumCode")]
    public required string NumCode { get; set; }
        
    [XmlElement("CharCode")]
    public required string CharCode { get; set; }
        
    [XmlElement("Nominal")]
    public int Nominal { get; set; }
        
    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("Value")]
    public required string Value { get; set; }
}