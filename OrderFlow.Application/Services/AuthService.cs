using OrderFlow.Application.DTOs.Auth;
using OrderFlow.Application.Exceptions;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser =
            await _userRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new AuthenticationException(
                "A user with this email already exists.");
        }

        var passwordHash =
            _passwordHasher.Hash(request.Password);

        var user = new User(
            Guid.NewGuid(),
            request.Email,
            passwordHash,
            UserRole.Customer,
            DateTime.UtcNow);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _userRepository.SaveChangesAsync(
            cancellationToken);

        return CreateResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user =
            await _userRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

        if (user is null)
        {
            throw new AuthenticationException(
                "Invalid email or password.");
        }

        var passwordIsValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new AuthenticationException(
                "Invalid email or password.");
        }

        return CreateResponse(user);
    }

    private AuthResponse CreateResponse(User user)
    {
        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = _tokenProvider.CreateToken(user)
        };
    }
}