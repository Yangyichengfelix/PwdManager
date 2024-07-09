using PwdManager.Shared.Data;

namespace PwdManager.srv.Contracts
{
    public interface IEntreeRepo : IBaseRepo<Entree>
    {
        Task<int> Create(Entree entity);
        Task<bool> Update(Entree entity);
        Task<bool> Delete(Entree entity);
        Task<Entree?> FindbyId(int id);
        Task<IEnumerable<Entree>?> FindbyCoffreId(int CoffreId);

    }
}
