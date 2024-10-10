using AutoMapper;
using Crud.Application.Dtos;
using Crud.Domain.Models;

namespace Crud.Application.MappingProfiles;
internal class QuoteProfile : Profile
{
    public QuoteProfile()
    {
        CreateMap<Quote, QuoteDto>();  // Mapping from entity to dto
        CreateMap<CreateUpdateQuoteDto, Quote>()   // Mapping from dto to entity
            .ForMember(dest => dest.TextLength, 
                opt => opt.MapFrom(x=>x.Text.Length)); //map from calculated text length
    }
}
