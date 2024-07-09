using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;
using PwdDbContext = PwdManager.srv.Data.PwdDbContext;


namespace PwdManager.srv.Services
{
    public class CoffreLogRepo : ICoffreLogRepo
    {
        private readonly PwdDbContext _db;
        private readonly IHttpContextAccessor _context;
        private IMapper _mapper;
        public CoffreLogRepo(IHttpContextAccessor context, PwdDbContext db, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> Add(CoffreLog log)
        {
            _db.CoffreLogs.Add(log);
            return await Save();
        }

        public async Task<IList<CoffreLog>> ReadCoffreLogByCoffreId(int coffreId)
        {
            return await _db.CoffreLogs.AsNoTracking()
                .Include(x => x.Coffre)
                .Where(x => x.Coffre.Id == coffreId)
                .ToListAsync();
        }

        public async Task<IList<CoffreLog>> ReadCoffreLogByUserId(string azureId)
        {
            return await _db.CoffreLogs.AsNoTracking()
                .Include(x=>x.ApiUser)
                .Where(x => x.ApiUser.AzureId == azureId )
                .ToListAsync();
        }

        public async Task<IList<CoffreLog>> ReadCoffreLogWithTimeRange(DateTime start, DateTime end)
        {
            return await _db.CoffreLogs.AsNoTracking().Where(x => ((x.DateOperation >= start && x.DateOperation <= end))).ToListAsync();
        }

        public async Task<IEnumerable<CoffreLogNotificationData>> ReadCoffreLogWithTimeRangeYield(DateTime start, DateTime end, string azureId)
        {
            List<CoffreLog> logs = await _db.CoffreLogs.AsNoTracking()
                .Include(a => a.Coffre)
                .Include(a => a.ApiUser)
                           .Where(x => x.ApiUser.AzureId == azureId)
            .Where(x => ((x.DateOperation >= start && x.DateOperation <= end)))
                .ToListAsync();

            var result = _mapper.Map<IList<CoffreLogNotificationData>>(logs);
            return result;
        }

        public async Task<bool> Save()
        {
            int change = await _db.SaveChangesAsync();
            return change > 0;
        }
    }
}
