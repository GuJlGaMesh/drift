using drift.Data.Entity;
using drift.Models.Dto;

namespace drift.Utils
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Competition, CompetitionDto>()
                .ForMember("CreatorUserName",
                    comp => comp.MapFrom(c => c.CreatedBy.UserName)).ReverseMap();
            CreateMap<Car, CarDto>().ReverseMap();
            CreateMap<CompetitionApplication, CompetitionApplicationDto>()
                .ForMember(x => x.ApplicationId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.ParticipantName, y => y.Ignore()).ReverseMap();
        }
    }
}