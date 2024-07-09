using Microsoft.EntityFrameworkCore;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;
using PwdManager.Shared.Data;
using System.ComponentModel.Design;

namespace PwdManager.srv.Services
{
    public class EntreeRepo : IEntreeRepo
    {
        private readonly PwdDbContext _db;
        private readonly IHttpContextAccessor _context;

        public EntreeRepo(IHttpContextAccessor context, PwdDbContext db)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _db = db;
        }
        public async Task<int> Create(Entree entity)
        {
            _db.Entrees.Add(entity);
            
            await Save();

            return entity.Id;
        }

        public async Task<bool> Delete(Entree entity)
        {
            _db.Entrees.Remove(entity);
            return await Save();    
        }

        public async Task<IEnumerable<Entree>?> FindbyCoffreId(int CoffreId)
        {
            Coffre? coffre = await _db.Coffres
                .AsNoTracking()
                .Include(x => x.Entrees).FirstOrDefaultAsync(c => c.Id == CoffreId);
            IEnumerable<Entree>? entrees = coffre?.Entrees?.ToList();
            return entrees;
        }

        public async Task<Entree?> FindbyId(int id)
        {
            return await _db.Entrees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);  
        }

        public Task<IEnumerable<Entree>> GetItems()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Entree>> GetItemsByUserId(string uid)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            int change = await _db.SaveChangesAsync();
            return change > 0;
        }

        public async Task<bool> Update(Entree entity)
        {
            _db.Entrees.Update(entity);
            return await Save();
        }
    }
}
