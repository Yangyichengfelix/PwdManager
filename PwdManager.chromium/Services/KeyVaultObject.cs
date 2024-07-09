using System.Runtime;

namespace PwdManager.chromium.Services
{
    public class KeyVaultObject
    {
		private int _vaultId;

		public int VaultId
		{
			get { return _vaultId; }
			set { _vaultId = value; }
		}
		private string _privateKey;

		public string PrivateKey
        {
			get { return _privateKey; }
			set { _privateKey = value; }
		}


	}
}
