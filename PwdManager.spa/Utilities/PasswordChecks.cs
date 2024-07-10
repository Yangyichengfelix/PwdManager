using System.Text.RegularExpressions;

namespace PwdManager.spa.Utilities
{
    public static class PasswordChecks
    {
        /// <summary>
        /// verify password strength
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public static IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Il faut un mot de passe!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Le mot de passe doit avoir minimum 8";
            if (pw.Length > 16)
                yield return "Le mot de passe doit avoir maximum 16";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Le mot de passe doit avoir au moins 1 lettre majuscule";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Le mot de passe doit avoir au moins 1 lettre miniscule";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Le mot de passe doit avoir au moins 1 numéro";
            if (!Regex.IsMatch(pw, @"[\*@!#%&\(\)\^~{}\\.]"))
                yield return "le mdp doit contenir un caractère spécial,ex: *@!#%&\\()^~{}./";
        }
        /// <summary>
        /// verify if password matches with password confirm
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string? PasswordMatch(string check, string arg)
        {
            if (check != arg)
                return "les deux saisies de mdp ne sont pas identiques";
            return null;
        }
    }
}
