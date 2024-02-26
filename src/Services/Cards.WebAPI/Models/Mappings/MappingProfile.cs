using AutoMapper;

using Core.Domain.Entities;
using Core.Management.Common.Projections;
using Cards.WebAPI.Models.DTOs.Responses;

namespace Cards.WebAPI.Models.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureMappings();
    }

    private void ConfigureMappings()
    {
        CreateMap<User, UserDto>()
            .ForMember(destinationMember => destinationMember.UserId, options => options.MapFrom(src => src.Id));
        CreateMap<Card, CardDto>();
        CreateMap<CardStatus, CardStatusDto>();
        CreateMap(typeof(PaginatedList<>), typeof(Pagination));
    }
}