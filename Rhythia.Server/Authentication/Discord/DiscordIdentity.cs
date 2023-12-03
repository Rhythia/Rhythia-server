using System.Security.Principal;

namespace Rhythia.Server.Authentication.Discord;

public class DiscordIdentity : IIdentity
{
    public string AuthenticationType => DiscordAuthenticationHandler.AUTH_SCHEME;
    public bool IsAuthenticated => true;
    public string Name { get; }

    public DiscordIdentity(string name)
    {
        Name = name;
    }
}