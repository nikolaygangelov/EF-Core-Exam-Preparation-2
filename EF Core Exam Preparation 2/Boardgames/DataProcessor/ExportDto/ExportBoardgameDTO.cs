
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Boardgame")]
    public class ExportBoardgameDTO
    {
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        [XmlElement("BoardgameName")]
        public string Name { get; set; }

        [Required]
        [Range(2018, 2023)]
        [XmlElement("BoardgameYearPublished")]
        public int YearPublished { get; set; }
    }
}
