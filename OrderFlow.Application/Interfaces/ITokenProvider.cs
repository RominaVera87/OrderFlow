using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces;

public interface ITokenProvider
{
    string CreateToken(User user);
}
