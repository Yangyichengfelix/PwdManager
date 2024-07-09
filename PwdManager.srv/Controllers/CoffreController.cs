using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.Security;
using Microsoft.Graph.Models;
using PwdManager.srv.Contracts;
using PwdManager.srv.Models;
using PwdManager.Shared.Dtos.Coffres;
using Microsoft.EntityFrameworkCore;
using PwdManager.Shared;
using PwdManager.srv.Services;
using PwdManager.Shared.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PwdManager.srv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[Authorize]
    public class CoffreController : ControllerBase
    {
        #region Porperties

        private readonly ILogger<CoffreController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationRepo _authorizationRepo;
        private readonly ICoffreRepo _coffreRepo;
        private readonly ICoffreLogRepo _coffreLogRepo;
        private readonly IUserCoffreRepo _userCoffreRepo;
        private readonly IUserRepo _userRepo;
        private readonly IHttpContextAccessor _httpContext;
        #endregion
        #region Constructor


        public CoffreController(
            ILogger<CoffreController> logger,
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
            _authorizationRepo= authorizationRepo;
            _coffreRepo= coffreRepo;
            _coffreLogRepo= coffreLogRepo;
            _userCoffreRepo = userCoffreRepo;
            _userRepo= userRepo;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }
        #endregion

        #region Read multi

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
        public async IAsyncEnumerable<CoffreEntreeReadOnlyDto> GetAll()
        {
            _logger.LogDebug($"GetAll {nameof(GetAll)} Coffre");

       
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
       
                ApiUser? user = await _userRepo.CheckUser(azureId);
                if (user == null)
                {
                    await _userRepo.AddUser(azureId);
                }
                user = await _userRepo.CheckUser(azureId);
                IEnumerable<Coffre> coffre = await _coffreRepo.GetItems();

                foreach (var item in await _coffreRepo.GetItemsYield())
                {
                    // logger.LogInformation($"Debug {item.Description} stream");
                    yield return item;
                }
        }
        [HttpGet("user")]
        public async IAsyncEnumerable<CoffreEntreeReadOnlyDto> GetUserCoffre()
        {
            _logger.LogDebug($"GetAll {nameof(GetUserCoffre)} Coffre");


            string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
            
            ApiUser? user = await _userRepo.CheckUser(azureId);
            if (user == null)
            {
                await _userRepo.AddUser(azureId);
            }
            user = await _userRepo.CheckUser(azureId) ?? throw new Exception("user null or azureId not found");
            // var coffre = await _coffreRepo.GetItems();

            foreach (var item in await _coffreRepo.GetItemsByUserIdYield(user.UserId))
            {
                // logger.LogInformation($"Debug {item.Description} stream");

                yield return item;
            }

        }
        // GET: api/<CoffreController>
        [HttpGet("user/old")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserCoffreOld()
        {
            _logger.LogDebug($"GetAll {nameof(GetUserCoffreOld)} Coffre");


            string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
            if (String.IsNullOrWhiteSpace(azureId))
            {
                return Unauthorized();
            }
            ApiUser? user = await _userRepo.CheckUser(azureId);
            if (user == null)
            {
                await _userRepo.AddUser(azureId);
            }
            user = await _userRepo.CheckUser(azureId) ?? throw new Exception("user null or azureId not found");
            IEnumerable<Coffre> coffre = await _coffreRepo.GetItemsByUserId(user.UserId);
            IList<CoffreEntreeReadOnlyDto> items= _mapper.Map<IList<CoffreEntreeReadOnlyDto>>(coffre);

            return Ok(items);
        

        }
        #endregion
        #region Read one by Id

        // GET api/<CoffreController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoffreById(int id)
        {

            _logger.LogDebug($"Get {nameof(GetCoffreById)} {id}");
            ActionResult? actionResult = null;

            try
            {
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");

                ApiUser? user = await _userRepo.CheckUser(azureId);
                if (user == null)
                {
                    await _userRepo.AddUser(azureId);
                }
                user = await _userRepo.CheckUser(azureId);
                Coffre? coffre = await _coffreRepo.FindbyId(id);
                
                if (coffre==null)
                {
                    actionResult = NotFound();
                }
                else
                {

                    CoffreEntreeReadOnlyDto response = _mapper.Map<CoffreEntreeReadOnlyDto>(coffre);

                    CoffreLog log = new CoffreLog
                    {
                        DateOperation = DateTime.UtcNow,
                        Operation = PwdManager.Shared.Operation.Read,
                        UserId = user?.UserId ?? "",
                        CoffreId =id,
                        CoffreName=id+" "+ coffre.Title
                    };
                    await _coffreLogRepo.Add(log);
                    actionResult = Ok(response);
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetCoffreById)}");
                return Problem($"Something went wrong in the {nameof(GetCoffreById)}", statusCode: 500);
            }
        }
        #endregion
        #region Create

        // POST api/<CoffreController>
        [HttpPost]
        public async Task<IActionResult> CreateCoffre([FromBody] CoffreCreateDto dto)
        {
            _logger.LogDebug($"Create {nameof(CreateCoffre)} {dto.Title}");
            ActionResult actionResult = NotFound();
            try
            {
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");

                ApiUser? user=await _userRepo.CheckUser(azureId);
                _logger.LogDebug($"Check user with {azureId}");

                if (user==null)
                {
                    await _userRepo.AddUser(azureId);
                }
                user = await _userRepo.CheckUser(azureId) ?? throw new Exception("user null or azureId not found"); 
                if (string.IsNullOrEmpty(dto.Title)||string.IsNullOrEmpty(dto.Description))
                {
                    actionResult = BadRequest("Vault title or description can't be empty");
                }
                _logger.LogDebug($"Check vault with name {dto.Title}");

                Coffre? check = await _coffreRepo.FindbyTitle(dto.Title);
                if (check!=null)
                {
                    actionResult = Conflict("Title aleady exists");
                }
                else
                {
                    _logger.LogDebug($"mapping db result with vault creation model {dto.Title}");

                    Coffre coffre = _mapper.Map<Coffre>(dto);
                    coffre.Created = DateTime.UtcNow;
                    coffre.Modified = DateTime.UtcNow;
                    int response = await _coffreRepo.Create(coffre);
                    //Coffre? created = await _coffreRepo.FindbyTitle(coffre.Title);

                    if (response>1)
                    {
                        await _userCoffreRepo.AdminToCoffre(user.UserId, response);
                    }

                    CoffreLog log = new CoffreLog
                    {
                        DateOperation = DateTime.UtcNow,
                        Operation = PwdManager.Shared.Operation.Create,
                        UserId = user.UserId ?? "",
                        CoffreId = response,
                        CoffreName = response + " " + coffre.Title
                    };
                    await _coffreLogRepo.Add(log);
                    actionResult=Created("Created", new { coffre });
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateCoffre)}");
                return Problem($"Something went wrong in the {nameof(CreateCoffre)}", statusCode: 500);
            }
        }
        #endregion
        #region Update

        // PUT api/<CoffreController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoffre(int id, [FromBody] CoffreUpdateDto dto)
        {
            ActionResult actionResult = NotFound();
            try
            {
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
                ApiUser? user = await _userRepo.CheckUser(azureId);
                if (user == null)
                {
                    await _userRepo.AddUser(azureId);
                }
                user = await _userRepo.CheckUser(azureId);
                if (id!=dto.Id)
                {
                    actionResult = BadRequest();
                }
                Coffre? check = await _coffreRepo.FindbyTitle(dto.Title.ToLowerInvariant());
                if (check != null)
                {
                    actionResult = Conflict("Title aleady exists");
                }
                Coffre? coffreToUpdate = await _coffreRepo.FindbyId(dto.Id);
                if (coffreToUpdate==null)
                {
                    actionResult = NotFound($"{id} of coffre not found");
                }
                else
                {
                    bool isAdmin = await _authorizationRepo.VerifyAdminAccess(coffreToUpdate.Id);
                    if (!isAdmin)
                    {
                        actionResult = Forbid("you are not admin of this vault!");
                        return actionResult;
                    }
                    Coffre coffre = _mapper.Map<Coffre>(dto);
                    coffre.Modified = DateTime.UtcNow;
                    bool updated = await _coffreRepo.Update(coffre);
                    if (updated==true)
                    {
                        CoffreLog log = new CoffreLog
                        {
                            DateOperation = DateTime.UtcNow,
                            Operation = PwdManager.Shared.Operation.Update,
                            UserId = user?.UserId??"",
                            CoffreId = dto.Id,
                            CoffreName=dto.Id+" "+dto.Title
                        };
                        await _coffreLogRepo.Add(log);
                        actionResult = Accepted("Updated", new { dto });
                    }
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCoffre)}");
                return Problem($"Something went wrong in the {nameof(UpdateCoffre)}", statusCode: 500);
            }
        }
        #endregion

        // DELETE api/<CoffreController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoffre(int id)
        {
            ActionResult actionResult = NotFound();
            try
            {
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
                ApiUser? user = await _userRepo.CheckUser(azureId);
                if (user == null)
                {
                    await _userRepo.AddUser(azureId);
                }
                user = await _userRepo.CheckUser(azureId) ?? throw new Exception("user null or azureId not found");
                Coffre? coffreToRemove = await _coffreRepo.FindbyId(id);
                if (coffreToRemove == null)
                {
                    actionResult = NotFound($"{id} of entree not found");
                }
                else
                {
                    bool isAdmin = await _authorizationRepo.VerifyAdminAccess(coffreToRemove.Id);
                    if (!isAdmin)
                    {
                        actionResult = Forbid("you are not admin of this vault!");
                        return actionResult;
                    }
                    bool deleted = await _coffreRepo.Delete(coffreToRemove);
                    if (deleted == true)
                    {
                        CoffreLog log = new CoffreLog
                        {
                            DateOperation = DateTime.UtcNow,
                            Operation = PwdManager.Shared.Operation.Delete,
                            UserId = user.UserId,
                            //CoffreId = id,
                            CoffreName = id + " "+coffreToRemove.Title
                        };
                        await _coffreLogRepo.Add(log);
                        actionResult = Ok("Deleted");
                    }
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteCoffre)}");
                return Problem($"Something went wrong in the {nameof(DeleteCoffre)}", statusCode: 500);
            }
        }
    }
}
