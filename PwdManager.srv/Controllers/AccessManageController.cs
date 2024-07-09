using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PwdManager.Shared;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.UserCoffres;
using PwdManager.srv.Contracts;
using PwdManager.srv.Services;
using System.Collections.Concurrent;
using static Microsoft.Graph.Constants;

namespace PwdManager.srv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class AccessManageController : ControllerBase
    {
        private readonly ILogger<AccessManageController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserCoffreRepo _userCoffreRepo;
        private readonly ICoffreRepo _coffreRepo;

        private readonly ICoffreLogRepo _coffreLogRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepo _userRepo;
        private readonly IAuthorizationRepo _authorizationRepo;
        public AccessManageController(
            ILogger<AccessManageController> logger,
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
            _coffreRepo = coffreRepo;
            _userCoffreRepo = userCoffreRepo;
            _userRepo = userRepo;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }


        #region Add access
        [HttpPost]
        public async Task<IActionResult> CreateAccess([FromBody] AzureCoffreAccessDto dto)
        {
            _logger.LogDebug($"Create read access {nameof(CreateAccess)} for {dto.AzureId} at vault:{dto.CoffreId}, as {dto.Access.ToString()}");
            ActionResult actionResult = NotFound();
            try
            {
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
                ApiUser? adminuser = await _userRepo.CheckUser(azureId);
                if (adminuser == null)
                {
                    await _userRepo.AddUser(azureId);
                }
                adminuser = await _userRepo.CheckUser(azureId);
                ApiUser user= await _userRepo.CheckUser(dto.AzureId) ?? throw new Exception("user null or azureId not found"); 
                Coffre? check = await _coffreRepo.FindbyId(dto.CoffreId);
                if (check == null)
                {
                    actionResult = NotFound("Vault doesn't exist");
                }
                bool accessOk = await _authorizationRepo.VerifyAdminAccess(dto.CoffreId);
                if (!accessOk)
                {
                    actionResult = Unauthorized("You don't has authorization to perform to this operation");
                }
                else
                {
                    bool response = dto.Access switch
                    {
                        Access.R => await _userCoffreRepo.AddUserToCoffreWithRead(user.UserId, dto.CoffreId),
                        Access.RW => await _userCoffreRepo.AddUserToCoffreWithReadWrite(user.UserId, dto.CoffreId),
                        Access.Admin => await _userCoffreRepo.AdminToCoffre(user.UserId, dto.CoffreId),
                        _ => await _userCoffreRepo.AddUserToCoffreWithRead(user.UserId, dto.CoffreId)
                    };

                    //bool response = await _userCoffreRepo.AddUserToCoffreWithRead(user.UserId, dto.CoffreId);
                    if (response)
                    {
                        return Ok("Ok");
                    }
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateAccess)}");
                return Problem($"Something went wrong in the {nameof(CreateAccess)}", statusCode: 500);
            }
        }
        #endregion

    }
}
