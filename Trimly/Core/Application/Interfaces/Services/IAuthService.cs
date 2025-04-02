using Trimly.Core.Application.DTOs.Register;
using Trimly.Core.Application.DTOs.ResetPassword;

namespace Trimly.Core.Application.Interfaces.Services;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(string email, string password);
    Task<RegisterResponseDTO> RegisterAsync(string endpoint, RegisterRequestDTO request);
    Task<ConfirmAccountResponseDTO> ConfirmAccountAsync(string userId, string token);
    Task LogoutAsync();
    Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<ConfirmResetPasswordResponseDTO> ConfirmResetPasswordAsync(ConfirmResetPasswordRequestDTO request);
}