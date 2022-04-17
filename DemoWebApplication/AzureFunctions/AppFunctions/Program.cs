using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using AppFunctions.Services;

namespace AppFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration(config =>
                {
                    // #1 APPROACH FOR ADDING AZURE KEY VAULT AND GETTING DATA 
                    // this will add Azure Key Vault to your app Configuration Provider and you can get the value
                    // by injecting IConfiguration in your class constructor and call _configuration["YourKeyStoredInAzureKeyVault"]
                    config.AddAzureKeyVault(new SecretClient(new Uri(Environment.GetEnvironmentVariable("KeyVaultUri")), new DefaultAzureCredential()), new KeyVaultSecretManager());
                })
                .ConfigureServices(services =>
                {
                    // #2 APPROACH FOR ADDING AZURE KEY VAULT AND GETTING DATA
                    services.AddAzureClients(builder =>
                    {
                        // Add a KeyVault client (SecretClient)
                        builder.AddSecretClient(new Uri(Environment.GetEnvironmentVariable("KeyVaultUri")));
                    });

                    services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
                })
                .Build();

            host.Run();
        }
    }
}