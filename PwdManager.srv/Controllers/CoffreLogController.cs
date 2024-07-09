using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.srv.Contracts;
using PwdManager.srv.Services;
using System.Net.Http;

namespace PwdManager.srv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffreLogController : ControllerBase
    {
        private readonly ILogger<CoffreLogController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserCoffreRepo _userCoffreRepo;
        private readonly ICoffreLogRepo _coffreLogRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepo _userRepo;
        private readonly IAuthorizationRepo _authorizationRepo;
        public CoffreLogController(
    ILogger<CoffreLogController> logger,
    IMapper mapper,
    IAuthorizationRepo authorizationRepo,
    ICoffreRepo coffreRepo,
    ICoffreLogRepo coffreLogRepo,
    IUserCoffreRepo userCoffreRepo,
    IUserRepo userRepo,
    IHttpContextAccessor httpContext
    )
        {
            _logger = logger;
            _mapper = mapper;
            _authorizationRepo = authorizationRepo;
   
            _coffreLogRepo = coffreLogRepo;
            _userCoffreRepo = userCoffreRepo;
            _userRepo = userRepo;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        // GET: api/<CoffreController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async IAsyncEnumerable<CoffreLogNotificationData> Get([FromBody]DateTime start)
        {
            _logger.LogDebug($"GetAll {nameof(Get)} Coffre opération");


            string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
            ApiUser? user = await _userRepo.CheckUser(azureId);
            if (user == null)
            {
                await _userRepo.AddUser(azureId);
            }
            user = await _userRepo.CheckUser(azureId);

            if (user!=null)
            {
                start=start.ToUniversalTime();
                foreach (var item in await _coffreLogRepo.ReadCoffreLogWithTimeRangeYield(start, DateTime.UtcNow, user.AzureId))
                {
                    // logger.LogInformation($"Debug {item.Description} stream");
                    yield return item;
                }
            }
        }
    }
}
