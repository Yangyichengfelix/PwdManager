using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.Entrees;

namespace PwdManager.spa.Services
{
    public interface IEntreeService
    {
        Task<HttpResponseMessage>AddEntree(EntreeCreateDto entreeCreateDto);
        Task<HttpResponseMessage> UpdateEntree(EntreeDto entreeCreateDto);
        Task<HttpResponseMessage> DeleteEntree(int id);
    }
}
