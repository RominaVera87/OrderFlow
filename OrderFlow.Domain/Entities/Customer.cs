namespace OrderFlow.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    private Customer()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    public Customer(
        Guid id,
        string name,
        string email)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer id cannot be empty.",
                nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Customer email is required.",
                nameof(email));
        }

        Id = id;
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
    }
}