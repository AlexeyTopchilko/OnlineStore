using Elskom.Generic.Libs;

namespace AuthenticationMicroservice.Service.Services.Encryption
{
    public static class Encryption
    {
        private static readonly byte[] Key = { 50, 199, 10, 159, 132, 55, 236, 189, 51, 243, 244, 91, 17, 136, 39, 230 };
        private static readonly BlowFish BlowFish = new(Key);

        public static string EncrypteString(this string str)
        {
            return BlowFish.EncryptECB(str);
        }
    }
}