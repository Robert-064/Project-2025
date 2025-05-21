namespace Project_2025_Web.Data.Entities
{
    using AutoMapper;
    using Project_2025_Web.Data.Entities;
    using Project_2025_Web.DTOs;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
            CreateMap<Plan, PlanDTO>().ReverseMap();
            CreateMap<Plan, PlanDTO>()
             .ForMember(dest => dest.ImagePath1, opt => opt.MapFrom(src => src.ImageUrl1))
             .ForMember(dest => dest.ImagePath2, opt => opt.MapFrom(src => src.ImageUrl2))
             .ReverseMap()
             .ForMember(dest => dest.ImageUrl1, opt => opt.MapFrom(src => src.ImagePath1))
             .ForMember(dest => dest.ImageUrl2, opt => opt.MapFrom(src => src.ImagePath2));
        }
    }

}
