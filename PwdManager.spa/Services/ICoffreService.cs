
using PwdManager.Shared.Dtos;
using PwdManager.Shared.Dtos.Coffres;
namespace PwdManager.spa.Services
{
    public interface ICoffreService:IBaseService
    {

        Task<List<CoffreEntreeReadOnlyDto>?> GetCoffres();
        Task<Stream> GetRelativeCoffres();
        Task<CoffreEntreeReadOnlyDto?> GetCoffreById(int id);
        Task<HttpResponseMessage> AddCoffre(CoffreCreateDto coffreCreateDto);
        Task<HttpResponseMessage> UpdateCoffre(CoffreUpdateDto coffreUpdateDto);
        Task<HttpResponseMessage> RemoveCoffre(int id);
    }
}
