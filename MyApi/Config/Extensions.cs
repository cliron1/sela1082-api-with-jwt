using System.Security.Cryptography;
using System.Text;

namespace MyApi.Config;

public static class Extensions {
    public static string Hash(this string src) {
        using var md5 = MD5.Create();
        return Convert.ToHexString(
            md5.ComputeHash(Encoding.ASCII.GetBytes(src))
        );
    }
}
