namespace PwdManager.srv.Contracts
{
    public interface IAuthorizationRepo
    {
        Task<bool> VerifyReadAccess(int coffreId);
        Task<bool> VerifyWriteAccess(int coffreId);
        Task<bool> VerifyAdminAccess(int coffreId);
    }
}
