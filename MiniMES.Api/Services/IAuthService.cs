using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<UserDto?> GetCurrentUserAsync(string username);
}
