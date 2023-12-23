using Microsoft.AspNetCore.SignalR;

namespace Rhythia.Server.Hubs;

public static class HubCallerContextExtensions
{
    public static string GetUserId(this HubCallerContext context)
    {
        if (context.UserIdentifier == null)
            throw new InvalidOperationException("User has no identifier");
        return context.UserIdentifier;
    }

    public static string GetUserName(this HubCallerContext context)
    {
        var displayName = context.User?.FindFirst(claim => claim.Type == "display")?.Value;
        return displayName ?? "Guest";
    }
    public static string GetToken(this HubCallerContext context)
    {
        var token = context.User?.FindFirst(claim => claim.Type == "discord")?.Value;
        return token ?? $"Guest{GetUserId(context)}";
    }
}