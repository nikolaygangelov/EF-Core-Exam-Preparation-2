using Boardgames.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellersDTO
    {
        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        [JsonProperty("Name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        [JsonProperty("Address")]
        public string Address { get; set; }
        [Required]
        [JsonProperty("Country")]
        public string Country { get; set; }

        [Required]
        [RegularExpression(@"^www\.[a-zA-z0-9-]+\.com")]
        [JsonProperty("Website")]
        public string Website { get; set; }

        [JsonProperty("Boardgames")]
        public int[] BoardgamesId { get; set; } //type "int" in order to map with given xml file
    }
}
