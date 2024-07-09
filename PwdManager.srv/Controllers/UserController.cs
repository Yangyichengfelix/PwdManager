using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.srv.Contracts;
using PwdManager.srv.Data;

namespace PwdManager.srv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
 

        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepo _userRepo;
       // private readonly IAuthorizationRepo _authorizationRepo;
        private readonly PwdDbContext _db;
        public UserController(
            ILogger<UserController> logger,
            IMapper mapper,
            PwdDbContext db,
            IUserRepo userRepo,
        IHttpContextAccessor httpContext
        )
        {
            _logger = logger;
            _mapper = mapper;
            _db = db;
            _userRepo = userRepo;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async IAsyncEnumerable<ApiUser> GetAll()
        {
            _logger.LogDebug($"GetAll {nameof(GetAll)} users");
            string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
            ApiUser? user = await _userRepo.CheckUser(azureId);
            if (user == null)
            {
                await _userRepo.AddUser(azureId);
            }
            user = await _userRepo.CheckUser(azureId);
            foreach (var item in await _db.Apiusers.ToListAsync())
            {
                yield return item;
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async IAsyncEnumerable<ApiUser> Search(string keyword)
        {
            _logger.LogDebug($"Search {nameof(Search)} users");
            string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
            ApiUser? user = await _userRepo.CheckUser(azureId);
            if (user == null)
            {
                await _userRepo.AddUser(azureId);
            }
            user = await _userRepo.CheckUser(azureId);
            foreach (var item in await _db.Apiusers.AsNoTracking().Include(x=>x.ApiUserCoffres).ThenInclude(s => s.Coffre)
                .Where(x=>x.AzureId.ToLower().Contains(keyword.ToLower())).ToListAsync())
            {
                yield return item;
            }
        }
    }
}
