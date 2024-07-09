using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;
using PwdManager.Shared.Data;
using PwdManager.Shared;
using PwdDbContext = PwdManager.srv.Data.PwdDbContext;

namespace PwdManager.srv.Services
{
    public class UserCoffreRepo : IUserCoffreRepo
    {
        #region Properties

        private readonly PwdDbContext _db;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor

        public UserCoffreRepo(PwdDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        #endregion
        #region Methods

        public async Task<bool> AddUserToCoffreWithRead(string uid, int coffreId)
        {
            ApiUserCoffre obj = new ApiUserCoffre { UserId=""};
            obj.UserId = uid;
            obj.CoffreId = coffreId;
            obj.Access = Access.R;
            if (! (isExists(obj.UserId, obj.CoffreId).Result))
            {
                await _db.ApiUserCoffres.AddAsync(obj);
            }
            else
            {
                _db.ApiUserCoffres.Update(obj);
            }
            return await Save();
        }

        public async Task<bool> AddUserToCoffresWithRead(string uid, List<int> coffreId)
        {

            foreach (var cid in coffreId)
            {

                ApiUserCoffre obj =new ApiUserCoffre { UserId = ""};
                obj.CoffreId = cid;
                obj.UserId = uid;
                obj.Access = Access.R;
                if (!  ( isExists(obj.UserId, obj.CoffreId).Result))
                {
                    _db.ApiUserCoffres.Add(obj);
                }
                else
                {
                    _db.ApiUserCoffres.Update(obj);
                }
            }
                     
            return await Save();
        }

        public async Task<bool> AdminToCoffre(string uid, int coffreId)
        {
            ApiUserCoffre obj = new ApiUserCoffre { UserId = "" };
            obj.CoffreId = coffreId;
            obj.UserId = uid;
            obj.Access = Access.Admin;
            if (!(await isExists(obj.UserId, obj.CoffreId)))
            {
                await _db.ApiUserCoffres.AddAsync(obj);
            }
            return await Save();
        }



        public async Task<bool> isExists(string uid, int coffreId)
        {

            
            return await _db.ApiUserCoffres.AnyAsync(a => (a.UserId == uid && a.CoffreId == coffreId));
        }

        public Task<bool> RemoveUserFromCoffre(string uid, int coffreId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromCoffres(string uid, List<int> coffreId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            int change= await _db.SaveChangesAsync();
            return change > 0;
        }

        public async Task<bool> AddUserToCoffreWithReadWrite(string uid, int coffreId)
        {
            ApiUserCoffre obj = new()
            {
                UserId = uid,
                CoffreId = coffreId,
                Access = Access.RW
            };
            if (!(await isExists(obj.UserId, obj.CoffreId)))
            {
                await _db.ApiUserCoffres.AddAsync(obj);
            }
            else
            {
                _db.ApiUserCoffres.Update(obj);
            }
            return await Save();
        }

        public async  Task<bool> AddUserToCoffresWithReadWirte(string uid, List<int> coffreId)
        {
            IEnumerable<Task> tasks = coffreId.Select(async cid => {
                ApiUserCoffre obj = new ApiUserCoffre { UserId = "" };
                obj.CoffreId = cid;
                obj.UserId = uid;
                obj.Access = Access.RW;
                if (!(await isExists(obj.UserId, obj.CoffreId)))
                {
                    await _db.ApiUserCoffres.AddAsync(obj);
                }
                else
                {
                    _db.ApiUserCoffres.Update(obj);
                }
            });
            await Task.WhenAll(tasks);
            return await Save();
        }

        public async Task<bool> AdminToCoffres(string uid, List<int> coffreId)
        {
            IEnumerable<Task> tasks = coffreId.Select(async cid => {
                ApiUserCoffre obj = new ApiUserCoffre { UserId = ""};
                obj.CoffreId = cid;
                obj.UserId = uid;
                obj.Access = Access.Admin;
                if (!(await isExists(obj.UserId, obj.CoffreId)))
                {
                    await _db.ApiUserCoffres.AddAsync(obj);
                }
                else
                {
                    _db.ApiUserCoffres.Update(obj);
                }
            });
            await Task.WhenAll(tasks);
            return await Save();
        }

        #endregion
        #region Unused

        public Task<IEnumerable<ApiUserCoffre>> GetItems()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApiUserCoffre>> GetItemsByUserId(string uid)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
