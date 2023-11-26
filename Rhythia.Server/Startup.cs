using Microsoft.AspNetCore.Authentication;
using Rhythia.Server.Authentication;

namespace Rhythia.Server;

public class Startup
{
    public void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = GuestAuthenticationHandler.AUTH_SCHEME;
                options.DefaultChallengeScheme = GuestAuthenticationHandler.AUTH_SCHEME;
            })
            .AddScheme<AuthenticationSchemeOptions, GuestAuthenticationHandler>(GuestAuthenticationHandler.AUTH_SCHEME, options => {});
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR()
            .AddMessagePackProtocol();
        
        // Hub singletons
        // services.AddSingleton<>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            // endpoints.MapHub<SpectatorHub>("/spectator");
            // endpoints.MapHub<MultiplayerHub>("/multiplayer");
        });
    }
}