using System.Security.Principal;

namespace Rhythia.Server.Authentication;

public class GuestIdentity : IIdentity
{
    public string AuthenticationType => GuestAuthenticationHandler.AUTH_SCHEME;
    public bool IsAuthenticated => true;
    public string Name { get; }

    public GuestIdentity(string name)
    {
        Name = name;
    }
}