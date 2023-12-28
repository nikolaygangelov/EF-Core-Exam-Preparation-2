
using Boardgames.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public  class ExportCreatorsDTO
    {
        [XmlAttribute("BoardgamesCount")]
        public int BoardgamesCount { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(5)]
        [XmlElement("CreatorName")]
        public string CreatorName { get; set; }

        [XmlArray("Boardgames")]
        public ExportBoardgameDTO[] Boardgames { get; set; }
    }
}
