using Microsoft.AspNetCore.SignalR;

namespace Rhythia.Server.Hubs;

public static class HubCallerContextExtensions
{
    public static int GetUserId(this HubCallerContext context)
    {
        if (context.UserIdentifier == null)
            throw new InvalidOperationException("User has no identifier");
        return int.Parse(context.UserIdentifier);
    }

    public static string GetToken(this HubCallerContext context)
    {
        var discordToken = context.User?.FindFirst(claim => claim.Type == "discord")?.Value;
        if (discordToken == null)
            return $"Guest{GetUserId(context)}";
        return discordToken;
    }
}