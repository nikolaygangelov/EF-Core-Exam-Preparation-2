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
            XmlSerializer serializer = new XmlSerializer(typeof(ExportCreatorsDTO[]), new XmlRootAttribute("Creators"));

            StringBuilder sb = new StringBuilder();

            using var writer = new StringWriter(sb);

            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);

            var creatorsAndGames = context.Creators
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorsDTO
                {
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

            serializer.Serialize(writer, creatorsAndGames, xns);
            writer.Close();

            return sb.ToString();
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
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

            return JsonConvert.SerializeObject(sellersAndBoardgames, Formatting.Indented);
        }
    }
}