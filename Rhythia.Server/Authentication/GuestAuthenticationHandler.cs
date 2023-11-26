using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Rhythia.Server.Authentication;

public class GuestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AUTH_SCHEME = "Guest";
    private static int userIdCounter = 0;
    private static Dictionary<string, int> rememberedUserIds = new();

    protected GuestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
        )
        : base(options, logger, encoder, clock)
    {}  
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        int userId = 0;
        string? guestToken = null;
        if (Context.Request.Headers.TryGetValue("guest_id", out var guestTokenValue))
            guestToken = guestTokenValue;
        if (guestToken == null)
            return Task.FromResult(AuthenticateResult.Fail("Can't connect as guest with no id"));
        if (!rememberedUserIds.TryGetValue(guestToken, out userId))
        {
            userId = ++userIdCounter;
            rememberedUserIds[guestToken] = userId;
        }
        var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
        var authenticationTicket = new AuthenticationTicket(
            new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }, AUTH_SCHEME) }),
            new AuthenticationProperties(),
            AUTH_SCHEME);
        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}