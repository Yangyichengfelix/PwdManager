using PwdManager.Shared.Data;

namespace PwdManager.srv.Contracts
{
    public interface IUserRepo: IBaseRepo<ApiUser>
    {
        Task<bool> AddUser(string azureId);
        Task<ApiUser?> CheckUser(string azureId);
        Task<List<ApiUser>?> Find(string keyword);
    }
}
