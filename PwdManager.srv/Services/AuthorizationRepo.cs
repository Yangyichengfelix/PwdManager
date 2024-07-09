using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;
using PwdManager.srv.Models;
using PwdManager.Shared;
using PwdManager.Shared.Data;
using PwdDbContext = PwdManager.srv.Data.PwdDbContext;

namespace PwdManager.srv.Services
{
    public class AuthorizationRepo : IAuthorizationRepo
    {
        #region properties

        private readonly IHttpContextAccessor _context;
        private readonly PwdDbContext _db;
        #endregion
        #region constructor


        public AuthorizationRepo(IHttpContextAccessor context, PwdDbContext db)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _db = db;
        }
        #endregion

        #region methods

        public async Task<bool> VerifyAdminAccess(int coffreId)
        {
            bool result = false;

            List<Coffre?> coffres = await GetAdminItems();
            if (coffres != null)
            {
                result = coffres.Any(x => x?.Id == coffreId);
            }
            return result;
        }
        
        public async Task<bool> VerifyReadAccess(int coffreId)
        {
            bool result = false;

            List<Coffre?> coffres = await GetReadItems();
            if (coffres != null)
            {
                result = coffres.Any(x => x?.Id == coffreId);
            }
            return result;
        }

        public async Task<bool> VerifyWriteAccess(int coffreId)
        {
            bool result = false;
            List<Coffre?> coffres = await GetWriteItems();
            if (coffres!=null)
            {
               result = coffres.Any(x => x?.Id == coffreId);
            }
            return result;
        }


        private async Task<List<Coffre?>> GetReadItems()
        {
            List<Coffre?> coffres = new ();
            string? azureId = _context?.HttpContext?.User?.Identity?.Name;
            ApiUser? user = await _db.Apiusers.AsNoTracking()
                .Include(x => x.ApiUserCoffres)
                .ThenInclude(s => s.Coffre)
                .FirstOrDefaultAsync(x => x.AzureId == azureId);
            if (user is not null)
            {
                coffres = user.ApiUserCoffres.Where(x => x.Access == Access.R).Select(u => u.Coffre).ToList();
            }
            return coffres;
        }
        private async Task<List<Coffre?>> GetWriteItems()
        {
            List<Coffre?> coffres = new();

            string? azureId = _context?.HttpContext?.User?.Identity?.Name;

            ApiUser? user = await _db.Apiusers.AsNoTracking()
                .Include(x => x.ApiUserCoffres)
                .ThenInclude(s => s.Coffre)
                .FirstOrDefaultAsync(x => x.AzureId == azureId);
            if (user is not null)
            {
                coffres = user.ApiUserCoffres.Where(x => x.Access == Access.RW).Select(u => u.Coffre).ToList();
            }
            return coffres;
        }
        private async Task<List<Coffre?>> GetAdminItems()
        {
            List<Coffre?> coffres = new();
            string? azureId = _context?.HttpContext?.User?.Identity?.Name;

            ApiUser? user = await _db.Apiusers.AsNoTracking()
                .Include(x => x.ApiUserCoffres)
                .ThenInclude(s => s.Coffre)
                .FirstOrDefaultAsync(x => x.AzureId == azureId);
            if (user is not null)
            {
               coffres = user.ApiUserCoffres.Where(x=>x.Access==Access.Admin).Select(u => u.Coffre).ToList();
            }
            return coffres;
        }
        #endregion

    }
}
