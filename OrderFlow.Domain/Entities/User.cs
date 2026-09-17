using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(
        Guid id,
        string email,
        string passwordHash,
        UserRole role,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "User id cannot be empty.",
                nameof(id));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "User email is required.",
                nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException(
                "Password hash is required.",
                nameof(passwordHash));
        }

        Id = id;
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        CreatedAtUtc = createdAtUtc;
    }
}