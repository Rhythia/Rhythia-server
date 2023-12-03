using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Rhythia.Server.Authentication.Discord;

public class DiscordAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AUTH_SCHEME = "Discord";
    private Dictionary<string, DiscordUser> knownUsers = new();

    protected DiscordAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock)
    {}

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? token = null;
        if (Context.Request.Headers.TryGetValue("Token", out var tokenValue))
            token = tokenValue;
        if (token == null)
            return AuthenticateResult.Fail("No token");
        var user = await GetUserFromToken(token);
        var nameClaim = new Claim(ClaimTypes.NameIdentifier, user.Id);
        var tokenClaim = new Claim("discord", token);
        var authenticationTicket = new AuthenticationTicket(
            new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { nameClaim, tokenClaim }, AUTH_SCHEME) }),
            new AuthenticationProperties(),
            AUTH_SCHEME);
        return AuthenticateResult.Success(authenticationTicket);
    }
    public async Task<DiscordUser> GetUserFromToken(string token)
    {
        DiscordUser? user;
        if (knownUsers.TryGetValue(token, out user))
            return user;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.GetAsync("https://discord.com/api/oauth2/@me");
        if (result.StatusCode != HttpStatusCode.OK)
            throw new InvalidCredentialException("Token is invalid");
        var response = await result.Content.ReadFromJsonAsync<DiscordResponse>();
        if (response == null)
            throw new InvalidDataException("Response is invalid");
        user = response.User;
        if (user == null)
            throw new InvalidDataException("Response is invalid");
        knownUsers[token] = user;
        return user;
    }
}