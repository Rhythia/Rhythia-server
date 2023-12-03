using System.Text.Json.Serialization;

namespace Rhythia.Server.Authentication.Discord;

[Serializable]
public class DiscordResponse
{
    [JsonPropertyName("user")] public DiscordUser User { get; set; }
}
[Serializable]
public class DiscordUser
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("avatar")] public string Avatar { get; set; }
}