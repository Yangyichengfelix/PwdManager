namespace PwdManager.spa.Services
{
    public class KeyService
    {

        private string _privateKey="";
        public string PrivateKey
        {
            get
            {
                return _privateKey;
            }
            set
            {
                _privateKey = value;
                NotifyDataChanged();
            }
        }

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
    }
}
