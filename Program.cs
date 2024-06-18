using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Create a temporary configuration builder to read the initial configuration
var tempConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var keyVaultEndpoint = tempConfig["https://kvyoutubedemowithdotnet.vault.azure.net/"];

if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        credential
    );
}

var app = builder.Build();

app.MapGet("/", (IConfiguration config) =>
{
    // Access a secret from Key Vault
    var secretValue = config["KeyVaultDemo-ConnectionStrings--DefaultConnection"];
    return Results.Ok($"Secret Value: {secretValue}");
});

app.Run();
