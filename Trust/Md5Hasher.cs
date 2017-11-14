using System.Text;

namespace Trust
{
    internal class Md5Hasher
    {
        private static readonly uint[] _lookup32 = CreateLookup32();

        private readonly System.Security.Cryptography.MD5 md5;

        private static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            var lookup32 = _lookup32;
            var result = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var s = i.ToString("X2");
                result[i] = s[0] + ((uint)s[1] << 16);
            }
            return result;
        }

        public Md5Hasher()
        {
            md5 = System.Security.Cryptography.MD5.Create();
        }

        public string CalculateMD5Hash(string input)
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            return ByteArrayToHexViaLookup32(hash);
        }
    }
}