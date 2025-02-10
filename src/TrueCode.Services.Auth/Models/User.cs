namespace TrueCode.Services.Auth.Models;

public class User
{
    public int Id { get; internal init; }
    public string Name { get; init; }
    public string Password { get; init; }

    public User(string name, string password)
    {
        Name = name;
        Password = password;
    }

    private User()
    {
    }
}