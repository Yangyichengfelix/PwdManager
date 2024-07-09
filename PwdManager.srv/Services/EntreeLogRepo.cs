using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.EntreeLogs;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;
using PwdManager.srv.Models;

namespace PwdManager.srv.Services
{
    public class EntreeLogRepo: IEntreeLogRepo
    {
        private readonly PwdDbContext _db;
        private readonly IHttpContextAccessor _context;
        private IMapper _mapper;

        public EntreeLogRepo(IHttpContextAccessor context, PwdDbContext db, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> Add(EntreeHistory log)
        {
            _db.EntreeLogs.Add(log);
            return await Save();
        }

        public async Task<IList<EntreeHistory>> ReadEntreeLogByCoffreId(int coffreId)
        {
            List<Entree>? entrees = new();
            List<EntreeHistory> histories = new();
            Coffre? coffre = await _db.Coffres.AsNoTracking()
                .Include(x => x.Entrees)
                .ThenInclude(x=>x.EntreeHistories)
                .FirstOrDefaultAsync(c => c.Id == coffreId);
            entrees = coffre?.Entrees.ToList();

            if (entrees is not null)
            {

                histories = (List<EntreeHistory>)entrees.Select(u => u.EntreeHistories);
            }

            return (IList<EntreeHistory>)histories;

        }

        public async Task<IList<EntreeHistory>> ReadEntreeLogByEntreeId(int entreeId)
        {
            List<Entree> entrees = await _db.Entrees.AsNoTracking()
                .Include(x => x.EntreeHistories).ToListAsync();
            return (IList<EntreeHistory>)(entrees.Select(u => u.EntreeHistories).ToList());
        }

        public async Task<IList<EntreeHistory>> ReadEntreeLogByUserId(string azureId)
        {
            return await _db.EntreeLogs.AsNoTracking()
                 .Include(x => x.ApiUser)
                 .Where(x => x.ApiUser.AzureId == azureId)
                 .ToListAsync();
        }

        public async Task<IList<EntreeHistory>> ReadEntreeLogWithTimeRange(DateTime start, DateTime end)
        {
            return await _db.EntreeLogs.AsNoTracking()
                .Where(x => ((x.DateOperation >= start && x.DateOperation <= end)))
                .ToListAsync();
        }

        public async Task<IEnumerable<EntreeLogNotificationData>> ReadEntreeLogWithTimeRangeYield(DateTime start, DateTime end, string azureId)
        {
            List<EntreeHistory> logs = await _db.EntreeLogs.AsNoTracking()
            .Include(a => a.ApiUser)
            .Where(x => x.ApiUser.AzureId == azureId)
            .Where(x => ((x.DateOperation >= start && x.DateOperation <= end)))
            .ToListAsync();
            var result = _mapper.Map<IList<EntreeLogNotificationData>>(logs);
            return result;
        }

        public async Task<bool> Save()
        {
            int change = await _db.SaveChangesAsync();
            return change > 0;
        }
    }
}
