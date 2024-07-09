namespace PwdManager.Shared.TableInfos
{
    public abstract class BaseNotification
    {
        public string table { get; set; } = "";
        public string action { get; set; } = "";
    }
}
