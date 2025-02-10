using System.Xml.Serialization;

namespace TrueCode.Services.Fetcher.DTO;

[XmlRoot("ValCurs")]
public record ValCurs
{
    [XmlAttribute("Date")]
    public required string Date { get; set; }
        
    [XmlAttribute("name")]
    public required string Name { get; set; }
        
    [XmlElement("Valute")]
    public required List<Valute> Valutes { get; set; }
}