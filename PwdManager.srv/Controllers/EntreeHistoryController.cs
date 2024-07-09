using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.EntreeLogs;
using PwdManager.srv.Contracts;

namespace PwdManager.srv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntreeHistoryController : ControllerBase
    {
        private readonly ILogger<EntreeHistoryController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserCoffreRepo _userCoffreRepo;
        private readonly IEntreeLogRepo _entreeLogRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepo _userRepo;
        private readonly IAuthorizationRepo _authorizationRepo;
        public EntreeHistoryController(
    ILogger<EntreeHistoryController> logger,
    IMapper mapper,
    IAuthorizationRepo authorizationRepo,

    IEntreeLogRepo entreeLogRepo,
    IUserCoffreRepo userCoffreRepo,
    IUserRepo userRepo,
    IHttpContextAccessor httpContext
    )
        {
            _logger = logger;
            _mapper = mapper;
            _authorizationRepo = authorizationRepo;

            _entreeLogRepo = entreeLogRepo;
            _userCoffreRepo = userCoffreRepo;
            _userRepo = userRepo;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        // GET: api/<CoffreController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async IAsyncEnumerable<EntreeLogNotificationData> Get(DateTime start)
        {
            _logger.LogDebug($"GetAll {nameof(Get)} entree opération");


            string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
            ApiUser? user = await _userRepo.CheckUser(azureId);
            if (user == null)
            {
                await _userRepo.AddUser(azureId);
            }
            user = await _userRepo.CheckUser(azureId);

            if (user!=null)
            {

                foreach (var item in await _entreeLogRepo.ReadEntreeLogWithTimeRangeYield(start, DateTime.UtcNow,user.AzureId ))
                {
                    // logger.LogInformation($"Debug {item.Description} stream");
                    if (item!=null)
                    {

                        yield return item;
                    }
                }
            }
        }
    }
}
