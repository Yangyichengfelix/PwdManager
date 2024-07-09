using AutoMapper;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.EntreeLogs;
using PwdManager.Shared.Dtos.Entrees;
using PwdManager.Shared.Dtos.UserCoffres;
using PwdManager.srv.Models;
namespace PwdManager.srv.Configs
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<ApiUserCoffre, UserCoffreReadOnlyDto>()
                .ForMember(a => a.CoffreTitle, d => d.MapFrom(x => x.Coffre.Title.ToString()))
                .ForMember(a => a.AzureId, d => d.MapFrom(x => x.ApiUser.AzureId.ToString()))
                .ForMember(a => a.UserId, d => d.MapFrom(x => x.ApiUser.UserId.ToString())) 
                .ReverseMap();
            CreateMap<ApiUserCoffre, UserCoffreDto>()
                .ReverseMap();

            CreateMap<Coffre, CoffreCreateDto>().ReverseMap();
            CreateMap<Coffre, CoffreUpdateDto>().ReverseMap();
            CreateMap<Coffre, CoffreLogReadOnlyDto>()
                .ForMember(a => a.CoffreLogs, d => d.MapFrom(x => x.CoffreLogs.ToList()))
                .ForMember(a => a.ApiUserCoffres, d => d.MapFrom(x => x.ApiUserCoffres.ToList()))
                .ReverseMap();
            CreateMap<Coffre, CoffreEntreeReadOnlyDto>()
                .ForMember(a => a.Entrees, d => d.MapFrom(x => x.Entrees.ToList()))
                .ForMember(a => a.ApiUserCoffres, d => d.MapFrom(x => x.ApiUserCoffres.ToList()))
                .ReverseMap();
            CreateMap<CoffreLog, CoffreLogDto>()
                .ForMember(a => a.Operation, d => d.MapFrom(x => x.Operation.ToString()))
                .ForMember(a => a.CoffreTitle, d => d.MapFrom(x => x.Coffre.Title.ToString()))
                .ForMember(a => a.CoffreDescription, d => d.MapFrom(x => x.Coffre.Description.ToString()))
                .ReverseMap();
            CreateMap<CoffreLog, CoffreLogNotificationData>()
                .ForMember(a => a.Operation, d => d.MapFrom(x => x.Operation.ToString()))
                .ForMember(a => a.AzureId, d => d.MapFrom(x => x.ApiUser.AzureId.ToString()))
                .ForMember(a => a.CoffreTitle, d => d.MapFrom(x => x.Coffre.Title.ToString()))
                .ForMember(a => a.CoffreDescription, d => d.MapFrom(x => x.Coffre.Description.ToString()))
                .ReverseMap();
            CreateMap<EntreeHistory, EntreeLogDto>().ReverseMap();
            CreateMap<Entree, EntreeDto>().ReverseMap();
            CreateMap<Entree, EntreeCreateDto>().ReverseMap();
            CreateMap<Entree, EntreeReadOnlyDto>()
                .ForMember(a => a.CoffreTitle, d => d.MapFrom(x => x.Coffre.Title.ToString()))
                .ReverseMap();

        }
    }
}
