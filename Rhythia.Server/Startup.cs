using Microsoft.AspNetCore.Authentication;
using Rhythia.Server.Authentication.Discord;
using Rhythia.Server.Entities;
using Rhythia.Server.Hubs.Spectator;

namespace Rhythia.Server;

public class Startup
{
    public void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = DiscordAuthenticationHandler.AUTH_SCHEME;
                options.DefaultChallengeScheme = DiscordAuthenticationHandler.AUTH_SCHEME;
            })
            .AddScheme<AuthenticationSchemeOptions, DiscordAuthenticationHandler>(DiscordAuthenticationHandler.AUTH_SCHEME, options => {});
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR()
            .AddMessagePackProtocol();
        
        ConfigureAuthentication(services);
        
        // Hub singletons
        services.AddSingleton<EntityStore<SpectatorClientState>>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseAuthentication();

        app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/status", async (context) =>
            {
                var response = context.Response;
                var identity = context.User.Identity;
                if (identity is { IsAuthenticated: true })
                {
                    await response.WriteAsync("ok");
                }
                else
                {
                    response.StatusCode = 403;
                    await response.WriteAsync("not authenticated");
                }
            });
            endpoints.MapHub<SpectatorHub>("/spectator");
            // endpoints.MapHub<MultiplayerHub>("/multiplayer");
        });
    }
}