using Proxy;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllers();


builder.Services.AddHttpClient("Monolith", c =>
{
    c.BaseAddress = new Uri(config["MONOLITH_URL"]);
});

builder.Services.AddHttpClient("Movies", c =>
{
    c.BaseAddress = new Uri(config["MOVIES_SERVICE_URL"]);
});

builder.Services.AddTransient<IProxyFacade>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var proxyOptions = new IProxyFacade.ProxyOptions
    {
        GradualMigration = configuration.GetValue<bool>("GRADUAL_MIGRATION"),
        MoviesMigrationPercent = configuration.GetValue<int>("MOVIES_MIGRATION_PERCENT")
    };
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var monolithHttpClient = factory.CreateClient("Monolith");
    var moviesHttpClient = factory.CreateClient("Movies");
    return new ProxyFacade(monolithHttpClient, moviesHttpClient, proxyOptions);
});


var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/api/proxy/health", () => Results.Ok(new { status = true }));
    endpoints.MapControllers();
});

app.Run();
