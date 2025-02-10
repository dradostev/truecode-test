using System.Globalization;
using AutoMapper;
using TrueCode.Services.Fetcher.DTO;
using TrueCode.Services.Fetcher.Models;

namespace TrueCode.Services.Fetcher.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Valute, Currency>()
            .ForMember(dest => dest.Id, 
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Code, 
                opt => opt.MapFrom(
                    src => src.CharCode))
            .ForMember(dest => dest.Rate,
                opt => opt.MapFrom(
                    src => decimal.Parse(src.Value.Replace(',', '.'), 
                        CultureInfo.InvariantCulture) / src.Nominal));
    }
}