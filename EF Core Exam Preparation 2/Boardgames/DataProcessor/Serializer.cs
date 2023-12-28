namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            //using Data Transfer Object Class to map it with Creators
            XmlSerializer serializer = new XmlSerializer(typeof(ExportCreatorsDTO[]), new XmlRootAttribute("Creators"));

            //using StringBuilder to gather all info in one string
            StringBuilder sb = new StringBuilder();

            //"using" automatically closes opened connections
            using var writer = new StringWriter(sb);

            var xns = new XmlSerializerNamespaces();

            //one way to display empty namespace in resulted file
            xns.Add(string.Empty, string.Empty);

            var creatorsAndGames = context.Creators
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorsDTO
                {
                    //using identical properties in order to map successfully
                    BoardgamesCount = c.Boardgames.Count,
                    CreatorName = c.FirstName + " " + c.LastName,
                    Boardgames = c.Boardgames
                    .Select(bg => new ExportBoardgameDTO
                    {
                        Name = bg.Name,
                        YearPublished = bg.YearPublished
                    })
                    .OrderBy(c => c.Name)
                    .ToArray()
                })
                .OrderByDescending(c => c.BoardgamesCount)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            //Serialize method needs file, TextReader object and namespace to convert/map
            serializer.Serialize(writer, creatorsAndGames, xns);

            //explicitly closing connection in terms of reaching edge cases
            writer.Close();

            //using TrimEnd() to get rid of white spaces
            return sb.ToString().TrimEnd();
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            //turning needed info about products into a collection using anonymous object
            //using less data
            var sellersAndBoardgames = context.Sellers
                .Where(s => s.BoardgamesSellers.Any(bgs => bgs.Boardgame.YearPublished >= year && bgs.Boardgame.Rating <= rating))
                .Select(s => new
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                    .Where(bgs => bgs.Boardgame.YearPublished >= year && bgs.Boardgame.Rating <= rating)
                    .Select(bg => new
                    {
                        Name = bg.Boardgame.Name,
                        Rating = bg.Boardgame.Rating,
                        Mechanics = bg.Boardgame.Mechanics,
                        Category = bg.Boardgame.CategoryType.ToString()
                    })
                    .OrderByDescending(bg => bg.Rating)
                    .ThenBy(bg => bg.Name)
                    .ToArray()
                })
                .OrderByDescending(s => s.Boardgames.Length)
                .ThenBy(s => s.Name)
                .Take(5)
                .ToArray();

            //Serialize method needs object to convert/map
	        //adding Formatting for better reading
            return JsonConvert.SerializeObject(sellersAndBoardgames, Formatting.Indented);
        }
    }
}
