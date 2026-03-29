namespace BasicExample.CustomValidator
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const string Prefix = "hash::";

        public bool Verify(string passwordHash, string plainTextPassword)
        {
            return passwordHash == Prefix + plainTextPassword;
        }

        public static string Hash(string plainTextPassword)
        {
            return Prefix + plainTextPassword;
        }
    }
}
