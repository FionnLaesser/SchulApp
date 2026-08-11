namespace SchulApp
{
    public static class BcryptHasher
    {
        public static string HashPassword(string passwort)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwort);
        }
    }
}