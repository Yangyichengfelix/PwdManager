using System;

namespace PwdManager.chromium.Services
{
    public class KeyService
    {

        //private string _privateKey;
        //public string PrivateKey
        //{
        //    get
        //    {
        //        return _privateKey;
        //    }
        //    set
        //    {
        //        _privateKey = value;
        //        NotifyDataChanged();
        //    }
        //}


        private List<KeyVaultObject> _keyVaults=new List<KeyVaultObject>();
        public List<KeyVaultObject> keyVaults
        {
            get
            {
                return _keyVaults;
            }
            set
            {
                _keyVaults = value;
                NotifyDataChanged();
            }
        }
        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
    }
}
