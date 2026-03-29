namespace BasicExample.MultipleSchemes
{
    public interface IPasswordHasher
    {
        bool Verify(string passwordHash, string plainTextPassword);
    }
}
