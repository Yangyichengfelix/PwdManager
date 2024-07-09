using PwdManager.Shared.Data;

namespace PwdManager.srv.Contracts
{
    public interface IUserCoffreRepo:IBaseRepo<ApiUserCoffre>
    {
        Task<bool> isExists(string uid, int coffreId);

        public Task<bool> AddUserToCoffreWithRead(string uid, int coffreId);
        public Task<bool> AddUserToCoffreWithReadWrite(string uid, int coffreId);
        public Task<bool> AdminToCoffre(string uid, int coffreId);
        public Task<bool> AdminToCoffres(string uid, List<int> coffreId);
        public Task<bool> RemoveUserFromCoffre(string uid, int coffreId);
        public Task<bool> AddUserToCoffresWithRead(string uid, List<int> coffreId);
        public Task<bool> AddUserToCoffresWithReadWirte(string uid, List<int> coffreId);
        public Task<bool> RemoveUserFromCoffres(string uid, List<int> coffreId);
    }
}
