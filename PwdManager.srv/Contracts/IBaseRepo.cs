using PwdManager.srv.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PwdManager.srv.Contracts
{
    public interface IBaseRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetItems();
      
        Task<IEnumerable<T>> GetItemsByUserId(string uid);
       
        Task<bool> Save();

    }
}
