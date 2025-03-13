#if WINDOWS
using System.Security.Cryptography;
#endif
using System.Text;

namespace AO3Statistics;

internal static class PasswordEncryptor
{
    /// <summary>
    /// Encrypts the provided password.
    /// </summary>
    /// <remarks>
    /// On Windows this utilizes DPAPI to encrypt the password. Not encrypted on other operating systems.
    /// </remarks>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string ProtectPassword(string password)
    {
#if WINDOWS
        byte[] unprotectedPasswordBytes = Encoding.Unicode.GetBytes(password);
        byte[] protectedPasswordBytes = ProtectedData.Protect(unprotectedPasswordBytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToHexString(protectedPasswordBytes);
#else
        byte[] unprotectedPasswordBytes = Encoding.Unicode.GetBytes(password);
        return Convert.ToHexString(unprotectedPasswordBytes);
#endif
    }

    /// <summary>
    /// Decrypts the provided password.
    /// </summary>
    /// <param name="protectedPassword"></param>
    /// <returns></returns>
    public static string UnProtectPassword(string protectedPassword)
    {
#if WINDOWS
        byte[] protectedPasswordBytes = Convert.FromHexString(protectedPassword);
        byte[] unProtectedPasswordBytes = ProtectedData.Unprotect(protectedPasswordBytes, null, DataProtectionScope.CurrentUser);
        return Encoding.Unicode.GetString(unProtectedPasswordBytes);
#else
        byte[] unprotectedPasswordBytes = Convert.FromHexString(protectedPassword);
        return Encoding.Unicode.GetString(unprotectedPasswordBytes);
#endif
    }
}