namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    using Footballers.Data.Models.Enums;
    using AutoMapper;
    using Footballers.Utilities;
    using Footballers.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FootballersProfile>();
            }));

            XmlHelper xmlHelper = new XmlHelper();

            List<Coach> coaches = context.Coaches
                .Where(c => c.Footballers.Count >= 1)
                .Include(c => c.Footballers)
                .OrderByDescending(c => c.Footballers.Count)
                .ThenBy(c => c.Name)
                .AsNoTracking()
                .ToList();

            foreach (Coach coach in coaches)
            {
                coach.Footballers = coach.Footballers.OrderBy(f => f.Name).ToList();
            }

            List<ExportCoachDto> exportCoaches = mapper.Map<List<ExportCoachDto>>(coaches);

            return xmlHelper.Serialize<List<ExportCoachDto>>(exportCoaches, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teams = context.Teams
                .Include(t=>t.TeamsFootballers)
                .ThenInclude(t=>t.Footballer)
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .Take(5)
                .ToList()
                .Select(t => new
                {
                    t.Name,
                    Footballers = t.TeamsFootballers
                        .Where(tf => tf.Footballer.ContractStartDate >= date)
                        .OrderByDescending(f => f.Footballer.ContractEndDate)
                        .ThenBy(f => f.Footballer.Name)
                        .Select(f => new
                        {
                            FootballerName = f.Footballer.Name,
                            ContractStartDate = f.Footballer.ContractStartDate.ToString("d"),
                            ContractEndDate = f.Footballer.ContractEndDate.ToString("d"),
                            BestSkillType = f.Footballer.BestSkillType.ToString(),
                            PositionType = f.Footballer.PositionType.ToString()
                        })
                        .ToList()
                })
                .OrderByDescending(t => t.Footballers.Count)
                .ThenBy(t => t.Name)
                .ToList();

            return JsonConvert.SerializeObject(teams, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }
}
