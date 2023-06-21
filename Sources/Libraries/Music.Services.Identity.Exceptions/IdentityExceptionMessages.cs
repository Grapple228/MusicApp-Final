namespace Music.Services.Identity.Exceptions;

public static class IdentityExceptionMessages
{
    public const string UsernameExists = "User with username already exists";
    public const string EmailExists = "Account with this email already exists";
    public const string InvalidLoginData = "Invalid username or password";
    public const string InvalidRefreshToken = "Invalid refresh token";
    public const string RoleNotFound = "Role not found";
}