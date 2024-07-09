using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.Entrees;
using PwdManager.srv.Contracts;
using PwdManager.Shared.Data;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PwdManager.srv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntreeController : ControllerBase
    {
        #region Properties

        private readonly ILogger<EntreeController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationRepo _authorizationRepo;
        private readonly ICoffreRepo _coffreRepo;
        private readonly IEntreeRepo _entreeRepo;
        private readonly IEntreeLogRepo _entreeLogRepo;
        private readonly ICoffreLogRepo _coffreLogRepo;
        private readonly IUserCoffreRepo _userCoffreRepo;
        private readonly IUserRepo _userRepo;
        private readonly IHttpContextAccessor _httpContext;
        #endregion

        #region Constructor

        public EntreeController(
            ILogger<EntreeController> logger,
            IMapper mapper,
            IAuthorizationRepo authorizationRepo,
            ICoffreRepo coffreRepo,
            ICoffreLogRepo coffreLogRepo,
            IEntreeLogRepo entreeLogRepo,
            IEntreeRepo entreeRepo,
            IUserCoffreRepo userCoffreRepo,
            IUserRepo userRepo,
            IHttpContextAccessor httpContext
            )
        {
            _logger = logger;
            _mapper = mapper;
            _authorizationRepo = authorizationRepo;
            _coffreRepo = coffreRepo;
            _coffreLogRepo = coffreLogRepo;
            _entreeLogRepo=entreeLogRepo;
            _entreeRepo = entreeRepo;
            _userCoffreRepo = userCoffreRepo;
            _userRepo = userRepo;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }
        #endregion

        #region Read multi by CoffreId

        // GET: api/<EntreeController>
        [HttpGet("coffre/{coffreId}")]
        public async Task<IActionResult> GetEntreesByCoffreId(int coffreId)
        {
            _logger.LogDebug($"Get All entrees of coffre id{nameof(GetEntreesByCoffreId)} CoffreId {coffreId}");
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
                IEnumerable<Entree>? entrees = await _entreeRepo.FindbyCoffreId(coffreId);
                if (entrees == null) 
                {
                    actionResult = NotFound();
                }
                else
                {               
                    List<EntreeReadOnlyDto> response = _mapper.Map<List<EntreeReadOnlyDto>>(entrees);
                    actionResult= Ok(response);
                }

                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetEntreesByCoffreId)}");
                return Problem($"Something went wrong in the {nameof(GetEntreesByCoffreId)}", statusCode: 500);
            }
        }
        #endregion

        #region Read one by Id
        // GET api/<EntreeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug($"Get entree by id{nameof(GetById)} entree Id {id}");
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
                Entree? entree = await _entreeRepo.FindbyId(id);
                Coffre? coffreRelated = await _coffreRepo.FindbyId(id);

                if (coffreRelated != null)
                {
                    bool isAdmin = await _authorizationRepo.VerifyAdminAccess(coffreRelated.Id);
                    bool isWrite = await _authorizationRepo.VerifyWriteAccess(coffreRelated.Id);
                    bool isRead = await _authorizationRepo.VerifyReadAccess(coffreRelated.Id);
                    if (!isAdmin)
                    {

                    }
                }
                if (entree == null)
                {
                    actionResult = NotFound();
                }
                else
                {
                    EntreeReadOnlyDto response = _mapper.Map<EntreeReadOnlyDto>(entree);
                    actionResult = Ok(response);
                    EntreeHistory log = new EntreeHistory
                    {
                        DateOperation = DateTime.UtcNow,
                        Operation = PwdManager.Shared.Operation.Read,
                        UserId = user?.UserId ?? throw new Exception("User Id null"),
                        EntreeId = id
                    };
                    await _entreeLogRepo.Add(log);
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetEntreesByCoffreId)}");
                return Problem($"Something went wrong in the {nameof(GetEntreesByCoffreId)}", statusCode: 500);
            }
        }
        #endregion

        #region Create

        // POST api/<EntreeController>
        [HttpPost]
        public async Task<IActionResult> CreateEntree([FromBody] EntreeCreateDto dto)
        {
            _logger.LogDebug($"Create {nameof(CreateEntree)} {dto.CoffreId}");
            try
            {
                ActionResult actionResult = NotFound();
                string? azureId = _httpContext.HttpContext?.User?.Identity?.Name ?? throw new Exception("");
                ApiUser? user = await _userRepo.CheckUser(azureId);
                if (user == null)
                {
                    await _userRepo.AddUser(azureId);
                }
                user = await _userRepo.CheckUser(azureId);
                Coffre? coffreRelated = await _coffreRepo.FindbyId(dto.CoffreId);
                bool isAdmin = await _authorizationRepo.VerifyAdminAccess(dto.CoffreId);
                bool isWrite = await _authorizationRepo.VerifyWriteAccess(dto.CoffreId);

                if (coffreRelated == null)
                {
                    actionResult = NotFound($"{dto.CoffreId} of coffre not found");
                }
                else if (!isAdmin && !isWrite)
                {
                    actionResult = Forbid($"You don't have authorization to modify this vault : {dto.CoffreId}");
                }
                else { 
                
                    Entree entree = _mapper.Map<Entree>(dto);
                    int response = await _entreeRepo.Create(entree);
                    if (response > 1)
                    {
                        EntreeHistory log = new EntreeHistory
                        {
                            DateOperation = DateTime.UtcNow,
                            Operation = PwdManager.Shared.Operation.Create,
                            UserId = user?.UserId ?? throw new Exception("User Id null"),
                            EntreeId = response,
                            CoffreId = dto.CoffreId,
                            EncryptedLogin = dto.EncryptedLogin,
                            EncryptedPwd = dto.EncryptedPwd,
                            EncryptedURL = dto.EncryptedURL ?? null,
                            TagLogin = dto.TagLogin,
                            TagPwd = dto.TagPwd,
                            TagUrl = dto.TagUrl,
                            IVLogin = dto.IVLogin,
                            IVPwd = dto.IVPwd,
                            IVUrl = dto.IVUrl ?? null,
                            EntreeName= coffreRelated.Title +" " + dto.CoffreId.ToString()+" " + response+" "+user.AzureId
                        };
                        await _entreeLogRepo.Add(log);
                    }
                    actionResult = Created("Created", new { });
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateEntree)}");
                return Problem($"Something went wrong in the {nameof(CreateEntree)}", statusCode: 500);
            }
        }
        #endregion

        #region Update

        // PUT api/<EntreeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntree(int id, [FromBody] EntreeDto dto)
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
                Entree? entreeToUpdate = await _entreeRepo.FindbyId(dto.Id);
                Coffre? coffreRelated = await _coffreRepo.FindbyId(dto.CoffreId);
                bool isAdmin = await _authorizationRepo.VerifyAdminAccess(dto.CoffreId);
                bool isWrite = await _authorizationRepo.VerifyWriteAccess(dto.CoffreId);
                if (id!=dto.Id)
                {
                    actionResult = BadRequest();
                }
                else if (entreeToUpdate == null)
                {
                    actionResult = NotFound($"{id} of entree not found");
                }
                else if (coffreRelated == null)
                {
                    actionResult = NotFound($"{dto.CoffreId} of coffre not found");
                }
                else if (!isAdmin && !isWrite)
                {
                    actionResult = Forbid($"You don't have authorization to modify this vault : {dto.CoffreId}");
                }
                else
                {
                    Entree entree = _mapper.Map<Entree>(dto);
                    bool updated = await _entreeRepo.Update(entree);
                    if (updated == true)
                    {
                        EntreeHistory log = new EntreeHistory
                        {
                            DateOperation = DateTime.UtcNow,
                            Operation = PwdManager.Shared.Operation.Update,
                            UserId = user?.UserId ?? throw new Exception("User Id null"),
                            EntreeId = dto.Id,
                            EncryptedLogin = entreeToUpdate.EncryptedLogin,
                            EncryptedPwd = entreeToUpdate.EncryptedPwd,
                            EncryptedURL = entreeToUpdate.EncryptedURL ?? null,
                            TagLogin = entreeToUpdate.TagLogin,
                            TagPwd = entreeToUpdate.TagPwd,
                            TagUrl = entreeToUpdate.TagUrl,
                            IVLogin = entreeToUpdate.IVLogin,
                            IVPwd = entreeToUpdate.IVPwd,
                            IVUrl = entreeToUpdate.IVUrl ?? null,
                            EntreeName = coffreRelated.Title + " " + dto.CoffreId + " " + dto.Id +" " +user.AzureId

                        };
                        await _entreeLogRepo.Add(log);
                        actionResult = Accepted("Updated", new { dto });
                    }
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateEntree)}");
                return Problem($"Something went wrong in the {nameof(UpdateEntree)}", statusCode: 500);
            }
        }

        #endregion
        #region Delete

        // DELETE api/<EntreeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntree(int id)
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
                Entree entreeToRemove = (await _entreeRepo.FindbyId(id))??throw new Exception("Entree not found");
                Coffre? coffreRelated = await _coffreRepo.FindbyId(entreeToRemove.CoffreId??throw new Exception("CoffreId null"));
                if (coffreRelated is not null)
                {
                    bool isAdmin = await _authorizationRepo.VerifyAdminAccess(coffreRelated.Id);
                    if (!isAdmin)
                    {
                        actionResult = Forbid("you are not admin of this vault!");
                        return actionResult;
                    }
                }
                if (entreeToRemove == null)
                {
                    actionResult = NotFound($"{id} of entree not found");
                }
                else
                {   
                    bool? deleted = await _entreeRepo.Delete(entreeToRemove);
                    if (deleted == true)
                    {
                        EntreeHistory log = new EntreeHistory
                        {
                            DateOperation = DateTime.UtcNow,
                            Operation = PwdManager.Shared.Operation.Delete,
                            UserId = user?.UserId ?? throw new Exception("User Id null"),
                            //EntreeId = id,
                            EncryptedLogin = entreeToRemove.EncryptedLogin,
                            EncryptedPwd = entreeToRemove.EncryptedPwd,
                            EncryptedURL = entreeToRemove.EncryptedURL ?? null,
                            TagLogin = entreeToRemove.TagLogin,
                            TagPwd = entreeToRemove.TagPwd,
                            TagUrl = entreeToRemove.TagUrl,
                            IVLogin = entreeToRemove.IVLogin,
                            IVPwd = entreeToRemove.IVPwd,
                            IVUrl = entreeToRemove.IVUrl ?? null,
                            //EntreeHistoryId = id,
                            EntreeName = coffreRelated?.Title +" "+ coffreRelated?.Id +" " + id + " " + user.AzureId,
                        };
                        await _entreeLogRepo.Add(log);
                        actionResult = Ok("Deleted");
                    }
                }
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteEntree)}");
                return Problem($"Something went wrong in the {nameof(DeleteEntree)}", statusCode: 500);
            }
        }
        #endregion      
    }
}
