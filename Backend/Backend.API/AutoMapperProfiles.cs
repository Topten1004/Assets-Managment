using AutoMapper;
using Backend.API.ViewModel;
using Backend.Data.Entities;

namespace Backend.API
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {

            CreateMap<AssetEntity, GetAssetVM>().ReverseMap();
            CreateMap<AssetEntity, PostAssetVM>().ReverseMap();
            CreateMap<CommandEntity, CommandVM>().ReverseMap();
            CreateMap<UserEntity, UserVM>().ReverseMap();
            CreateMap<SocketCommandVM, CommandVM>().ReverseMap();
            CreateMap<CommandEntity, SocketCommandVM>().ReverseMap(); 
            CreateMap<LogEntity, LogVM>().ReverseMap();
        }
    }
}
