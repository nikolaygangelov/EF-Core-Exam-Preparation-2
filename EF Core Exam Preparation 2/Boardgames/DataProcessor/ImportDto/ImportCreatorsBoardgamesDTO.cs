using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Boardgame")]
    public class ImportCreatorsBoardgamesDTO
    {
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        public string Name { get; set; }
        [Required]
        [Range(1.00, 10.00)]
        public double Rating { get; set; }
        [Required]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }
        [Required]
        public int CategoryType { get; set; } //type "int" in order to map with type in given xml file
        [Required]
        public string Mechanics { get; set; }
    }
}
