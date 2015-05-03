using System.Security.Cryptography;
using System.Text;

namespace RAOServer.Utils {
    internal class Auth {

        public static byte[] GetHash(string inputString) {
            HashAlgorithm algorithm = SHA1.Create(); // SHA1.Create()
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }


        public static string GetHashString(string inputString) {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}