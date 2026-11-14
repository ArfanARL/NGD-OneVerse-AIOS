namespace NGD.OneVerse.Api.Contracts.Auth;

public sealed class CurrentUserResponse
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } =
        Array.Empty<string>();

    public IReadOnlyCollection<string> Permissions { get; set; } =
        Array.Empty<string>();
}
