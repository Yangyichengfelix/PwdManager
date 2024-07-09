using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;
using PwdManager.Shared.Data;
using PwdDbContext = PwdManager.srv.Data.PwdDbContext;

namespace PwdManager.srv.Services
{
    public class UserRepo : IUserRepo
    {


        #region Properties

        private readonly PwdDbContext _db;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor

        public UserRepo(PwdDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> AddUser(string azureId)
        {
            ApiUser user = new ApiUser
            {
                AzureId = azureId,
                UserId = Guid.NewGuid().ToString(),
            };
             _db.Apiusers.Add(user);
            return await Save();
        }

        public async Task<ApiUser?> CheckUser(string azureId)
        {
            ApiUser? user = await _db.Apiusers.AsNoTracking().FirstOrDefaultAsync(x => x.AzureId == azureId);
            return user;
        }

        public async Task<List<ApiUser>?> Find(string keyword)
        {
            List<ApiUser>? user = await _db.Apiusers.AsNoTracking().Where(x => x.AzureId.Contains(keyword)).ToListAsync(); 
            return user;
        }
        #endregion


        public Task<IEnumerable<ApiUser>> GetItems()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApiUser>> GetItemsByUserId(string uid)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            int change = await _db.SaveChangesAsync();
            return change > 0;
        }
    }
}
