using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.Coffres;

namespace PwdManager.spa.Services
{
    public interface IUserService : IBaseService
    {
        Task<HttpResponseMessage> Search(string keyword);

    }
}
