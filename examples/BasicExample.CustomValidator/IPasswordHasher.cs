namespace BasicExample.CustomValidator
{
    public interface IPasswordHasher
    {
        bool Verify(string passwordHash, string plainTextPassword);
    }
}
