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
            //using Data Transfer Object Class to map it with Creators
            var serializer = new XmlSerializer(typeof(ImportCreatorsDTO[]), new XmlRootAttribute("Creators"));

            //Deserialize method needs TextReader object to convert/map 
            using StringReader inputReader = new StringReader(xmlString);
            var creatorsArrayDTOs = (ImportCreatorsDTO[])serializer.Deserialize(inputReader);

            //using StringBuilder to gather all info in one string
            StringBuilder sb = new StringBuilder();

            //creating List where all valid creators can be kept
            List<Creator> creatorsXML = new List<Creator>();

            foreach (ImportCreatorsDTO creator in creatorsArrayDTOs)
            {
                //validating info for creator from data
                if (!IsValid(creator))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //creating a valid creator
                Creator creatorToAdd = new Creator
                {
                    //using identical properties in order to map successfully
                    FirstName = creator.FirstName,
                    LastName = creator.LastName
                };

                foreach (var boardgame in creator.Boardgames)
                {
                    //validating info for boardgame from data
                    if (!IsValid(boardgame))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    //adding valid boardgame
                    creatorToAdd.Boardgames.Add(new Boardgame()
                    {
                        //using identical properties in order to map successfully
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

            //actual importing info from data
            context.SaveChanges();

            //using TrimEnd() to get rid of white spaces
            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            //using Data Transfer Object Class to map it with sellers
            var sellersArray = JsonConvert.DeserializeObject<ImportSellersDTO[]>(jsonString);

            //using StringBuilder to gather all info in one string
            StringBuilder sb = new StringBuilder();

            //creating List where all valid sellers can be kept
            List<Seller> sellerList = new List<Seller>();

            //taking only unique boardgames
            var existingBoardgameIds = context.Boardgames
                .Select(bg => bg.Id)
                .ToArray();

            foreach (ImportSellersDTO sellerDTO in sellersArray)
            {
                //validating info for seller from data
                if (!IsValid(sellerDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //creating a valid seller
                Seller sellerToAdd = new Seller()
                {
                    //using identical properties in order to map successfully
                    Name = sellerDTO.Name,
                    Address = sellerDTO.Address,
                    Country = sellerDTO.Country,
                    Website = sellerDTO.Website,
                };



                foreach (int boardgameId in sellerDTO.BoardgamesId.Distinct())
                {
                    //validating only unique boardgames
                    if (!existingBoardgameIds.Contains(boardgameId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    //adding valid BoardgameSeller
                    sellerToAdd.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        //using identical properties in order to map successfully
                        Seller = sellerToAdd,
                        BoardgameId = boardgameId
                    });

                }

                sellerList.Add(sellerToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, sellerToAdd.Name, sellerToAdd.BoardgamesSellers.Count));
            }

            context.AddRange(sellerList);

            //actual importing info from data
            context.SaveChanges();

            //using TrimEnd() to get rid of white spaces
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
