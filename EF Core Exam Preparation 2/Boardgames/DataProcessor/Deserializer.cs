namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportCreatorsDTO[]), new XmlRootAttribute("Creators"));
            using StringReader inputReader = new StringReader(xmlString);
            var creatorsArrayDTOs = (ImportCreatorsDTO[])serializer.Deserialize(inputReader);

            StringBuilder sb = new StringBuilder();
            List<Creator> creatorsXML = new List<Creator>();

            foreach (ImportCreatorsDTO creator in creatorsArrayDTOs)
            {
                Creator creatorToAdd = new Creator
                {
                    FirstName = creator.FirstName,
                    LastName = creator.LastName
                };

                if (!IsValid(creator))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                foreach (var boardgame in creator.Boardgames)
                {
                    if (!IsValid(boardgame))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    creatorToAdd.Boardgames.Add(new Boardgame()
                    {
                        Name = boardgame.Name,
                        Rating = boardgame.Rating,
                        YearPublished = boardgame.YearPublished,
                        CategoryType = (CategoryType)(boardgame.CategoryType),
                        Mechanics = boardgame.Mechanics
                    });
                }

                creatorsXML.Add(creatorToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creatorToAdd.FirstName, creatorToAdd.LastName,
                    creatorToAdd.Boardgames.Count));
            }

            context.Creators.AddRange(creatorsXML);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sellersArray = JsonConvert.DeserializeObject<ImportSellersDTO[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            List<Seller> sellerList = new List<Seller>();

            var existingBoardgameIds = context.Boardgames
                .Select(bg => bg.Id)
                .ToArray();

            foreach (ImportSellersDTO sellerDTO in sellersArray)
            {

                if (!IsValid(sellerDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller sellerToAdd = new Seller()
                {
                    Name = sellerDTO.Name,
                    Address = sellerDTO.Address,
                    Country = sellerDTO.Country,
                    Website = sellerDTO.Website,
                };



                foreach (int boardgameId in sellerDTO.BoardgamesId.Distinct())
                {
                    if (!existingBoardgameIds.Contains(boardgameId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    sellerToAdd.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        Seller = sellerToAdd,// !!!!!!!!!!!
                        BoardgameId = boardgameId
                    });

                }

                sellerList.Add(sellerToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, sellerToAdd.Name, sellerToAdd.BoardgamesSellers.Count));
            }

            context.AddRange(sellerList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
