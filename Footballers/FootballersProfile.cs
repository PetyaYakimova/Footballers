namespace Footballers
{
    using AutoMapper;
    using Footballers.Data.Models;
    using Footballers.DataProcessor.ExportDto;

    // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
    public class FootballersProfile : Profile
    {
        public FootballersProfile()
        {
            this.CreateMap<Footballer, ExportFootballerDto>()
                 .ForMember(d => d.Position, opt => opt.MapFrom(s => s.PositionType.ToString()));

            this.CreateMap<Coach, ExportCoachDto>()
                .ForMember(d => d.Footballers, opt => opt.MapFrom(s => s.Footballers))
                .ForMember(d => d.FootballersCount, opt => opt.MapFrom(s => s.Footballers.Count));
        }
    }
}
