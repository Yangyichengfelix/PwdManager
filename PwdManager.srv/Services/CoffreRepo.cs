using AutoMapper;
using Microsoft.EntityFrameworkCore;

using PwdManager.Shared.Dtos.Coffres;
using PwdManager.srv.Contracts;
using PwdManager.Shared.Data;


using PwdDbContext = PwdManager.srv.Data.PwdDbContext;

namespace PwdManager.srv.Services
{
    public class CoffreRepo:ICoffreRepo
    {
        private readonly ILogger<CoffreRepo> _logger;
        private readonly PwdDbContext _db;
        private readonly IHttpContextAccessor _context;
        private IMapper _mapper;
        public CoffreRepo(IHttpContextAccessor context, PwdDbContext db, IMapper mapper, ILogger<CoffreRepo> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _db=db;
            _mapper=mapper;
            _logger = logger;
        }

        public async Task<int> Create(Coffre entity)
        {
            _db.Coffres.Add(entity);
                await Save();
            return entity.Id;

        }



        public async Task<bool> Delete(Coffre entity)
        {
            _db.Coffres.Remove(entity);
            return await Save();
        }



        public async Task<Coffre?> FindbyId(int id)
        {
            return await _db.Coffres
                .AsNoTracking()
                .Include(a => a.Entrees)
                .Include(a => a.ApiUserCoffres)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x=>x.Id==id);
        }

        public async Task<Coffre?> FindbyTitle(string tilte)
        {
            return await _db.Coffres
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Title == tilte);
        }


        public async Task<IEnumerable<Coffre>> GetItems()
        {


            return await _db.Coffres.AsNoTracking()
                .Include(a=>a.Entrees)
                .Include(a => a.ApiUserCoffres)
                .ToListAsync();

        }

        public async Task<IEnumerable<Coffre>> GetItemsByUserId(string uid)
        {

            var items =await (from mu in _db.ApiUserCoffres
                         join u in _db.Apiusers on mu.UserId equals u.UserId
                         join m in _db.Coffres on mu.CoffreId equals m.Id
                         where mu.UserId == uid
                         select new Coffre()
                        {
                            Id=m.Id,
                             PasswordHash=m.PasswordHash,
                             Salt=m.Salt,
                             Title = m.Title,
                             Description=m.Description,
                             Created=m.Created,
                             Modified=m.Modified,
                             ApiUserCoffres= m.ApiUserCoffres,
                             Entrees = m.Entrees,
                         }).ToListAsync();

            return items;
         
        }

        public async Task<IEnumerable<CoffreEntreeReadOnlyDto>> GetItemsByUserIdYield(string uid)
        {
            List<Coffre> items = await(from mu in _db.ApiUserCoffres
                         join u in _db.Apiusers on mu.UserId equals u.UserId
                         join m in _db.Coffres.Include(x=>x.ApiUserCoffres).Include(x=>x.Entrees) on mu.CoffreId equals m.Id
                         where mu.UserId == uid
                         select m).ToListAsync();

            IList<CoffreEntreeReadOnlyDto> result = _mapper.Map<IList<CoffreEntreeReadOnlyDto>>(items);
            return result;
        }

        public async Task<IEnumerable<CoffreEntreeReadOnlyDto>> GetItemsYield()
        {
            List<Coffre> coffre= await _db.Coffres.AsNoTracking()
                .Include(a => a.Entrees)
                .Include(a => a.ApiUserCoffres)
                .ToListAsync();
            foreach (Coffre? item in coffre)
            {
                _logger.LogDebug(item.Description);
            }
            var result = _mapper.Map<IList<CoffreEntreeReadOnlyDto>>(coffre);
                return result;
        }

        public async Task<bool> Save()
        {
            int change = await _db.SaveChangesAsync();
            return change > 0;
            
        }

        public async Task<bool> Update(Coffre entity)
        {
            _db.Coffres.Update(entity);
            return await Save();
        }


    }
}
