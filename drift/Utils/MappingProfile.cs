﻿using drift.Data.Entity;
using drift.Models.Dto;

namespace drift.Utils
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Competition, CompetitionDto>()
                .ForMember("CreatorUserName",
                    comp => comp.MapFrom(c => c.CreatedBy.UserName));
            
        }
    }
}