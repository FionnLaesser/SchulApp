string passwort = "admin";

string hash = BCrypt.Net.BCrypt.HashPassword(passwort);

Console.WriteLine(hash);