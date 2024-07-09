using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Data;

namespace PwdManager.srv.Contracts
{
    public interface ICoffreRepo:IBaseRepo<Coffre>
    {
        Task<int> Create(Coffre entity);
        Task<bool> Update(Coffre entity);
        Task<bool> Delete(Coffre entity);
        Task<Coffre?> FindbyId(int id);
        Task<Coffre?> FindbyTitle(string tilte);



        Task<IEnumerable<CoffreEntreeReadOnlyDto>> GetItemsYield();
        Task<IEnumerable<CoffreEntreeReadOnlyDto>> GetItemsByUserIdYield(string uid);
    }
}
