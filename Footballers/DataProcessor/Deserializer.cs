namespace Footballers.DataProcessor
{
    using Castle.Core.Internal;
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Footballers.Utilities;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            ImportCoachDto[] coaches = xmlHelper.Deserialize<ImportCoachDto[]>(xmlString, "Coaches");

            List<Coach> validCoaches = new List<Coach>();
            StringBuilder sb = new StringBuilder();

            foreach (ImportCoachDto coach in coaches)
            {
                if (string.IsNullOrEmpty(coach.Name) || string.IsNullOrEmpty(coach.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Coach validCoach = new Coach();
                validCoach.Name = coach.Name;
                validCoach.Nationality = coach.Nationality;

                if (!IsValid(validCoach))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                List<Footballer> validFootballers = new List<Footballer>();

                foreach (ImportFootballerDto footballer in coach.Footballers)
                {
                    if (string.IsNullOrEmpty(footballer.Name) || string.IsNullOrEmpty(footballer.ContractEndDate) || string.IsNullOrEmpty(footballer.ContractStartDate) || !footballer.PositionType.HasValue || !footballer.BestSkillType.HasValue)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer validFootballer = new Footballer();
                    validFootballer.Name = footballer.Name;
                    validFootballer.ContractStartDate = DateTime.ParseExact(footballer.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    validFootballer.ContractEndDate = DateTime.ParseExact(footballer.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    validFootballer.PositionType = (PositionType)footballer.PositionType;
                    validFootballer.BestSkillType = (BestSkillType)footballer.BestSkillType;

                    if (!IsValid(validFootballer) || validFootballer.ContractStartDate > validFootballer.ContractEndDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    validFootballers.Add(validFootballer);
                }

                context.Footballers.AddRange(validFootballers);
                validCoach.Footballers = validFootballers;

                validCoaches.Add(validCoach);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, validCoach.Name, validCoach.Footballers.Count));
            }

            context.AddRange(validCoaches);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            ImportTeamDto[] teams = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            List<Team> validTeams = new List<Team>();
            StringBuilder sb = new StringBuilder();

            List<int> validFootBallersIds = context.Footballers.Select(f => f.Id).ToList();

            foreach (ImportTeamDto team in teams)
            {
                if (string.IsNullOrEmpty(team.Name) || string.IsNullOrEmpty(team.Nationality) || !team.Trophies.HasValue || team.Trophies.Value == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Team validTeam = new Team();
                validTeam.Name = team.Name;
                validTeam.Nationality = team.Nationality;
                validTeam.Trophies = team.Trophies.Value;

                if (!IsValid(validTeam))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                team.Footballers = team.Footballers.Distinct().ToList();

                foreach (int footballerId in team.Footballers)
                {
                    if (!validFootBallersIds.Contains(footballerId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    TeamFootballer validTeamFootballer = new TeamFootballer();
                    validTeamFootballer.Team = validTeam;
                    validTeamFootballer.FootballerId = footballerId;

                    validTeam.TeamsFootballers.Add(validTeamFootballer);
                }

                validTeams.Add(validTeam);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, validTeam.Name, validTeam.TeamsFootballers.Count));
            }

            context.Teams.AddRange(validTeams);
            context.SaveChanges();

            return sb.ToString().Trim();


        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
