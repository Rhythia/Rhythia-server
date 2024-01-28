namespace Rhythia.Server;

// This entire project references https://github.com/ppy/osu-server-spectator/ a lot.
// If anything doesn't make sense, please see the better documented code in that repo.
// Few exceptions:
//  - Authentication
//  - Any reference to Rhythia.Core
public static class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
        {
            builder.UseStartup<Startup>();
            builder.UseUrls("http://localhost:5000");
        });
    }
}