using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Create a temporary configuration builder to read the initial configuration
var tempConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var keyVaultEndpoint = tempConfig["AzureKeyVaultEndpoint"];

if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        credential
    );
}

var app = builder.Build();

// Serve static files from wwwroot folder
app.UseStaticFiles();

// Enable default files mapping (like index.html)
app.UseDefaultFiles();

// Use routing
app.UseRouting();

// Configure the endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/secret", (IConfiguration config) =>
    {
        var secretValue = config["MySecretKey"];
        return Results.Ok($"Secret Value: {secretValue}");
    });

    endpoints.MapFallbackToFile("index.html"); // Fallback to serve index.html for any other route
});

app.Run();
