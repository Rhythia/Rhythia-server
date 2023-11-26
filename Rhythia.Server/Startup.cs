namespace Rhythia.Server;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR()
            .AddMessagePackProtocol();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            
        });
    }
}