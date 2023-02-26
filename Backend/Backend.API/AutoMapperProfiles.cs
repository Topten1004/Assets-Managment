using AutoMapper;
using Backend.API.ViewModel;
using Backend.Data.Entities;

namespace Backend.API
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {

            CreateMap<AssetEntity, AssetVM>().ReverseMap();
            CreateMap<CommandEntity, CommandVM>().ReverseMap();
        }
    }
}
